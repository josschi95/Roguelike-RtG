namespace JS.WorldMap
{
    [System.Serializable]
    public class Lake
    {
        public int ID;
        public GridCoordinates[] GridNodes;
        //Could also add some category for size: puddle vs lake vs. whatever

        public Lake(int ID)
        {
            this.ID = ID;
        }
    }
}