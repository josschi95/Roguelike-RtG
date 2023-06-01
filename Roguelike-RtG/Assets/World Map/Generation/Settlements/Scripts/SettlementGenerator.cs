using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JS.WorldMap.Generation
{
    public class SettlementGenerator : MonoBehaviour
    {
        [SerializeField] private WorldGenerator worldGenerator;
        [SerializeField] private WorldGenerationParameters mapFeatures;
        [SerializeField] private TerrainData terrainData;
        [SerializeField] private BiomeHelper biomeHelper;
        //[SerializeField] private SettlementData settlementData;
        [SerializeField] private WorldData worldMap;
        [SerializeField] private TribalRelation tribeRelations;
        private WorldSize worldSize;

        [Space]

        [SerializeField] private HumanoidTribe[] tribes;
        [SerializeField] private SettlementType[] settlementTypes;
        private int[] settlementCount;

        private List<WorldTile> availableNodes;
        private List<Settlement> settlements;
        private Dictionary<HumanoidTribe, List<WorldTile>> tribeTerritories;
        private Dictionary<Biome, List<WorldTile>> biomes;

        [SerializeField] private bool usePoisson;
        [SerializeField] private int poissonRadius = 15;
        [SerializeField] private int landCheckRadius = 5;


        public void SetInitialValues(WorldSize size)
        {
            worldSize = size;
            settlements = new List<Settlement>();

            settlementCount = mapFeatures.SettlementCount(worldSize);
            for (int i = 0; i < settlementCount.Length; i++)
            {
                settlementCount[i] *= tribes.Length;
            }

            tribeTerritories = new Dictionary<HumanoidTribe, List<WorldTile>>();
            for (int i = 0; i < tribes.Length; i++)
            {
                tribeTerritories[tribes[i]] = new List<WorldTile>();
            }

            biomes = new Dictionary<Biome, List<WorldTile>>();
            for (int i = 0; i < biomeHelper.Biomes.Length; i++)
            {
                biomes[biomeHelper.Biomes[i]] = new List<WorldTile>();
            }
        }

        private HumanoidTribe GetTribe(int ID)
        {
            for (int i = 0; i < tribes.Length; i++)
            {
                if (tribes[i].ID == ID) return tribes[i];
            }
            throw new System.Exception("Tribe ID not found.");
        }

        #region - Settlement Placement -
        public void PlaceSettlements()
        {
            var size = new Vector2(worldMap.Width, worldMap.Height);
            var points = Poisson.GeneratePoints(worldMap.Seed, poissonRadius, size);
            int tribeIndex = 0;
            for (int i = 0; i < points.Count; i++)
            {
                var pos = new Vector3Int((int)points[i].x, (int)points[i].y);
                var node = worldMap.GetNode(pos.x, pos.y);

                //Try to correct to a land-node
                if (!node.IsLand) node = TryFindLand(node);
                if (node == null) continue;

                var tribe = tribes[tribeIndex];
                var newSettlement = PlaceSettlement(node, GetSettlementType(), tribe);
                tribeIndex++;
                if (tribeIndex >= tribes.Length) tribeIndex = 0;
            }

            //CenterSettlements();

            //CleanUpMinorSettlements();

            //Set Area of Influence
            //Based on settlement size
            //Probably use flood fill again
            //GetSettlementConnections();

            //GetTerritoryReport();


            worldMap.SettlementData.PlaceSettlements(settlements.ToArray());
        }

        private WorldTile TryFindLand(WorldTile tile)
        {
            var nodes = worldMap.GetNodesInRange_Square(tile, landCheckRadius);
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].IsLand) return nodes[i];
            }
            return null;
        }


        private SettlementType GetSettlementType()
        {
            for (int i = 0; i < settlementCount.Length; i++)
            {
                if (settlementCount[i] > 0)
                {
                    settlementCount[i]--;
                    return settlementTypes[i];
                }
            }
            return settlementTypes[settlementTypes.Length - 1];
        }

        private HumanoidTribe GetTribeBids(Settlement settlement)
        {
            var node = worldMap.GetNode(settlement.X, settlement.Y);
            var biome = biomeHelper.GetBiome(node.BiomeID);

            var tribalBids = new List<HumanoidTribe>(tribes);

            for (int i = 0; i < tribes.Length; i++)
            {
                if (tribes[i].preferredBiomes.Contains(biome)) tribalBids.Add(tribes[i]);
                if (!tribes[i].opposedBiomes.Contains(biome)) tribalBids.Add(tribes[i]);

            }


            return tribalBids[worldGenerator.rng.Next(0, tribalBids.Count)];
        }

        private void ExpandTerritory(Settlement settlement)
        {
            //clamp max size based on settlement size

            int[,] mapFlags = new int[worldMap.Width, worldMap.Height];
            Queue<WorldTile> queue = new Queue<WorldTile>();
            queue.Enqueue(worldMap.GetNode(settlement.X, settlement.Y));

            //So this is going to repeat the same territory every single time, which seems a waste
            while(queue.Count > 0)// && territoryToAdd > 0)
            {
                //Limit settlements from getting too large at the start
                if (settlement.Territory.Count >= settlementTypes[settlement.TypeID].MaxTerritory) break;

                var node = queue.Dequeue();
                if (!settlement.OwnsTerritory(node.x, node.y))
                {
                    AssignTerritory(settlement, node);
                    //territoryToAdd--;
                }

                for (int i = 0; i < node.neighbors_adj.Count; i++)
                {
                    var neighbor = node.neighbors_adj[i];
                    if (mapFlags[neighbor.x, neighbor.y] == 1) continue; //visited
                    
                    mapFlags[neighbor.x, neighbor.y] = 1;
                    if (!neighbor.IsLand) continue; //is water
                    if (settlement.isSubterranean != (neighbor.Mountain != null)) continue; //subterranean can't extend outside mountain, others can't extend into mountains
                    if (GetTribe(settlement.TribeID).opposedBiomes.Contains(biomeHelper.GetBiome(neighbor.BiomeID))) continue; //won't extend into opposed biomes

                    var claimant = FindClaimedTerritory(neighbor.x, neighbor.y);
                    if (claimant != null && claimant != settlement) continue; //already claimed
                    //The distance in node path count from the settlement to the new territory
                    //Also need to take into account going around mountains instead of through them
                    //if (worldMap.GetPathCount(settlement.Node, neighbor, settlement) > 50 + Random.Range(-10, 5)) continue;
                    if (GridMath.GetStraightDist(settlement.X, settlement.Y, neighbor.x, neighbor.y) > 50 + Random.Range(-10, 5)) continue;

                    queue.Enqueue(neighbor);
                }
            }
        }

        /// <summary>
        /// Places a new settlement of the given type and tribe at the node and assigns territory and area of influence
        /// </summary>
        private Settlement PlaceSettlement(WorldTile node, SettlementType type, HumanoidTribe tribe)
        {
            int population = worldGenerator.rng.Next(type.minPopulation, type.maxPopulation);
            var newSettlement = new Settlement("", settlements.Count, node, type, tribe, population);

            //availableNodes?.Remove(node);
            biomes[biomeHelper.GetBiome(node.BiomeID)].Remove(node);

            settlements.Add(newSettlement);
            tribeTerritories[tribe].Add(node);

            return newSettlement;
        }
        #endregion

        private void AssignTerritory(Settlement settlement, WorldTile territory)
        {
            settlement.AddTerritory(territory);
            availableNodes.Remove(territory);
            biomes[biomeHelper.GetBiome(territory.BiomeID)].Remove(territory);

            if (!tribeTerritories[GetTribe(settlement.TribeID)].Contains(territory))
                tribeTerritories[GetTribe(settlement.TribeID)].Add(territory);
        }

        /// <summary>
        /// Recenters settlements closer to the center of their territory
        /// </summary>
        private void CenterSettlements()
        {
            foreach(var settlement in settlements)
            {
                float avgX = 0, avgY = 0;
                for (int i = 0; i < settlement.Territory.Count; i++)
                {
                    avgX += settlement.Territory[i].x;
                    avgY += settlement.Territory[i].y;
                }
                int x = Mathf.RoundToInt(avgX / settlement.Territory.Count);
                int y = Mathf.RoundToInt(avgY / settlement.Territory.Count);

                var newNode = worldMap.GetNode(x, y);
                if (settlement.OwnsTerritory(x, y)) settlement.Relocate(newNode);
                else
                {
                    float dist = int.MaxValue;
                    WorldTile nodeToMove = null;
                    for (int i = 0; i < settlement.Territory.Count; i++)
                    {
                        var newDist = GridMath.GetStraightDist(settlement.Territory[i].x, settlement.Territory[i].y, newNode.x, newNode.y);
                        if (newDist < dist)
                        {
                            nodeToMove = worldMap.GetNode(settlement.Territory[i].x, settlement.Territory[i].y);
                            dist = newDist;
                        }
                    }
                    settlement.Relocate(nodeToMove);
                }
            }
        }

        private void CleanUpMinorSettlements()
        {
            for (int i = settlements.Count - 1; i >= 0; i--)
            {
                var settlement = settlements[i];

                //Need to own at least 5 nodes to exist
                if (settlement.Territory.Count > 4) continue;

                //Find the nearest settlement
                var settlementToMerge = FindNearestSettlement(settlement);

                //Transfers all territory to the chosen settlement and removes it from the tribe
                for (int j = 0; j < settlement.Territory.Count; j++)
                {
                    var node = worldMap.GetNode(settlement.Territory[j].x, settlement.Territory[j].y);
                    tribeTerritories[GetTribe(settlement.TribeID)].Remove(node);
                    AssignTerritory(settlementToMerge, node);
                }

                //Clears all data in the settlement
                settlement.DeconstructSettlement();
                settlements.RemoveAt(i);
            }
        }

        private Settlement FindNearestSettlement(Settlement fromSettlement)
        {
            Settlement toSettlement = null;
            float dist = int.MaxValue;
            for (int i = 0; i < settlements.Count; i++)
            {
                if (settlements[i] == fromSettlement) continue;
                var newDist = GridMath.GetStraightDist(fromSettlement.X, fromSettlement.Y, settlements[i].X, settlements[i].Y);

                if (newDist < dist)
                {
                    toSettlement = settlements[i];
                    dist = newDist;
                }
            }
            return toSettlement;
        }

        private void GetSettlementConnections()
        {
            foreach (Settlement settlement in settlements)
            {
                for (int i = 0; i < settlement.Reach.Count; i++)
                {
                    var newSettlement = FindSettlement(settlement.Reach[i].x, settlement.Reach[i].y);
                    if (newSettlement == null) continue;
                    if (newSettlement == settlement) continue;

                    var disposition = tribeRelations.GetDisposition(GetTribe(settlement.TribeID), GetTribe(newSettlement.TribeID));

                    settlement.AddNewRelation(newSettlement, disposition);
                    newSettlement.AddNewRelation(settlement, disposition);
                    Debug.Log(GetTribe(settlement.TribeID) + " (" + settlement.TypeID + ") at " + settlement.X + "," + settlement.Y +
                        " has encountered a " + GetTribe(newSettlement.TribeID) + " (" + newSettlement.TypeID + ") at " +
                        newSettlement.X + "," + newSettlement.Y + ". They have a disposition of " + disposition);
                }
            }
        }

        private void GetTerritoryReport()
        {
            for (int i = 0; i < tribes.Length; i++)
            {
                int count = 0, hamlets = 0, villages = 0, towns = 0, cities = 0;
                for (int j = 0; j < settlements.Count; j++)
                {
                    if (GetTribe(settlements[j].TribeID) == tribes[i])
                    {
                        count++;
                        if (settlements[j].TypeID == 0) cities++;
                        if (settlements[j].TypeID == 1) towns++;
                        if (settlements[j].TypeID == 2) villages++;
                        if (settlements[j].TypeID == 3) hamlets++;
                    }
                }
                Debug.Log(tribes[i].ToString() + ": " + count + " settlements, " + tribeTerritories[tribes[i]].Count + " nodes, " +
                    "hamlets: " + hamlets + ", villages: " + villages + ", towns: " + towns + ", cities: " + cities);

            }
        }

        public Settlement FindSettlement(int x, int y)
        {
            if (settlements == null) return null;

            for (int i = 0; i < settlements.Count; i++)
            {
                if (settlements[i].X == x && settlements[i].Y == y)
                {
                    return settlements[i];
                }
            }
            return null;
        }

        public Settlement FindClaimedTerritory(int x, int y)
        {
            if (settlements == null) return null;

            for (int i = 0; i < settlements.Count; i++)
            {
                if (settlements[i].OwnsTerritory(x, y)) return settlements[i];
            }
            return null;
        }
    }
}

        #region - Obsolete -
        /*
        private WorldTile GetTribeSettlementSite(HumanoidTribe tribe)
        {
            if (TryFindMountainNode(tribe, out WorldTile mountain)) return mountain;

            if (TryFindIslandNode(tribe, out WorldTile island)) return island;

            //This one here is hurting quite a bit compared to the others
            var initialTime = Time.realtimeSinceStartup;
            if (TryFindPreferredBiome(tribe, out WorldTile biome))
            {
                //Debug.LogWarning("Preferred biome found at " + (Time.realtimeSinceStartup - initialTime));
                return biome;
            }

            if (TryFindUnopposedBiome(tribe, out WorldTile node)) return node;

            if (TryFindAnyNode(out WorldTile lastNode)) return lastNode;

            return null;
        }

        private bool TryFindMountainNode(HumanoidTribe tribe, out WorldTile node)
        {
            node = null;

            var mountainPreference = 15;
            if (tribe.PrefersMountains) mountainPreference = 75;
            if (worldGenerator.rng.Next(0, 100) > mountainPreference) return false;

            var allMountainNodes = new List<WorldTile>();
            foreach (MountainRange mountain in terrainData.Mountains)
            {
                for (int i = 0; i < mountain.Nodes.Count; i++)
                {
                    if (!availableNodes.Contains(mountain.Nodes[i])) continue;
                    allMountainNodes.Add(mountain.Nodes[i]);
                }
            }
            if (allMountainNodes.Count == 0) return false;

            int index = worldGenerator.rng.Next(0, allMountainNodes.Count);
            //Debug.Log("Found Mountain Home");
            node = allMountainNodes[index];
            return true;
        }

        private bool TryFindIslandNode(HumanoidTribe tribe, out WorldTile node)
        {
            node = null;

            var islandPreference = 15;
            if (tribe.PrefersIslands) islandPreference = 75;
            if (worldGenerator.rng.Next(0, 100) > islandPreference) return false;

            var allIslandNodes = new List<WorldTile>();
            foreach (Island island in terrainData.Islands)
            {
                if (island.Nodes.Count < 5) continue; //ignore small islands
                for (int i = 0; i < island.Nodes.Count; i++)
                {
                    if (!availableNodes.Contains(island.Nodes[i])) continue;
                    if (tribe.opposedBiomes.Contains(GetBiome(island.Nodes[i].BiomeID))) continue;
                    allIslandNodes.Add(island.Nodes[i]);
                }
            }
            if (allIslandNodes.Count == 0) return false;

            int index = worldGenerator.rng.Next(0, allIslandNodes.Count);
            //Debug.Log("Found Island Home");
            node = allIslandNodes[index];
            return true;
        }

        private bool TryFindPreferredBiome(HumanoidTribe tribe, out WorldTile node)
        {

            node = null;
            if (tribe.preferredBiomes.Count == 0) return false;
            if (worldGenerator.rng.Next(0, 100) < 25) return false; //25% chance of not getting a preferred biome
            Biome chosenBiome = tribe.preferredBiomes[worldGenerator.rng.Next(0, tribe.preferredBiomes.Count)];

            if (biomes[chosenBiome].Count == 0) return false;
            int index = worldGenerator.rng.Next(0, biomes[chosenBiome].Count);
            node = biomes[chosenBiome][index];
            return true;
        }

        private bool TryFindUnopposedBiome(HumanoidTribe tribe, out WorldTile node)
        {
            node = null;
            var nodeList = new List<WorldTile>();

            for (int i = 0; i < availableNodes.Count; i++)
            {
                if (tribe.opposedBiomes.Contains(GetBiome(availableNodes[i].BiomeID))) continue;
                nodeList.Add(availableNodes[i]);
            }
            if (nodeList.Count == 0) return false;

            int index = worldGenerator.rng.Next(0, nodeList.Count);
            //Debug.Log("Found Unopposed Biome");
            node = nodeList[index];
            return true;
        }

        private bool TryFindAnyNode(out WorldTile node)
        {
            node = null;
            if (availableNodes.Count == 0) return false;

            int index = worldGenerator.rng.Next(0, availableNodes.Count);
            //Debug.Log("Found Any Site");
            node = availableNodes[index];
            return true;
        }
        */
        #endregion