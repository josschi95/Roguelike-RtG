namespace JS.WorldMap
{
    public class WorldData
    {
        public int mapWidth, mapHeight;
        public int originX, originY;
        public float[] heightMap;
        public float[] heatMap;
        public float[] moistureMap;
        //public float[,] airPressureMap;
        public SecondaryDirections[] windMap;

        public int[] biomeMap; //reference to the ID of each biome
        public BiomeGroup[] BiomeGroups;

        public MountainRange[] Mountains;
        public Lake[] Lakes;
        public River[] Rivers;
        public Island[] Islands;
    }
}