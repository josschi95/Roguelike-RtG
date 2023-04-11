using System.Collections.Generic;

namespace JS.WorldGeneration
{
    [System.Serializable]
    public class Island
    {
        public int ID { get; private set; }
        public List<TerrainNode> Nodes;

        public Island()
        {
            Nodes = new List<TerrainNode>();
        }

        public void MergeIslands(Island otherIsland)
        {
            otherIsland.Nodes.AddRange(Nodes);
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].Island = otherIsland;
            }
            Nodes.Clear();
        }

        public void FinalizeValues(int ID)
        {
            this.ID = ID;
        }

        public void DeconstrucIsland()
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].Island = null;
            }
            Nodes.Clear();
        }
    }
}