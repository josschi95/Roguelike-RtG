using System.Collections.Generic;

namespace JS.World.Map.Features
{
    public static class TerrainHelper
    {
        public static void MergeBiomes(BiomeGroup oldGroup, BiomeGroup newGroup)
        {
            newGroup.Nodes.AddRange(oldGroup.Nodes);
            for (int i = 0; i < oldGroup.Nodes.Count; i++)
            {
                oldGroup.Nodes[i].BiomeGroup = newGroup;
            }
            oldGroup.Nodes.Clear();
        }

        public static List<MountainRange> FindMountainRanges(int minRangeSize = 4, float minMountainHeight = 0.7f)
        {
            var ranges = new List<MountainRange>();

            for (int x = 0; x < WorldMap.Width; x++)
            {
                for (int y = 0; y < WorldMap.Height; y++)
                {
                    WorldTile node = WorldMap.GetNode(x, y);

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
    }
}