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
                    else
                    {
                        node.Mountain.DeconstructRange();
                        //UnityEngine.Debug.Log("Mountain Removed.");
                    }
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
                    if (node.Lake != null && !lakes.Contains(node.Lake))
                    {
                        if (node.Lake.IsLandLocked(mapSize))
                        {
                            node.Lake.FinalizeValues(lakes.Count);
                            lakes.Add(node.Lake);
                            //Debug.Log("New Lake Identified! " + node.Lake.ID);
                        }
                        else node.Lake.DeconstructLake();
                    }
                }
            }

            return lakes;
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
                    if (node.Island != null && !islands.Contains(node.Island))
                    {
                        if (node.Island.Nodes.Count > mapSize)
                        {
                            //Debug.Log("Island is too large! Deconstructing. " + node.Island.Nodes.Count);
                            node.Island.DeconstrucIsland();
                            continue;
                        }

                        node.Island.FinalizeValues(islands.Count);
                        islands.Add(node.Island);
                        //Debug.Log("New Island Identified! " + node.Island.ID);
                    }
                }
            }

            return islands;
        }
    }
}