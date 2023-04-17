using System.Collections.Generic;

namespace JS.WorldGeneration
{
    [System.Serializable]
    public class Island
    {
        public int ID { get; private set; }
        private List<TerrainNode> nodes;
        public List<TerrainNode> Nodes => nodes;

        public Island(int ID)
        {
            nodes = new List<TerrainNode>();
            this.ID = ID;
        }

        public void Add(TerrainNode node)
        {
            if (!nodes.Contains(node))
            {
                nodes.Add(node);
            }
            node.Island = this;
        }

        public void AddRange(List<TerrainNode> newNodes)
        {
            for (int i = 0; i < newNodes.Count; i++)
            {
                Add(newNodes[i]);
            }
        }
    }
}