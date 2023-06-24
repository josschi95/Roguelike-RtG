namespace JS.WorldMap
{
    [System.Serializable]
    public class LandMass
    {
        public int ID { get; private set; }
        public GridCoordinates[] GridNodes { get; set; }
        public LandSize Size;

        public LandMass(int ID)
        {
            this.ID = ID;
        }
    }
}

public enum LandSize
{
    Islet,
    Island,
    Continent,
}