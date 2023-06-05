using System.Collections.Generic;

namespace JS.WorldMap
{
    [System.Serializable]
    public class BiomeGroup
    {
        public int ID;
        public int BiomeID;
        public List<WorldTile> Nodes { get; private set; }

        public BiomeGroup(int biomeID)
        {
            BiomeID = biomeID;
            Nodes = new List<WorldTile>();
        }
    }
}