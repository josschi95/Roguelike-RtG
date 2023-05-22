using System.Collections.Generic;

namespace JS.WorldMap
{
    [System.Serializable]
    public class Island
    {
        public int ID { get; private set; }
        public List<WorldTile> Nodes { get; set; }
        public GridCoordinates[] GridNodes;

        public Island(int ID)
        {
            Nodes = new List<WorldTile>();
            this.ID = ID;
        }
    }
}