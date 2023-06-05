namespace JS.WorldMap
{
	[System.Serializable]
	public class River
	{
		public int ID;
		public int Length;

		public RiverNode[] Nodes;

		public int TurnCount;
		public Compass CurrentDirection;

		public bool hasBeenDug { get; set; }
	}

	[System.Serializable]
	public class RiverNode
	{
		public int x, y; //world location of node
		public int Size; //radius of river from center line
		public int Offset; //offset of river from local map center
		public Compass PathDirection; //Directional bend of the river
		public Compass Flow; //Direction the river is flowing in

		public RiverNode(int x = 0, int y = 0)
		{
			this.x = x;
			this.y = y;
		}
	}
}