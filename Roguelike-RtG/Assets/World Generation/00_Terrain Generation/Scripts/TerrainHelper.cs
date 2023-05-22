using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JS.WorldMap.Generation
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
    }
}