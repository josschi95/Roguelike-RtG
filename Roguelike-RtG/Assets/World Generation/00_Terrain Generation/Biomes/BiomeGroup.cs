using System.Collections.Generic;

namespace JS.WorldMap
{
    [System.Serializable]
    public class BiomeGroup
    {
        public int ID;
        public int BiomeID;
        public Biome biome;
        public List<WorldTile> Nodes;

        public BiomeGroup(Biome biome)
        {
            Nodes = new List<WorldTile>();
            this.biome = biome;
            BiomeID = biome.ID;
        }

        public void MergeBiomes(BiomeGroup otherBiome)
        {
            otherBiome.Nodes.AddRange(Nodes);
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].BiomeGroup = otherBiome;
            }
            Nodes.Clear();
        }
    }
}