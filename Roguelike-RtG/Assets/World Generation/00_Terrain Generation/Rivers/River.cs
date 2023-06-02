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
		public int x, y;
		public Compass Flow;
		public int Size;

		public RiverNode(int x = 0, int y = 0)
		{
			this.x = x;
			this.y = y;
		}
	}
}