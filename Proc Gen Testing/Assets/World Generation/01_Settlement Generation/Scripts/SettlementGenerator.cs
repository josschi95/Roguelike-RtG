using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JS.WorldGeneration
{
    public class SettlementGenerator : MonoBehaviour
    {
        [SerializeField] private WorldGenerator worldGenerator;
        [SerializeField] private WorldGenerationParameters mapFeatures;
        [SerializeField] private TerrainData terrainData;
        [SerializeField] private WorldMapData worldMap;
        [SerializeField] private TribalRelation tribeRelations;
        private WorldSize worldSize;

        [Space]

        [SerializeField] private HumanoidTribe[] tribes;
        [SerializeField] private SettlementType city;
        [SerializeField] private SettlementType town;
        [SerializeField] private SettlementType village;
        [SerializeField] private SettlementType hamlet;

        private List<TerrainNode> availableNodes;
        private List<Settlement> settlements;
        private Dictionary<HumanoidTribe, List<TerrainNode>> tribeTerritories;

        public void SetInitialValues(WorldSize size)
        {
            worldSize = size;
            settlements = new List<Settlement>();
            
            tribeTerritories = new Dictionary<HumanoidTribe, List<TerrainNode>>();
            for (int i = 0; i < tribes.Length; i++)
            {
                tribeTerritories[tribes[i]] = new List<TerrainNode>();
            }
        }

        #region - Settlement Placement -
        private void GetSiteList()
        {
            availableNodes = new List<TerrainNode>();
            for (int x = 0; x < mapFeatures.MapSize(worldSize); x++)
            {
                for (int y = 0; y < mapFeatures.MapSize(worldSize); y++)
                {
                    TerrainNode node = worldMap.GetNode(x, y);
                    if (!node.isNotWater) continue; //Don't include water
                    if (node.Settlement != null) continue; //this shouldn't happen
                    if (node.Territory != null) continue;
                    availableNodes.Add(node);
                }
            }

            //Shuffle List
            availableNodes = ShuffleList(availableNodes);
        }

        private List<TerrainNode> ShuffleList(List<TerrainNode> list)
        {
            var shuffledList = new List<TerrainNode>(list);
            for (int i = 0; i < shuffledList.Count; i++)
            {
                var temp = shuffledList[i];
                int randomIndex = worldGenerator.rng.Next(i, shuffledList.Count);
                shuffledList[i] = shuffledList[randomIndex];
                shuffledList[randomIndex] = temp;
            }
            return shuffledList;
        }

        private TerrainNode GetRandomSite()
        {
            int index = worldGenerator.rng.Next(0, availableNodes.Count);
            var site = availableNodes[index];
            availableNodes.RemoveAt(index);

            return site;
        }
        
        private List<Biome> ShuffleList(List<Biome> list)
        {
            var shuffledList = new List<Biome>(list);
            for (int i = 0; i < shuffledList.Count; i++)
            {
                var temp = shuffledList[i];
                int randomIndex = worldGenerator.rng.Next(i, shuffledList.Count);
                shuffledList[i] = shuffledList[randomIndex];
                shuffledList[randomIndex] = temp;
            }
            return shuffledList;
        }

        public IEnumerator PlaceSettlements()
        {
            GetSiteList();

            //Next step here is to start clamping the number of cities that can be produced
            //Current report shows that the vast majority of settlements are cities, followed by hamlets
            //and almost no towns or villages are being created
            //This could be due to the size settings I have, but I think I also need to due some more factoring
            //Maybe certain tribes will favor smaller settlements over larger, 
            //I think I also need to start thinking more about the Local Map as well, since each World Tile can be zoomed in to a 200x200 Local Map

            Debug.LogWarning("Pickup from here.");



            while(availableNodes.Count > 0)
            {
                //Debug.Log("Available Nodes: " + availableNodes.Count);
                var newSettlement = TerritoryDraft();

                ExpandTerritory(newSettlement);

                yield return null;
            }

            CenterSettlements();

            CleanUpMinorSettlements();

            SetSettlementSizes();

            //Set Area of Influence
            //Based on settlement size
            //Probably use flood fill again
            //GetSettlementConnections();

            GetTerritoryReport();
        }

        //Grants a new settlement placed on a random node to the highest bidder
        private Settlement TerritoryDraft()
        {
            var randomSite = GetRandomSite();
            if (randomSite.Territory != null)
            {
                Debug.LogError("Receiving invalid node.");
            }
            var tribesInDraft = new List<HumanoidTribe>();

            //Add all tribes to the current bid who aren't opposed to the biome
            for (int i = 0; i < tribes.Length; i++)
            {
                if (!tribes[i].opposedBiomes.Contains(randomSite.biome))
                {
                    tribesInDraft.Add(tribes[i]);
                }
            }
            float[] tribeBids = new float[tribesInDraft.Count];

            //Set initial bids and modify based on terrain features and favored biomes
            for (int i = 0; i < tribesInDraft.Count; i++)
            {
                tribeBids[i] = 1;
                if (randomSite.Island != null) tribeBids[i] += tribesInDraft[i].IslandPreference;
                if (randomSite.Mountain != null) tribeBids[i] += tribesInDraft[i].MountainPreference;
                if (tribesInDraft[i].preferredBiomes.Contains(randomSite.biome)) tribeBids[i] += 1;
            }

            //Bid is divided by total number of nodes owned by that tribe
            for (int i = 0; i < tribeBids.Length; i++)
            {
                int territory = tribeTerritories[tribesInDraft[i]].Count;
                if (territory == 0) territory = 1; //Don't divide by zero
                tribeBids[i] /= tribeTerritories[tribesInDraft[i]].Count;
            }

            //Select the tribe with the highest bid
            HumanoidTribe winningTribe = tribesInDraft[0];
            float highestBid = 0;
            for (int i = 0; i < tribeBids.Length; i++)
            {
                if (tribeBids[i] > highestBid)
                {
                    winningTribe = tribesInDraft[i];
                    highestBid = tribeBids[i];
                }
            }

            //Place a hamelet on that site
            var newSettlement = PlaceSettlement(randomSite, hamlet, winningTribe);
            return newSettlement;
            //expand territory based on tribe's preferences

            //repeat
        }

        private void ExpandTerritory(Settlement settlement)
        {
            int[,] mapFlags = new int[worldMap.Width, worldMap.Height];
            Queue<TerrainNode> queue = new Queue<TerrainNode>();
            queue.Enqueue(settlement.Node);

            //So this is going to repeat the same territory every single time, which seems a waste
            while(queue.Count > 0)// && territoryToAdd > 0)
            {
                //Limit settlements from getting too large at the start
                if (settlement.territory.Count > mapFeatures.MaxStartingSettlementSize) break;

                var node = queue.Dequeue();
                if (!settlement.territory.Contains(node))
                {
                    AssignTerritory(settlement, node);
                    //territoryToAdd--;
                }

                for (int i = 0; i < node.neighbors.Length; i++)
                {
                    var neighbor = node.neighbors[i];
                    if (mapFlags[neighbor.x, neighbor.y] == 1) continue; //visited
                    
                    mapFlags[neighbor.x, neighbor.y] = 1;
                    if (!neighbor.isNotWater) continue; //is water
                    if (settlement.isSubterranean != (neighbor.Mountain != null)) continue; //subterranean can't extend outside mountain, others can't extend into mountains
                    if (settlement.tribe.opposedBiomes.Contains(neighbor.biome)) continue; //can't extend into opposed biomes
                    if (neighbor.Territory != null && neighbor.Territory != settlement) continue; //already claimed

                    //The distance in node path count from the settlement to the new territory
                    if (worldMap.GetPathCount(settlement.Node, neighbor, settlement) > 50 + Random.Range(-10, 5)) continue;

                    queue.Enqueue(neighbor);
                }
            }
        }

        /// <summary>
        /// Places a new settlement of the given type and tribe at the node and assigns territory and area of influence
        /// </summary>
        private Settlement PlaceSettlement(TerrainNode node, SettlementType type, HumanoidTribe tribe)
        {
            int population = worldGenerator.rng.Next(type.minPopulation, type.maxPopulation);
            var newSettlement = new Settlement("", settlements.Count, node, type, tribe, population);

            availableNodes.Remove(node);
            settlements.Add(newSettlement);
            tribeTerritories[tribe].Add(node);

            return newSettlement;
        }
        #endregion

        private void AssignTerritory(Settlement settlement, TerrainNode territory)
        {
            settlement.AddTerritory(territory);
            availableNodes.Remove(territory);
            if (!tribeTerritories[settlement.tribe].Contains(territory))
                tribeTerritories[settlement.tribe].Add(territory);
        }

        /// <summary>
        /// Recenters settlements closer to the center of their territory
        /// </summary>
        private void CenterSettlements()
        {
            foreach(var settlement in settlements)
            {
                float avgX = 0, avgY = 0;
                for (int i = 0; i < settlement.territory.Count; i++)
                {
                    avgX += settlement.territory[i].x;
                    avgY += settlement.territory[i].y;
                }
                int x = Mathf.RoundToInt(avgX / settlement.territory.Count);
                int y = Mathf.RoundToInt(avgY / settlement.territory.Count);

                var newNode = worldMap.GetNode(x, y);
                if (settlement.territory.Contains(newNode)) settlement.Relocate(newNode);
                else
                {
                    float dist = int.MaxValue;
                    TerrainNode nodeToMove = null;
                    for (int i = 0; i < settlement.territory.Count; i++)
                    {
                        var newDist = worldMap.GetNodeDistance_Straight(settlement.territory[i], newNode);
                        if (newDist < dist)
                        {
                            nodeToMove = settlement.territory[i];
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
                if (settlement.territory.Count > 4) continue;

                //Find the nearest settlement
                var settlementToMerge = FindNearestSettlement(settlement);

                //Transfers all territory to the chosen settlement and removes it from the tribe
                for (int j = 0; j < settlement.territory.Count; j++)
                {
                    tribeTerritories[settlement.tribe].Remove(settlement.territory[j]);
                    AssignTerritory(settlementToMerge, settlement.territory[j]);
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
                var newDist = worldMap.GetNodeDistance_Straight(fromSettlement.Node, settlements[i].Node);
                if (newDist < dist)
                {
                    toSettlement = settlements[i];
                    dist = newDist;
                }
            }
            return toSettlement;
        }

        private void SetSettlementSizes()
        {
            for (int i = 0; i < settlements.Count; i++)
            {
                SettlementType type;

                //Hamlet by default
                if (settlements[i].territory.Count < 25) type = hamlet;
                else if (settlements[i].territory.Count <= 50) type = village;
                else if (settlements[i].territory.Count <= 75) type = town;
                else if (settlements[i].territory.Count <= 150) type = city;
                else type = city; //Capital?

                int population = worldGenerator.rng.Next(type.minPopulation, type.maxPopulation);
                settlements[i].AdjustSize(type, population);
            }
        }

        private void GetSettlementConnections()
        {
            foreach (Settlement settlement in settlements)
            {
                for (int i = 0; i < settlement.areaOfInfluence.Count; i++)
                {
                    if (settlement.areaOfInfluence[i].Settlement != null)
                    {
                        var newSettlement = settlement.areaOfInfluence[i].Settlement;
                        if (newSettlement == settlement) continue;

                        var disposition = tribeRelations.GetDisposition(settlement.tribe, newSettlement.tribe);

                        settlement.AddNewRelation(newSettlement, disposition);
                        newSettlement.AddNewRelation(settlement, disposition);
                        Debug.Log(settlement.tribe + " " + settlement.type + " at " + settlement.Node.x + "," + settlement.Node.y +
                            " has encountered a " + newSettlement.tribe + " " + newSettlement.type + " at " +
                            newSettlement.Node.x + "," + newSettlement.Node.y + ". They have a disposition of " + disposition);
                    }
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
                    if (settlements[j].tribe == tribes[i])
                    {
                        count++;
                        if (settlements[j].type == hamlet) hamlets++;
                        if (settlements[j].type == village) villages++;
                        if (settlements[j].type == town) towns++;
                        if (settlements[j].type == city) cities++;
                    }
                }
                Debug.Log(tribes[i].ToString() + ": " + count + " settlements, " + tribeTerritories[tribes[i]].Count + " nodes, " +
                    "hamlets: " + hamlets + ", villages: " + villages + ", towns: " + towns + ", cities: " + cities);

            }
        }

        #region - Obsolete -
        [System.Obsolete]
        public void GenerateSettlements()
        {
            /*GetSiteList();

            GenerateCities();
            GenerateTowns();
            GenerateVillages();
            GenerateHamlets();
            StartCoroutine(ClaimTerritory());

            GetSettlementConnections();*/
        }

        [System.Obsolete]
        public IEnumerator ClaimTerritory()
        {
            //GetSiteList();

            //TerritoryDraft();


            GetSiteList();

            GenerateCities();

            var territoriesToExpand = new List<Settlement>();
            territoriesToExpand.AddRange(settlements);

            while (territoriesToExpand.Count > 0)
            {
                for (int i = territoriesToExpand.Count - 1; i >= 0; i--)
                {
                    if (!TryExpandTerritory(territoriesToExpand[i]))
                        territoriesToExpand.RemoveAt(i);
                }

                yield return null;
            }

            //GenerateTowns();

            //Generate Villages();

            //Generate Hamlets();

            //GetSettlementConnections();
        }

        private void GenerateCities()
        {
            for (int round = 0; round < mapFeatures.CityCount(worldSize); round++)
            {
                for (int ancestry = 0; ancestry < tribes.Length; ancestry++)
                {
                    FindSettlementSite(city, tribes[ancestry]);
                }
            }
        }
        
        private void GenerateTowns()
        {
            for (int round = 0; round < mapFeatures.TownCount(worldSize); round++)
            {
                for (int ancestry = 0; ancestry < tribes.Length; ancestry++)
                {
                    FindSettlementSite(town, tribes[ancestry]);
                }
            }
        }

        private void GenerateVillages()
        {
            for (int round = 0; round < mapFeatures.VillageCount(worldSize); round++)
            {
                for (int ancestry = 0; ancestry < tribes.Length; ancestry++)
                {
                    FindSettlementSite(village, tribes[ancestry]);
                }
            }
        }

        private void GenerateHamlets()
        {
            for (int round = 0; round < mapFeatures.TribeCount(worldSize); round++)
            {
                for (int ancestry = 0; ancestry < tribes.Length; ancestry++)
                {
                    FindSettlementSite(hamlet, tribes[ancestry]);
                }
            }
        }

        private void FindSettlementSite(SettlementType type, HumanoidTribe tribe)
        {
            var chance = worldGenerator.rng.Next(0, 100) * 0.01f;
            if (chance < tribe.MountainPreference)
            {
                if (FoundMountainHome(type, tribe)) return;
            }

            if (chance < tribe.MountainPreference + tribe.IslandPreference)
            {
                if (FoundIslandHome(type, tribe)) return;
            }

            //Try to find an available biome that is preferred
            if (FoundPreferredBiomeHome(type, tribe)) return;

            availableNodes = ShuffleList(availableNodes);
            //Unable to fit any previous preference, check all nodes but exclude opposed biomes
            for (int i = availableNodes.Count - 1; i >= 0; i--)
            {
                if (availableNodes[i].Settlement != null)
                {
                    availableNodes.RemoveAt(i);
                    continue;
                }
                if (tribe.opposedBiomes.Contains(availableNodes[i].biome)) continue;
                if (TerritoriesOverlap(availableNodes[i], type.territorySize)) continue;
                //Debug.Log("Found Preferred Home.");
                PlaceSettlement(availableNodes[i], type, tribe);
                return;
            }

            availableNodes = ShuffleList(availableNodes);
            //Unable to fit ANY preference, just find what is available
            for (int i = availableNodes.Count - 1; i >= 0; i--)
            {
                if (availableNodes[i].Settlement != null)
                {
                    availableNodes.RemoveAt(i);
                    continue;
                }
                if (TerritoriesOverlap(availableNodes[i], type.territorySize)) continue;
                //Debug.Log("Found A Home.");
                PlaceSettlement(availableNodes[i], type, tribe);
                return;
            }

            Debug.LogWarning("Unable to find any available spots for a settlement. Need to adjust parameters.");
        }

        private bool FoundMountainHome(SettlementType type, HumanoidTribe tribe)
        {
            foreach (MountainRange mountain in terrainData.Mountains)
            {
                for (int i = 0; i < mountain.Nodes.Count; i++)
                {
                    //if (mountain.Nodes[i].Settlement != null) continue;
                    //Don't need to check biome because a mountain home would be underground
                    if (TerritoriesOverlap(mountain.Nodes[i], type.territorySize)) continue;

                    //Found unclaimed mountain node
                    //Debug.Log("Found Mountain Home.");
                    PlaceSettlement(mountain.Nodes[i], type, tribe);
                    return true;
                }
            }
            return false;
        }

        private bool FoundIslandHome(SettlementType type, HumanoidTribe tribe)
        {
            foreach (Island island in terrainData.Islands)
            {
                for (int i = 0; i < island.Nodes.Count; i++)
                {
                    //if (island.Nodes[i].Settlement != null) continue;
                    if (tribe.opposedBiomes.Contains(island.Nodes[i].biome)) continue;
                    if (TerritoriesOverlap(island.Nodes[i], type.territorySize)) continue;

                    //Found unclaimed island node
                    //Debug.Log("Found Island Home.");
                    PlaceSettlement(island.Nodes[i], type, tribe);
                    return true;
                }
            }
            return false;
        }

        private bool FoundPreferredBiomeHome(SettlementType type, HumanoidTribe tribe)
        {
            var biomeList = new List<Biome>(tribe.preferredBiomes);
            biomeList = ShuffleList(biomeList);

            for (int i = 0; i < biomeList.Count; i++)
            {
                if (FoundPreferredBiomeHome(biomeList[i], type, tribe)) return true;
            }

            return false;
        }

        private bool FoundPreferredBiomeHome(Biome biome, SettlementType type, HumanoidTribe tribe)
        {
            var siteList = new List<TerrainNode>();
            foreach (BiomeGroup group in terrainData.Biomes)
            {
                if (group.biome != biome) continue;
                for (int i = 0; i < group.Nodes.Count; i++)
                {
                    //if (group.Nodes[i].Settlement != null) continue;
                    if (TerritoriesOverlap(group.Nodes[i], type.territorySize)) continue;

                    siteList.Add(group.Nodes[i]);
                }
            }
            if (siteList.Count == 0) return false;
            siteList = ShuffleList(siteList);

            int index = worldGenerator.rng.Next(0, siteList.Count);
            PlaceSettlement(siteList[index], type, tribe);
            return true;
        }

        private bool TryExpandTerritory(Settlement settlement)
        {
            var territoryToAdd = new List<TerrainNode>();
            for (int i = 0; i < settlement.territory.Count; i++)
            {
                for (int j = 0; j < settlement.territory[i].neighbors.Length; j++)
                {
                    var node = settlement.territory[i].neighbors[j];
                    if (!node.isNotWater) continue;
                    if (node.Settlement != null) continue;
                    if (node.Territory != null) continue;
                    if (territoryToAdd.Contains(node)) continue;
                    territoryToAdd.Add(node);
                }
            }
            if (territoryToAdd.Count == 0)
            {
                Debug.Log("Settlement located at " + settlement.Node.x + "," + settlement.Node.y + " cannot expand");
                return false;
            }

            for (int i = 0; i < territoryToAdd.Count; i++)
            {
                settlement.AddTerritory(territoryToAdd[i]);
            }
            Debug.Log("Settlement located at " + settlement.Node.x + "," + settlement.Node.y + " can expand");

            return true;
        }

        /// <summary>
        /// Determines if the territory for a given settlement would overlap with territory of an existing settlement
        /// </summary>
        private bool TerritoriesOverlap(TerrainNode proposedSite, int range)
        {
            foreach (Settlement settlement in settlements)
            {
                //Returns false if the territory range of the proposed settlement site would overlap with territory of another settlement
                if (Mathf.Abs(settlement.Node.x - proposedSite.x) <= settlement.type.territorySize + range)
                {
                    //Debug.Log(range + " + " + settlement.settlementType + " range is " + (range + settlement.settlementType.territorySize));
                    return true;
                }
                if (Mathf.Abs(settlement.Node.y - proposedSite.y) <= settlement.type.territorySize + range)
                {
                    //Debug.Log(range + " + " + settlement.settlementType + " range is " + (range + settlement.settlementType.territorySize));
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}