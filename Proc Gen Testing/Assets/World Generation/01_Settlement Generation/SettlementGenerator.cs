using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace JS.WorldGeneration
{
    public class SettlementGenerator : MonoBehaviour
    {
        [SerializeField] private WorldGenerator worldGenerator;
        [SerializeField] private WorldGenerationParameters mapFeatures;
        [SerializeField] private TerrainData terrainData;
        private WorldSize worldSize;

        [Space]

        [SerializeField] private HumanoidTribe[] ancestries;
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
            Debug.Log("Beginning Settlement Generation");

            settlements = new List<Settlement>();

            GetSiteList();

            GenerateCities();
            GenerateTowns();
            GenerateVillages();
            GenerateHamlets();

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
                    TerrainNode node = WorldMap.instance.GetNode(x, y);
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
                for (int ancestry = 0; ancestry < ancestries.Length; ancestry++)
                {
                    FindSettlementSite(city, ancestries[ancestry]);
                }
            }
        }

        private void GenerateTowns()
        {
            for (int round = 0; round < mapFeatures.TownCount(worldSize); round++)
            {
                for (int ancestry = 0; ancestry < ancestries.Length; ancestry++)
                {
                    FindSettlementSite(town, ancestries[ancestry]);
                }
            }
        }

        private void GenerateVillages()
        {
            for (int round = 0; round < mapFeatures.VillageCount(worldSize); round++)
            {
                for (int ancestry = 0; ancestry < ancestries.Length; ancestry++)
                {
                    FindSettlementSite(village, ancestries[ancestry]);
                }
            }
        }

        private void GenerateHamlets()
        {
            for (int round = 0; round < mapFeatures.TribeCount(worldSize); round++)
            {
                for (int ancestry = 0; ancestry < ancestries.Length; ancestry++)
                {
                    FindSettlementSite(hamlet, ancestries[ancestry]);
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
                    if (mountain.Nodes[i].Settlement != null) continue;
                    //Don't need to check biome because a mountain home would be underground

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
                    if (island.Nodes[i].Settlement != null) continue;
                    if (tribe.opposedBiomes.Contains(island.Nodes[i].biome)) continue;

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
                    if (group.Nodes[i].Settlement != null) continue;
                    siteList.Add(group.Nodes[i]);
                }
            }
            if (siteList.Count == 0) return false;
            siteList = ShuffleList(siteList);

            int index = worldGenerator.rng.Next(0, siteList.Count);
            PlaceSettlement(siteList[index], type, tribe);
            return true;
        }

        private void PlaceSettlement(TerrainNode node, SettlementType type, HumanoidTribe tribe)
        {
            int population = worldGenerator.rng.Next(type.minPopulation, type.maxPopulation);
            var newSettlement = new Settlement(settlements.Count, node.x, node.y,
                type, tribe, population);

            node.Settlement = newSettlement;
            availableNodes.Remove(node);
            settlements.Add(newSettlement);

            var territory = WorldMap.instance.GetNodesInRange_Square(node, type.territorySize);
            for (int i = 0; i < territory.Count; i++)
            {
                territory[i].Settlement = newSettlement;
            }

            //Debug.Log(type.name + " settlement placed at " + node.x + "," + node.y + " belonging to " + tribe.name);
        }
        #endregion

        private void GetSettlementConnections()
        {
            for (int i = 0; i < settlements.Count; i++)
            {

            }
        }
    }
}