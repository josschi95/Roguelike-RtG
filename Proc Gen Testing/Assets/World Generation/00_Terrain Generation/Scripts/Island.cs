using System.Collections.Generic;

namespace JS.WorldGeneration
{
    [System.Serializable]
    public class Island
    {
        public int ID { get; private set; }
        private List<TerrainNode> nodes;
        public List<TerrainNode> Nodes => nodes;
        private List<TerrainNode> openList;

        public Island()
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
            node.Island = this;
        }

        public void AddRange(List<TerrainNode> newNodes)
        {
            for (int i = 0; i < newNodes.Count; i++)
            {
                Add(newNodes[i]);
            }
        }

        public void MergeIslands(Island otherIsland)
        {
            if (otherIsland == this) return;
            otherIsland.AddRange(nodes);
            nodes.Clear();
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
                    if (!node.isNotWater) continue;
                    if (node.Island != null) continue;
                    nodesToAdd.Add(node);
                }
            }
            openList.Clear();

            if (nodesToAdd.Count == 0) return false;

            AddRange(nodesToAdd);

            return true;
        }

        public void FinalizeValues(int ID)
        {
            this.ID = ID;
        }

        public void DeconstrucIsland()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Island = null;
            }
            nodes.Clear();
        }
    }
}