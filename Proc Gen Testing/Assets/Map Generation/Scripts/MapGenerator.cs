using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int mapSize { get; private set; }
    public Vector3Int origin { get; private set; }
    private float seaLevel = 0.4f;

    private WorldMap worldMap;
    private MapDisplay mapDisplay;
    private RiverGenerator riverGenerator;

    [SerializeField] private MapFeatures mapFeatures;
    [SerializeField] private WorldSize worldSize;
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
    }

    public void GenerateMap()
    {
        if (randomSeed) SetRandomSeed();
        rng = new System.Random(seed);

        seaLevel = mapFeatures.SeaLevel;

        mapSize = mapFeatures.MapSize(worldSize);
        origin = new Vector3Int(Mathf.FloorToInt(transform.position.x - mapSize / 2f), Mathf.FloorToInt(transform.position.y - mapSize / 2f));
        var go = new GameObject("Origin");
        go.transform.position = origin;

        worldMap.CreateGrid(mapSize, mapSize);

        float[,] heightMap = GetHeightMap();

        //Debug.Log("Generating Heat Map. " + Time.realtimeSinceStartup);
        float[,] heatMap = TemperatureData.GenerateHeatMap(heightMap, seaLevel);
        //Debug.Log("Generating Moisture Map. " + Time.realtimeSinceStartup);
        float[,] moistureMap = DampedCosine.GetMoistureMap(heightMap, seaLevel);

        float[,] airPressureMap = AirPressureData.GetAirPressureMap(heightMap);
        SecondaryDirections[,] windMap = AirPressureData.GetWindMap(heightMap);

        SetTileValues(heightMap, heatMap, moistureMap, airPressureMap, windMap);

        var ranges = MountainFinder.FindMountainRanges(mapSize);

        var rivers = riverGenerator.GenerateRivers(mapSize, ranges, mapFeatures.RiverCount(worldSize));

        AdjustTileValues(moistureMap);

        /*MapData mapData = new MapData(heightMap, heatMap, moistureMap, airPressureMap, windMap, ranges.ToArray(), rivers.ToArray());*/

        mapDisplay.DisplayHeightMap();
        //mapDisplay.DisplayBiomeMap();
        //Debug.Log("Complete. " + Time.realtimeSinceStartup);
    }

    private float[,] GetHeightMap()
    {
        float[,] heightMap = new float[mapSize, mapSize];
        PlaceTectonicPlates(heightMap);

        float[,] perlinMap = PerlinNoise.GenerateHeightMap(mapSize, seed, noiseScale, octaves, persistence, lacunarity, offset);
        for (int x = 0; x < perlinMap.GetLength(0); x++)
        {
            for (int y = 0; y < perlinMap.GetLength(1); y++)
            {
                perlinMap[x, y] -= 0.25f;
                heightMap[x, y] = Mathf.Clamp(heightMap[x, y] + perlinMap[x, y], 0, 1);
            }
        }

        if (useErosion) return GetComponent<Erosion>().Erode(heightMap, mapFeatures.Raindrops(worldSize), seed);
        return heightMap;
    }

    private void PlaceTectonicPlates(float[,] heightMap)
    {
        //place n tectonic points and increase the altitude of surrounding nodes within range r by  a flat-top gaussian
        //tectonic points will also result in mountains, volcanoes? Fault lines?
        //place fault lines using Voronoi polygons, this is where volcanoes and mountains will be added

        int count = mapFeatures.TectonicPlates(worldSize);
        int border = mapSize / 5;
        for (int points = 0; points < count; points++)
        {
            //select a random point on the map
            int nodeX = rng.Next(0, mapSize - 1);
            int nodeY = rng.Next(0, mapSize - 1);

            if (points < 4) //First four points will favor the center of the map
            {
                nodeX = rng.Next(border, mapSize - border - 1);
                nodeY = rng.Next(border, mapSize - border - 1);
            }

            TerrainNode tectonicNode = worldMap.GetNode(nodeX, nodeY);
            tectonicNode.isTectonicPoint = true;
            float range = rng.Next(mapFeatures.MinPlateSize(worldSize), mapFeatures.MaxPlateSize(worldSize)); //this will need some testing, probably also scaled for map size
            //could probably be a little bigger for a small map


            //Grab all nodes within range
            var nodesInRange = worldMap.GetNodesInRange(tectonicNode, (int)range);

            //Calculate their base height based on distance from the tectonic point
            for (int i = 0; i < nodesInRange.Count; i++)
            {
                nodesInRange[i].isTectonicPoint = true;
                //The relative distance from this node to the tectonic node
                float x = worldMap.GetNodeDistance(tectonicNode, nodesInRange[i]) / range;
                float n = 6; //affects the width of the flat-top on the gaussian
                float y = 0.5f * Mathf.Exp(-10 * Mathf.Pow(x, n)); //flat-top gaussian
                //Only set it to be y if the point is lower, so plates don't sink each other
                if (heightMap[nodesInRange[i].x, nodesInRange[i].y] < y)
                    heightMap[nodesInRange[i].x, nodesInRange[i].y] = y;
            }
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

public enum WorldSize { Tiny, Small, Medium, Large, Huge }
public enum Direction { North, South, East, West}
public enum SecondaryDirections { NorthEast, NorthWest, SouthEast, SouthWest }