using System.Collections.Generic;

namespace JS.WorldGeneration
{
    public static class MountainFinder
    {
        public static List<MountainRange> FindMountainRanges(int size, int minRangeSize = 4, float minMountainHeight = 0.7f)
        {
            //UnityEngine.Debug.Log("Finding Mountain Ranges. " + UnityEngine.Time.realtimeSinceStartup);

            var worldMap = WorldMap.instance;

            var ranges = new List<MountainRange>();

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
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
    }
}