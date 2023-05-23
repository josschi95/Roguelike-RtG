namespace JS.WorldMap
{
	[System.Serializable]
	public class River
	{
		public int ID;
		public int Length;

		public GridCoordinates[] Coordinates;
		public Compass[] Flow;

		public int Intersections;
		public int TurnCount;
		public Direction CurrentDirection;

		public bool hasBeenDug { get; set; }
	}
}