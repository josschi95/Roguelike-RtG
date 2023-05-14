using System.Collections.Generic;

namespace JS.WorldGeneration
{
    [System.Serializable]
    public class BiomeGroup
    {
        public int ID { get; private set; }
        public Biome biome;
        public List<WorldTile> Nodes;

        public BiomeGroup(Biome biome)
        {
            Nodes = new List<WorldTile>();
            this.biome = biome;
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

        public void FinalizeValues(int ID)
        {
            this.ID = ID;
        }
    }
}