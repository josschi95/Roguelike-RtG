using System.Collections.Generic;

namespace JS.WorldMap
{
    [System.Serializable]
    public class Island
    {
        public int ID { get; private set; }
        private List<WorldTile> nodes;
        public List<WorldTile> Nodes => nodes;

        public Island(int ID)
        {
            nodes = new List<WorldTile>();
            this.ID = ID;
        }

        public void Add(WorldTile node)
        {
            if (!nodes.Contains(node))
            {
                nodes.Add(node);
            }
            node.Island = this;
        }

        public void AddRange(List<WorldTile> newNodes)
        {
            for (int i = 0; i < newNodes.Count; i++)
            {
                Add(newNodes[i]);
            }
        }
    }
}