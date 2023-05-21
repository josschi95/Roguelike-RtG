using System.Collections.Generic;

namespace JS.WorldMap
{
    [System.Serializable]
    public class Lake
    {
        public int ID { get; private set; }
        private List<WorldTile> nodes;
        public List<WorldTile> Nodes => nodes;

        public Lake()
        {
            nodes = new List<WorldTile>();
        }

        public void Add(WorldTile node)
        {
            if (!nodes.Contains(node))
            {
                nodes.Add(node);
            }
            node.Lake = this;
        }

        public void AddRange(List<WorldTile> newNodes)
        {
            for (int i = 0; i < newNodes.Count; i++)
            {
                Add(newNodes[i]);
            }
        }

        public bool IsLandLocked(int mapSize)
        {
            if (nodes.Count >= 1000)
            {
                //UnityEngine.Debug.Log("Lake size is greater than 1,000. " + Nodes.Count);
                //return false;
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].x == 0 || nodes[i].x == mapSize - 1) return false;
                if (nodes[i].y == 0 || nodes[i].y == mapSize - 1) return false;
            }
            return true;
        }

        public void FinalizeValues(int ID)
        {
            this.ID = ID;
        }

        /// <summary>
        /// Clears Lake nodes to be picked up by GC
        /// </summary>
        public void DeconstructLake()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].Lake == this) nodes[i].Lake = null;
            }
            nodes.Clear();
        }
    }
}