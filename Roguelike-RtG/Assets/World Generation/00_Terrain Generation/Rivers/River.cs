namespace JS.WorldMap
{
	[System.Serializable]
	public class River
	{
		public int ID;
		public int Length;

		public RiverNode[] Nodes;

		//public int Intersections;
		public int TurnCount;
		public Compass CurrentDirection;

		public bool hasBeenDug { get; set; }
	}

	[System.Serializable]
	public class RiverNode
	{
		public GridCoordinates Coordinates;
		public Compass Flow;
		public int Size;
	}
}