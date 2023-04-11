using System.Collections.Generic;

namespace JS.WorldGeneration
{
    [System.Serializable]
    public class Lake
    {
        public int ID { get; private set; }
        private List<TerrainNode> Nodes;
        private List<TerrainNode> edgeNodes;

        public Lake()
        {
            Nodes = new List<TerrainNode>();
            edgeNodes = new List<TerrainNode>();
        }

        public void Add(TerrainNode node)
        {
            if (!Nodes.Contains(node))
            {
                Nodes.Add(node);
                for (int i = 0; i < node.neighbors.Length; i++)
                {
                    if (node.neighbors[i].isNotWater)
                    {
                        edgeNodes.Add(node);
                        return;
                    }
                }
            }
        }

        public void AddRange(List<TerrainNode> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                Add(nodes[i]);
            }
        }

        public void MergeLakes(Lake otherLake)
        {
            otherLake.AddRange(Nodes);
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].Lake = otherLake;
            }
            Nodes.Clear();
        }

        public bool IsLandLocked(int mapSize)
        {
            if (Nodes.Count >= 1000)
            {
                //UnityEngine.Debug.Log("Lake size is greater than 1,000. " + Nodes.Count);
                return false;
            }
            return true;
            /*
            else if (Nodes.Count < 100)
            {
                UnityEngine.Debug.Log("Lake size is less than 100. " + Nodes.Count);
                //return true;
            }
            UnityEngine.Debug.Log(edgeNodes.Count);
            foreach (TerrainNode node in edgeNodes)
            {
                bool nodeConnects = false;
                for (int i = 0; i < node.neighbors.Length; i++)
                {
                    if (edgeNodes.Contains(node.neighbors[i]))
                    {
                        nodeConnects = true;
                    }
                }
                if (!nodeConnects) return false;
            }

            return true;*/
        }

        public void FinalizeValues(int ID)
        {
            this.ID = ID;
        }

        public void DeconstructLake()
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].Lake = null;
            }
            Nodes.Clear();
        }
    }
}