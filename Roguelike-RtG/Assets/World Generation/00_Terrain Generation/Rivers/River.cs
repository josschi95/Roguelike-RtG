using System.Collections.Generic;
using JS.WorldMap.Generation;

namespace JS.WorldMap
{
	[System.Serializable]
	public class River
	{
		public int Length;
		public List<WorldTile> Nodes { get; private set; }
		public List<Compass> Flow { get; private set; }
		public int ID { get; private set; }

		public int Intersections;
		public int TurnCount;
		public Direction CurrentDirection;

		public bool hasBeenDug;

		public River()
		{
			Nodes = new List<WorldTile>();
			Flow = new List<Compass>();
		}

		public void Register(int id)
		{
			ID = id;

			for (int i = 0; i < Nodes.Count; i++)
			{
				Nodes[i].SetRiverPath(this);
			}
			DirectFlow();
		}

		public void AddNode(WorldTile tile)
        {
			Nodes.Add(tile);
        }

		private void DirectFlow()
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
				if (i == 0) //Direction is only dependent on following node
                {
					Flow.Add(ConvertDirection(Nodes[i].NeighborDirection_Adjacent(Nodes[i + 1])));
                }
				else if (i == Nodes.Count - 1) //Direction is only dependent on previous node
                {
					Flow.Add(ConvertDirection(Nodes[i].NeighborDirection_Adjacent(Nodes[i - 1])));
				}
                else
                {
					var from = Nodes[i].NeighborDirection_Adjacent(Nodes[i - 1]);
					var to = Nodes[i].NeighborDirection_Adjacent(Nodes[i + 1]);
					Flow.Add(CombineDirections(from, to));
				}
			}
        }

		private Compass ConvertDirection(Direction direction)
        {
            switch (direction)
            {
				case Direction.North: return Compass.North;
				case Direction.South: return Compass.South;
				case Direction.East: return Compass.East;
				case Direction.West: return Compass.West;
				default: throw new System.Exception("Direction outside parameters");
            }
        }

		private Compass CombineDirections(Direction fromDirection, Direction toDirection)
        {
            switch (fromDirection)
            {
				case Direction.North:
					switch (toDirection)
					{
						case Direction.East: return Compass.NorthEast;
						case Direction.West: return Compass.NorthWest;
						default: return Compass.North;
					}
				case Direction.South:
					switch (toDirection)
					{
						case Direction.East: return Compass.SouthEast;
						case Direction.West: return Compass.SouthWest;
						default: return Compass.North;
					}
				case Direction.East:
					switch (toDirection)
					{
						case Direction.North: return Compass.NorthEast;
						case Direction.South: return Compass.SouthEast;
						default: return Compass.East;
					}
				case Direction.West:
					switch (toDirection)
					{
						case Direction.North: return Compass.NorthWest;
						case Direction.South: return Compass.SouthWest;
						default: return Compass.East;
					}
			}
			throw new System.Exception("Compass Direction outside parameters");
		}
	}
}