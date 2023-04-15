using System.Collections.Generic;

namespace JS.WorldGeneration
{
    [System.Serializable]
    public class Lake
    {
        public int ID { get; private set; }
        private List<TerrainNode> nodes;
        public List<TerrainNode> Nodes => nodes;

        private List<TerrainNode> openList;


        public Lake()
        {
            nodes = new List<TerrainNode>();
            openList = new List<TerrainNode>();
        }

        public void Add(TerrainNode node)
        {
            if (!nodes.Contains(node))
            {
                nodes.Add(node);
                openList.Add(node);
            }
            node.Lake = this;
        }

        public bool IsExpanding()
        {
            if (openList.Count == 0) return false;

            var nodesToAdd = new List<TerrainNode>();

            for (int i = openList.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < openList[i].neighbors.Length; j++)
                {
                    var node = openList[i].neighbors[j];
                    if (node.isNotWater) continue;
                    if (node.Lake != null) continue;
                    nodesToAdd.Add(node);
                }
            }
            openList.Clear();

            if (nodesToAdd.Count == 0) return false;

            AddRange(nodesToAdd);

            return true;
        }

        public void AddRange(List<TerrainNode> newNodes)
        {
            for (int i = 0; i < newNodes.Count; i++)
            {
                Add(newNodes[i]);
            }
        }

        public void MergeLakes(Lake otherLake)
        {
            if (otherLake == this) return;
            otherLake.AddRange(nodes);
            nodes.Clear();
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
                nodes[i].Lake = null;
            }
            nodes.Clear();
        }
    }
}