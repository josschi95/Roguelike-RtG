using System.Collections.Generic;

namespace JS.WorldGeneration
{
	[System.Serializable]
	public class River
	{
		public int Length;
		public List<WorldTile> Nodes;
		public int ID { get; private set; }

		public int Intersections;
		public float TurnCount;
		public Direction CurrentDirection;

		public bool hasBeenDug;

		public River()
		{
			Nodes = new List<WorldTile>();
		}

		public void Register(int id)
		{
			ID = id;
			for (int i = 0; i < Nodes.Count; i++)
			{
				Nodes[i].SetRiverPath(this);
			}
		}

		public void AddNode(WorldTile node)
		{
			Nodes.Add(node);
		}
	}
}