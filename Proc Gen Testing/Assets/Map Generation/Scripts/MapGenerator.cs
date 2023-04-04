using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    #region - Map Constants -
    //height and width are currently measured in Local Tiles = 0.01 km2 or 100x100 Unit Tiles
    private const float minRainShadowAltitude = 0.7f;

    private const int minuteMapSize = 200;
    private const int minuteRiverCount = 25;
    private const int minuteRainDrops = 40000;
    private const int minuteTectonicPlates = 4;

    private const int tinyMapSize = 400;
    private const int tinyRiverCount = 50;
    private const int tinyRainDrops = 80000;
    private const int tinyTectonicPlates = 6;

    private const int smallMapSize = 800;
    private const int smallRiverCount = 75;
    private const int smallRainDrops = 160000;
    private const int smallTectonicPlates = 8;

    private const int mediumMapSize = 1500;
    private const int mediumRiverCount = 100;
    private const int mediumRainDrops = 320000;
    private const int mediumTectonicPlates = 10;

    private const int largeMapSize = 3000;
    private const int largeRiverCount = 125;
    private const int largeRainDrops = 640000;
    private const int largeTectonicPlates = 12;
    #endregion

    public int mapSize { get; private set; }
    public Vector3Int origin { get; private set; }
    private float seaLevel = 0.4f;

    private WorldMap worldMap;
    private MapDisplay mapDisplay;
    private RiverGenerator riverGenerator;

    [SerializeField] private HeightGeneration heightGeneration;
    [SerializeField] private MapDimensions dimensions;
    [SerializeField] private bool randomSeed;
    [SerializeField] private bool useErosion;
    
    [Header("Perlin Noise")]
    [SerializeField] private float noiseScale;
    [Tooltip("The number of iterations of Perlin Noise over an area")]
    [SerializeField] private int octaves = 4;
    [Range(0, 1)]
    [Tooltip("Controls decrease in amplitude of subsequent octaves")]
    [SerializeField] private float persistence = 0.5f;
    [Tooltip("Controls increase in frequency of octaves")]
    [SerializeField] private float lacunarity = 2f;

    [Space]

    [SerializeField] private int seed;
    [SerializeField] private Vector2 offset;

    [Space]

    [SerializeField] private TerrainType waterLevel;
    [SerializeField] private TerrainType[] terrainTypes;
    [SerializeField] private TemperatureZone[] temperatureZones;
    [SerializeField] private PrecipitationZone[] precipitationZones;
    [SerializeField] private Biome[] biomes;

    #region - Biome Table -
    BiomeTypes[,] BiomeTable = new BiomeTypes[6, 6] {   
    //COLDEST                   //COLDER                    //COLD                          //HOT                                   //HOTTER                            //HOTTEST
    { BiomeTypes.Tundra,        BiomeTypes.Tundra,       BiomeTypes.TemperateGrassland,    BiomeTypes.Desert,                   BiomeTypes.Desert,                   BiomeTypes.Desert },              //DRYEST
    { BiomeTypes.Tundra,        BiomeTypes.Tundra,       BiomeTypes.TemperateGrassland,    BiomeTypes.TemperateGrassland,       BiomeTypes.Desert,                   BiomeTypes.Desert },              //DRYER
    { BiomeTypes.Tundra,        BiomeTypes.Tundra,       BiomeTypes.Woodland,              BiomeTypes.Woodland,                 BiomeTypes.Savanna,                  BiomeTypes.Savanna },             //DRY
    { BiomeTypes.Tundra,        BiomeTypes.BorealForest, BiomeTypes.DeciduousForest,       BiomeTypes.Woodland,                 BiomeTypes.TropicalSeasonalForest,   BiomeTypes.Savanna },             //WET
    { BiomeTypes.Tundra,        BiomeTypes.BorealForest, BiomeTypes.DeciduousForest,       BiomeTypes.DeciduousForest,          BiomeTypes.TropicalRainforest,       BiomeTypes.TropicalRainforest },  //WETTER
    { BiomeTypes.BorealForest,  BiomeTypes.BorealForest, BiomeTypes.DeciduousForest,       BiomeTypes.TropicalSeasonalForest,   BiomeTypes.TropicalRainforest,       BiomeTypes.TropicalRainforest }   //WETTEST
    };
    #endregion

    private System.Random rng;

    //May need to consider caching a 

    private void Start()
    {
        worldMap = WorldMap.instance;
        mapDisplay = GetComponent<MapDisplay>();
        riverGenerator = GetComponent<RiverGenerator>();

        GenerateMap();
    }

    public void GenerateMap()
    {
        seaLevel = waterLevel.Height;
        rng = new System.Random(seed);

        SetMapDimensions();
        origin = new Vector3Int(Mathf.FloorToInt(transform.position.x - mapSize / 2f), Mathf.FloorToInt(transform.position.y - mapSize / 2f));
        var go = new GameObject("Origin");
        go.transform.position = origin;

        worldMap.CreateGrid(mapSize, mapSize);

        if (randomSeed) SetRandomSeed();

        float[,] heightMap = GetHeightMap();

        //Debug.Log("Generating Heat Map. " + Time.realtimeSinceStartup);
        float[,] heatMap = TemperatureData.GenerateHeatMap(heightMap, seaLevel);
        //Debug.Log("Generating Moisture Map. " + Time.realtimeSinceStartup);
        float[,] moistureMap = DampedCosine.GetMoistureMap(heightMap, seaLevel);

        float[,] airPressureMap = AirPressureData.GetAirPressureMap(heightMap);
        SecondaryDirections[,] windMap = AirPressureData.GetWindMap(heightMap);

        SetTileValues(heightMap, heatMap, moistureMap, airPressureMap, windMap);

        var ranges = MountainFinder.FindMountainRanges(mapSize);

        if (NeedToRestart(ranges.ToArray())) Debug.LogWarning("Should probably restart.");

        var rivers = riverGenerator.GenerateRivers(mapSize, ranges, GetRiverCount());


        AdjustTileValues(moistureMap);

        /*MapData mapData = new MapData(heightMap, heatMap, moistureMap, airPressureMap, windMap, ranges.ToArray(), rivers.ToArray());*/

        mapDisplay.DisplayHeightMap();
        //mapDisplay.DisplayBiomeMap();
        //Debug.Log("Complete. " + Time.realtimeSinceStartup);
    }

    private void SetMapDimensions()
    {
        if (heightGeneration == HeightGeneration.DiamondSquare)
        {
            int n = 7 + (int)dimensions;
            int size = Mathf.RoundToInt(Mathf.Pow(2, n) + 1);
            mapSize = size;
            return;
        }
        switch (dimensions)
        {
            case MapDimensions.Minute_200:
                mapSize = minuteMapSize;
                break;
            case MapDimensions.Tiny_400:
                mapSize = tinyMapSize;
                break;
            case MapDimensions.Small_800:
                mapSize = smallMapSize;
                break;
            case MapDimensions.Medium_1500:
                mapSize = mediumMapSize;
                break;
            case MapDimensions.Large_3000:
                mapSize = largeMapSize;
                break;
            default:
                mapSize = minuteMapSize;
                break;
        }
    }

    private int GetRainDropIterations()
    {
        switch (dimensions)
        {
            case MapDimensions.Minute_200: return minuteRainDrops;
            case MapDimensions.Tiny_400: return tinyRainDrops;
            case MapDimensions.Small_800: return smallRainDrops;
            case MapDimensions.Medium_1500: return mediumRainDrops;
            case MapDimensions.Large_3000:  return largeRainDrops;
            default: return minuteRainDrops;
        }
    }

    private int GetRiverCount()
    {
        switch (dimensions)
        {
            case MapDimensions.Minute_200: return minuteRiverCount;
            case MapDimensions.Tiny_400: return tinyRiverCount;
            case MapDimensions.Small_800: return smallRiverCount;
            case MapDimensions.Medium_1500: return mediumRiverCount;
            case MapDimensions.Large_3000: return largeRiverCount;
            default: return minuteRiverCount;
        }
    }

    private int GetTectonicPlateCount()
    {
        switch (dimensions)
        {
            case MapDimensions.Minute_200: return minuteTectonicPlates;
            case MapDimensions.Tiny_400: return tinyTectonicPlates;
            case MapDimensions.Small_800: return smallTectonicPlates;
            case MapDimensions.Medium_1500: return mediumTectonicPlates;
            case MapDimensions.Large_3000: return largeTectonicPlates;
            default: return minuteTectonicPlates;
        }
    }

    private float[,] GetHeightMap()
    {
        float[,] heightMap = new float[mapSize, mapSize];
        PlaceTectonicPoints(heightMap);

        float[,] perlinMap = PerlinNoise.GenerateHeightMap(mapSize, seed, noiseScale, octaves, persistence, lacunarity, offset);
        for (int x = 0; x < perlinMap.GetLength(0); x++)
        {
            for (int y = 0; y < perlinMap.GetLength(1); y++)
            {
                perlinMap[x, y] -= 0.1f;
                heightMap[x, y] = Mathf.Clamp(heightMap[x, y] + perlinMap[x, y], 0, 1);
            }
        }

        //Debug.Log("Generating Height Map.");
        if (heightGeneration == HeightGeneration.DiamondSquare)
        {
            heightMap = DiamondSquare.GetHeightMap(mapSize);
            for (int x = 0; x < perlinMap.GetLength(0); x++)
            {
                for (int y = 0; y < perlinMap.GetLength(1); y++)
                {
                    perlinMap[x, y] -= 0.5f;
                    heightMap[x, y] = Mathf.Clamp(heightMap[x, y] + perlinMap[x, y], 0, 1);
                }
            }
        }

        if (useErosion) return GetComponent<Erosion>().Erode(heightMap, GetRainDropIterations(), seed);
        return heightMap;
    }

    private void PlaceTectonicPoints(float[,] heightMap)
    {
        //place n tectonic points and increase the altitude of surrounding nodes within range r by  a flat-top gaussian
        //tectonic points will also result in mountains, volcanoes? 
        int count = GetTectonicPlateCount();

        for (int points = 0; points < count; points++)
        {
            //select a random point on the map
            int nodeX = rng.Next(0, mapSize - 1);
            int nodeY = rng.Next(0, mapSize - 1);
            Debug.Log("Tectonic Point placed at " + nodeX + "," + nodeY);
            TerrainNode tectonicNode = worldMap.GetNode(nodeX, nodeY);
            float range = rng.Next(75, 150); //this will need some testing, probably also scaled for map size
             //Definitely scale with map size

            //Grab all nodes within range
            var nodesInRange = worldMap.GetNodesInRange(tectonicNode, (int)range);

            //Calculate their base height based on distance from the tectonic point
            for (int i = 0; i < nodesInRange.Count; i++)
            {
                //The relative distance from this node to the tectonic node
                float x = worldMap.GetNodeDistance(tectonicNode, nodesInRange[i]) / range;
                float n = 6; //affects the width of the flat-top on the gaussian
                float pow = -10 * Mathf.Pow(x, n); 
                float y = 0.5f * Mathf.Exp(-10 * Mathf.Pow(x, n)); //flat-top gaussian

                heightMap[nodesInRange[i].x, nodesInRange[i].y] = y;
            }

            //place a tectonic point there, should also store this

            //using a flat-top gaussian function, set the base height for all nodes within range r of the point


        }
    }

    private void SetTileValues(float[,] heightMap, float[,] heatMap, float[,] moistureMap, float[,] airPressureMap, SecondaryDirections[,] windMap)
    {
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                TerrainNode node = worldMap.GetNode(x, y);

                node.SetHeightValues(heightMap[x, y], GetTerrain(heightMap[x, y]));
                node.SetTemperatureValues(heatMap[x, y], GetTemperatureZone(heatMap[x, y]));
                node.SetPrecipitationValues(moistureMap[x, y], GetPrecipitationZone(moistureMap[x, y]));

                node.airPressure = airPressureMap[x, y];
                node.windDirection = windMap[x, y];
            }
        }
    }

    private void AdjustTileValues(float[,] moistureMap)
    {
        //Adjust moisture levels to account for rain shadows
        //to get rain shadows, loop through all mountains and find the direction of prevailing winds
        //adjust moisture on either side of mountain based on altitude of range

        moistureMap = GetAdjustedMoistureMap(moistureMap);

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                TerrainNode node = worldMap.GetNode(x, y);
                node.SetPrecipitationValues(moistureMap[x, y], GetPrecipitationZone(moistureMap[x, y]));

                //if (node.isLand) node.SetBiome(GetWhittakerBiome(node));
                if (node.isLand) node.SetBiome(GetBiomeByTable(node));
            }
        }

        ConsolidateBiomes(true);
    }

    private void ConsolidateBiomes(bool repeat)
    {
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                TerrainNode node = worldMap.GetNode(x, y);
                node.AdjustToNeighborBiomes();
            }
        }

        if (repeat) ConsolidateBiomes(false);
    }

    #region - Value Abstractions -
    private TerrainType GetTerrain(float height)
    {
        for (int i = terrainTypes.Length - 1; i >= 0; i--)
        {
            if (height >= terrainTypes[i].Height)
            {
                return terrainTypes[i];
            }
        }
        return null;
    }

    private TemperatureZone GetTemperatureZone(float heatValue)
    {
        for (int i = 0; i < temperatureZones.Length; i++)
        {
            if (heatValue <= temperatureZones[i].TemperatureValue)
            {
                return temperatureZones[i];
            }
        }
        return temperatureZones[0];
    }

    private PrecipitationZone GetPrecipitationZone(float moistureValue)
    {
        for (int i = precipitationZones.Length - 1; i >= 0; i--)
        {
            if (moistureValue >= precipitationZones[i].PrecipitationValue)
            {
                return precipitationZones[i];
            }
        }
        Debug.LogWarning("Ya dun fucked up");
        return precipitationZones[2];
    }

    private Biome GetWhittakerBiome(TerrainNode node)
    {
        float avgAnnualTemp = node.avgAnnualTemperature_C;
        float annualPrecipitation = node.annualPrecipitation_cm;

        for (int i = 0; i < biomes.Length; i++)
        {
            if (biomes[i].FitsWhittakerModel(avgAnnualTemp, annualPrecipitation))
                return biomes[i];
        }
        Debug.LogWarning("Node " + node.x + "," + node.y +  " values do not fit into any biome.");
        return null;
    }

    private Biome GetBiomeByTable(TerrainNode node)
    {
        int temperatureIndex = TemperatureIndex(node.temperatureZone);
        int precipitationIndex = PrecipitationIndex(node.precipitationZone);

        BiomeTypes type = BiomeTable[precipitationIndex, temperatureIndex];
        //Debug.Log(precipitationIndex + " + " + temperatureIndex + " results in " + type);
        for (int i = 0; i < biomes.Length; i++)
        {
            if (biomes[i].BiomeType == type) return biomes[i];
        }
        return null;
    }

    private int TemperatureIndex(TemperatureZone zone)
    {
        if (zone == temperatureZones[0]) return 0;      //Coldest
        else if (zone == temperatureZones[1]) return 1; //Colder
        else if (zone == temperatureZones[2]) return 2; //Cold
        else if (zone == temperatureZones[3]) return 3; //Warm
        else if (zone == temperatureZones[4]) return 4; //Warmer
        else if (zone == temperatureZones[5]) return 5; //Warmest
        else throw new UnityException("What the fuck.");
    }
    
    private int PrecipitationIndex(PrecipitationZone zone)
    {
        if (zone == precipitationZones[0]) return 0;
        else if (zone == precipitationZones[1]) return 1;
        else if (zone == precipitationZones[2]) return 2;
        else if (zone == precipitationZones[3]) return 3;
        else if (zone == precipitationZones[4]) return 4;
        else return 5;
    }
    #endregion

    private float[,] GetAdjustedMoistureMap(float[,] moistureMap)
    {
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                moistureMap[x, y] = worldMap.GetNode(x, y).precipitationValue;
            }
        }
        return moistureMap;
    }

    //Method to check if there is just one giant mountain covering the whole map
    private bool NeedToRestart(MountainRange[] mountains)
    {
        if (mountains.Length > 5) return false;
        for (int i = 0; i < mountains.Length; i++)
        {
            if (mountains[i].Nodes.Count > 200)
            {
                return true;
            }
        }
        return false;
    }

    public void SetRandomSeed()
    {
        seed = Random.Range(-100000, 100000);
    }

    private void OnValidate()
    {
        if (lacunarity < 1) lacunarity = 1;
        if (octaves < 0) octaves = 0;
    }
}

public enum HeightGeneration { Perlin, DiamondSquare }
public enum MapDimensions { Minute_200, Tiny_400, Small_800, Medium_1500, Large_3000 }
public enum Direction { North, South, East, West}
public enum SecondaryDirections { NorthEast, NorthWest, SouthEast, SouthWest }