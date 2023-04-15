using System.Collections.Generic;

namespace JS.WorldGeneration
{
    public static class Topography
    {
        public static List<MountainRange> FindMountainRanges(WorldMapData worldMap, int mapSize, int minRangeSize = 4, float minMountainHeight = 0.7f)
        {
            //UnityEngine.Debug.Log("Finding Mountain Ranges. " + UnityEngine.Time.realtimeSinceStartup);
            //var worldMap = WorldMap.instance;

            var ranges = new List<MountainRange>();

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    TerrainNode node = worldMap.GetNode(x, y);

                    //node is not part of a mountain
                    if (node.Mountain == null) continue;
                    //mountain range is already registered
                    if (ranges.Contains(node.Mountain)) continue;

                    if (node.Mountain.Nodes.Count >= minRangeSize)
                    {
                        node.Mountain.FinalizeValues(ranges.Count);
                        ranges.Add(node.Mountain);
                        //UnityEngine.Debug.Log("New Mountian Added.");
                    }
                    else node.Mountain.DeconstructRange();
                }
            }

            return ranges;
        }

        public static List<Lake> FindLakes(WorldMapData worldMap, int mapSize)
        {
            //var worldMap = WorldMap.instance;
            var lakes = new List<Lake>();

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    TerrainNode node = worldMap.GetNode(x, y);
                    if (node.Lake == null) continue;
                    if (lakes.Contains(node.Lake)) continue;

                    if (TryFindRegisteredLake(lakes, node, out Lake registeredLake))
                    {
                        node.Lake.MergeLakes(registeredLake);
                        continue;
                    }

                    if (node.Lake.IsLandLocked(mapSize))
                    {
                        node.Lake.FinalizeValues(lakes.Count);
                        lakes.Add(node.Lake);
                        //Debug.Log("New Lake Identified! " + node.Lake.ID);
                    }
                    else node.Lake.DeconstructLake();
                }
            }

            return lakes;
        }

        private static bool TryFindRegisteredLake(List<Lake> oldLakes, TerrainNode node, out Lake registerdLake)
        {
            registerdLake = null;
            for (int i = 0; i < oldLakes.Count; i++)
            {
                if (oldLakes[i].Nodes.Contains(node))
                {
                    registerdLake = oldLakes[i];
                    return true;
                }
            }
            return false;
        }

        public static List<Island> FindIslands(WorldMapData worldMap, int mapSize)
        {
            //var worldMap = WorldMap.instance;
            var islands = new List<Island>();

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    TerrainNode node = worldMap.GetNode(x, y);
                    if (node.Island == null) continue;
                    if (islands.Contains(node.Island)) continue;

                    if (TryFindRegisteredIsland(islands, node, out Island registerdIsland))
                    {
                        node.Island.MergeIslands(registerdIsland);
                        continue;
                    }

                    if (node.Island.Nodes.Count <= mapSize)
                    {
                        node.Island.FinalizeValues(islands.Count);
                        islands.Add(node.Island);
                    }
                    else node.Island.DeconstrucIsland();
                }
            }

            return islands;
        }

        private static bool TryFindRegisteredIsland(List<Island> islands, TerrainNode node, out Island registeredIsland)
        {
            registeredIsland = null;
            for (int i = 0; i < islands.Count; i++)
            {
                if (islands[i].Nodes.Contains(node))
                {
                    registeredIsland = islands[i];
                    return true;
                }
            }
            return false;
        }
    }
}