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

        public void SetInitialValues(WorldSize size)
        {
            worldSize = size;
        }

        public void GenerateSettlements()
        {
            settlements = new List<Settlement>();

            GetSiteList();

            GenerateCities();
            GenerateTowns();
            GenerateVillages();
            GenerateHamlets();
            StartCoroutine(ClaimTerritory());

            GetSettlementConnections();

            //pass this data to the mapData 
        }

        #region - Settlement Placement -
        private void GetSiteList()
        {
            availableNodes = new List<TerrainNode>();
            for (int x = 0; x < mapFeatures.MapSize(worldSize); x++)
            {
                for (int y = 0; y < mapFeatures.MapSize(worldSize); y++)
                {
                    //TerrainNode node = WorldMap.instance.GetNode(x, y);
                    TerrainNode node = worldMap.GetNode(x, y);
                    if (!node.isNotWater) continue; //Don't include water
                    if (node.Settlement != null) continue; //this shouldn't happen

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

        public IEnumerator ClaimTerritory()
        {
            settlements = new List<Settlement>();

            GetSiteList();

            GenerateCities();

            var territoriesToExpand = new List<Settlement>();
            territoriesToExpand.AddRange(settlements);

            while(territoriesToExpand.Count > 0)
            {
                for (int i = territoriesToExpand.Count - 1; i >= 0; i--)
                {
                    if (!TryExpandTerritory2(territoriesToExpand[i]))
                        territoriesToExpand.RemoveAt(i);
                }

                yield return null;
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
            foreach(MountainRange mountain in terrainData.Mountains)
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
            foreach(BiomeGroup group in terrainData.Biomes)
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

        /// <summary>
        /// Places a new settlement of the given type and tribe at the node and assigns territory and area of influence
        /// </summary>
        private void PlaceSettlement(TerrainNode node, SettlementType type, HumanoidTribe tribe)
        {
            int population = worldGenerator.rng.Next(type.minPopulation, type.maxPopulation);
            var newSettlement = new Settlement("", settlements.Count, node, type, tribe, population);

            availableNodes.Remove(node);
            settlements.Add(newSettlement);

            //var territory = WorldMap.instance.GetNodesInRange_Square(node, type.territorySize);
            var territory = worldMap.GetNodesInRange_Square(node, type.territorySize);
            for (int i = 0; i < territory.Count; i++)
            {
                newSettlement.AddTerritory(territory[i]);
            }
            var areaOfInfluence = worldMap.GetNodesInRange_Circle(node, type.AreaOfInfluence());
            for (int i = 0; i < areaOfInfluence.Count; i++)
            {
                newSettlement.AddAreaOfInfluence(areaOfInfluence[i]);
            }
            //Debug.Log(type.name + " settlement placed at " + node.x + "," + node.y + " belonging to " + tribe.name);
        }
        #endregion

        private void GetSettlementConnections()
        {
            foreach(Settlement settlement in settlements)
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

        private bool TryExpandTerritory2(Settlement settlement)
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
                //Debug.Log("Settlement located at " + settlement.Node.x + "," + settlement.Node.y + " cannot expand");
                return false;
            }

            for (int i = 0; i < territoryToAdd.Count; i++)
            {
                settlement.AddTerritory(territoryToAdd[i]);
            }
            //Debug.Log("Settlement located at " + settlement.Node.x + "," + settlement.Node.y + " can expand");

            return true;
        }
    }
}