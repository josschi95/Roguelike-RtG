namespace JS.World.Map.Features
{
    [System.Serializable]
    public class LandMass
    {
        public int ID { get; private set; }
        public GridCoordinates[] GridNodes;
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