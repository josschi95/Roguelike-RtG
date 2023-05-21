using JS.WorldMap;

public class WorldData
{
    public string saveFileName; //The default name for the file
    public string saveFileNameOverride;  //If the player chooses to name a save file directly. We'll see if I implement this

    //public MapData mapData; //Saves the state of the world map, height, heat, moisture, mountains, rivers, etc.

    public int mapWidth, mapHeight;
    public int originX, originY;
    public float[,] heightMap;
    public float[,] heatMap;
    public float[,] moistureMap;
    public float[,] airPressureMap;
    public SecondaryDirections[,] windMap;

    public int[,] biomeMap; //reference to the ID of each biome

    public MountainRange[] Mountains;
    public Lake[] Lakes;
    public River[] Rivers;
    public BiomeGroup[] Biomes;
    public Island[] Islands;
}