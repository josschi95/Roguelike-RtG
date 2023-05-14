using JS.WorldGeneration;

public class MapData
{
    public int mapSize;

    //Will also need to store all values used for PerlinNoise

    public float[,] heightMap;
    public float[,] heatMap;
    public float[,] moistureMap;

    public float[,] airPressureMap;
    public SecondaryDirections[,] windMap;

    public MountainRange[] mountainRanges;
    public River[] rivers;

    public MapData(int size, float[,] heightMap, float[,] heatMap, float[,] moistureMap, float[,] pressureMap, SecondaryDirections[,] windMap, MountainRange[] ranges, River[] rivers)
    {
        mapSize = size;

        this.heightMap = heightMap;
        this.heightMap = heatMap;
        this.moistureMap = moistureMap;
        airPressureMap = pressureMap;
        this.windMap = windMap;

        mountainRanges = new MountainRange[ranges.Length];
        ranges.CopyTo(mountainRanges, 0);

        this.rivers = new River[rivers.Length];
        rivers.CopyTo(this.rivers, 0);
    }
}