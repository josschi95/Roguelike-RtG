using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using JS.EventSystem;

// Features To Add:
//      Rain Shadows
//      Ocean Currents
//

namespace JS.WorldGeneration
{
    /// <summary>
    /// Generates world altitude, climate, and biomes
    /// </summary>
    public class TerrainGenerator : MonoBehaviour 
    {
        public int mapSize { get; private set; }
        public Vector3Int origin { get; private set; }

        private int seed;
        private WorldSize worldSize;
        private float seaLevel = 0.4f;

        [SerializeField] private WorldGenerator worldGenerator;
        [SerializeField] private RiverGenerator riverGenerator;
        [SerializeField] private Erosion erosion;
        //private WorldMap worldMap;

        [SerializeField] private WorldGenerationParameters mapFeatures;
        [SerializeField] private TerrainData terrainData;
        [SerializeField] private WorldMapData worldMap;

        [Header("Perlin Noise")]
        [SerializeField] private float noiseScale;
        [Tooltip("The number of iterations of Perlin Noise over an area")]
        [SerializeField] private int octaves = 4;
        [Range(0, 1)]
        [Tooltip("Controls decrease in amplitude of subsequent octaves")]
        [SerializeField] private float persistence = 0.5f;
        [Tooltip("Controls increase in frequency of octaves")]
        [SerializeField] private float lacunarity = 2f;
        [SerializeField] private Vector2 offset;

        private List<WorldTile> water;
        private List<WorldTile> land;

        [SerializeField] private Biome Tundra, Taiga, TemperateGrassland, Shrubland, DeciduousForest, Desert, TropicalSeasonalForest, Savanna, Jungle;


        public void SetInitialValues(WorldSize size, int seed)
        {
            this.seed = seed;
            worldSize = size;
            //worldMap = WorldMap.instance;

            terrainData.ClearData();
            seaLevel = mapFeatures.SeaLevel;
            water = new List<WorldTile>();
            land = new List<WorldTile>();

            mapSize = mapFeatures.MapSize(worldSize);
            origin = new Vector3Int(Mathf.FloorToInt(-mapSize / 2f), Mathf.FloorToInt(-mapSize / 2f));
            terrainData.SetMapSize(mapSize, origin);

            worldMap.CreateGrid(mapSize, mapSize);
        }

        #region - Altitude -
        /// <summary>
        /// Randomly places tectonic plates and raises the altitude of surrouning nodes
        /// </summary>
        public void PlaceTectonicPlates()
        {
            //place n tectonic points and increase the altitude of surrounding nodes within range r by  a flat-top gaussian
            //tectonic points will also result in mountains, volcanoes? Fault lines?
            //place fault lines using Voronoi polygons, this is where volcanoes and mountains will be added
            float[,] heightMap = new float[mapSize, mapSize];

            int count = mapFeatures.TectonicPlates(worldSize);
            
            for (int points = 0; points < count; points++)
            {
                
                int nodeX, nodeY; //select a random point on the map
                nodeX = worldGenerator.rng.Next(0, mapSize - 1);
                nodeY = worldGenerator.rng.Next(0, mapSize - 1);
                /*if (points < count / 2) //First half points will favor the center of the map
                {
                    int border = mapSize / 5;
                    nodeX = worldGenerator.rng.Next(border, mapSize - border - 1);
                    nodeY = worldGenerator.rng.Next(border, mapSize - border - 1);
                }
                else
                {
                    nodeX = worldGenerator.rng.Next(0, mapSize - 1);
                    nodeY = worldGenerator.rng.Next(0, mapSize - 1);
                }*/

                WorldTile tectonicNode = worldMap.GetNode(nodeX, nodeY);
                tectonicNode.isTectonicPoint = true;
                float range = worldGenerator.rng.Next(mapFeatures.MinPlateSize(worldSize), mapFeatures.MaxPlateSize(worldSize));

                //Grab all nodes within range
                var nodesInRange = worldMap.GetNodesInRange_Circle(tectonicNode, (int)range);

                //Calculate their base height based on distance from the tectonic point
                for (int i = 0; i < nodesInRange.Count; i++)
                {
                    nodesInRange[i].isTectonicPoint = true; //Mark as tectonic node, mainly for visual referencing

                    //The relative distance from this node to the tectonic node
                    float x = worldMap.GetNodeDistance_Straight(tectonicNode, nodesInRange[i]) / range;

                    float n = 6; //affects the width of the flat-top on the gaussian
                    float y = 0.5f * Mathf.Exp(-10 * Mathf.Pow(x, n)); //flat-top gaussian
                    
                    //Only set it to be y if the point is lower, so plates don't sink each other
                    if (heightMap[nodesInRange[i].x, nodesInRange[i].y] < y)
                        heightMap[nodesInRange[i].x, nodesInRange[i].y] = y;
                }
            }
            terrainData.heightMap = heightMap;
        }

        /// <summary>
        /// Generates a height map using randomly placed tectonic plates and Perlin Noise
        /// </summary>
        public void GenerateHeightMap()
        {
            float[,] heightMap = terrainData.heightMap;

            float[,] perlinMap = PerlinNoise.GenerateHeightMap(mapSize, seed, noiseScale, octaves, persistence, lacunarity, offset);
            for (int x = 0; x < perlinMap.GetLength(0); x++)
            {
                for (int y = 0; y < perlinMap.GetLength(1); y++)
                {
                    perlinMap[x, y] -= 0.25f;
                    heightMap[x, y] = Mathf.Clamp(heightMap[x, y] + perlinMap[x, y], 0, 1);
                }
            }

            terrainData.heightMap = heightMap;
        }

        /// <summary>
        /// Simulates erosion on the height map using the Raindrop algorithm.
        /// </summary>
        public void ErodeLandMasses()
        {
            float[,] heightMap = terrainData.heightMap;
            heightMap = erosion.Erode(heightMap, mapFeatures.Raindrops(worldSize), seed);
            terrainData.heightMap = heightMap;
        }

        public void SetNodeAltitudeValues()
        {
            float[,] heightMap = terrainData.heightMap;

            //This is doing nothing and is EXTREMELY FLAWED only taking into account altitude
            //float[,] airPressureMap = AirPressureData.GetAirPressureMap(heightMap);

            //Pass height and air pressure values to nodes
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    WorldTile node = worldMap.GetNode(x, y);
                    var altitude = GetAltitude(heightMap[x, y]);
                    node.SetAltitude(heightMap[x, y], altitude);
                    if (altitude.isLand) land.Add(node);
                    else water.Add(node);

                    //node.airPressure = airPressureMap[x, y];
                }
            }
            terrainData.heightMap = heightMap;
        }

        /// <summary>
        /// Returns an Altitude Zone Scriptable Object based on given height value.
        /// </summary>
        private AltitudeZone GetAltitude(float height)
        {
            for (int i = 0; i < mapFeatures.AltitudeZones.Length; i++)
            {
                if (height <= mapFeatures.AltitudeZones[i].Height)
                {
                    return mapFeatures.AltitudeZones[i];
                }
            }
            throw new UnityException("Node altitude is outside bounds of designated zones. " + height);
        }
        #endregion

        #region - Terrain Features -
        /// <summary>
        /// Identifies and registers Lakes.
        /// </summary>   
        public IEnumerator IdentifyBodiesOfWater()
        {
            var lakes = new List<Lake>();
            while(water.Count > 0)
            {
                var body = FloodFillRegion(water[0], false);
                Lake newLake = new Lake();
                newLake.AddRange(body);
                if (newLake.IsLandLocked(mapSize))
                {
                    newLake.FinalizeValues(lakes.Count);
                    lakes.Add(newLake);
                }
                else newLake.DeconstructLake();

                for (int i = 0; i < body.Count; i++)
                {
                    water.Remove(body[i]);
                }

                yield return null;
            }
            terrainData.SetLakes(lakes.ToArray());
        }

        /// <summary>
        /// Identifies and registers Islands.
        /// </summary>
        public IEnumerator IdentifyLandMasses()
        {
            var islands = new List<Island>();
            while (land.Count > 0)
            {
                var body = FloodFillRegion(land[0], true);
                if (body.Count <= mapSize)
                {
                    Island newIsland = new Island(islands.Count);
                    newIsland.AddRange(body);
                    islands.Add(newIsland);
                }

                for (int i = 0; i < body.Count; i++)
                {
                    land.Remove(body[i]);
                }

                yield return null;
            }
            terrainData.SetIslands(islands.ToArray());
        }

        /// <summary>
        /// Flood Fill Algorithm to find all neighbor land/water tiles
        /// </summary>
        private List<WorldTile> FloodFillRegion(WorldTile startNode, bool isLand)
        {
            var tiles = new List<WorldTile>();

            if (startNode.isNotWater != isLand) throw new UnityException("Start Node does not align with given parameters!");

            int[,] mapFlags = new int[mapSize, mapSize];

            Queue<WorldTile> queue = new Queue<WorldTile>();
            queue.Enqueue(startNode);

            while(queue.Count > 0)
            {
                var node = queue.Dequeue();
                tiles.Add(node);

                for (int i = 0; i < node.neighbors_adj.Count; i++)
                {
                    var neighbor = node.neighbors_adj[i];
                    if (mapFlags[neighbor.x, neighbor.y] == 0 && neighbor.isNotWater == isLand)
                    {
                        mapFlags[neighbor.x, neighbor.y] = 1;
                        queue.Enqueue(neighbor);
                    }
                }
            }
            return tiles;
        }

        public void IdentifyCoasts()
        {
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    WorldTile node = worldMap.GetNode(x, y);
                    if (!node.isNotWater) continue;
                    for (int i = 0; i < node.neighbors_adj.Count; i++)
                    {
                        if (!node.neighbors_adj[i].isNotWater && node.rivers.Count == 0)
                        {
                            node.isCoast = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Identifies and registers Mountains.
        /// </summary>
        public void IdentifyMountains()
        {
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    WorldTile node = worldMap.GetNode(x, y);
                    if (node.altitudeZone.isMountain) 
                        node.CheckNeighborMountains();
                }
            }
            var ranges = Topography.FindMountainRanges(worldMap, mapSize);
            terrainData.SetMountains(ranges.ToArray());
        }

        public void GenerateRivers()
        {
            //Create rivers
            var rivers = riverGenerator.GenerateRivers(mapSize, mapFeatures.RiverCount(worldSize));
            terrainData.SetRivers(rivers.ToArray());
        }
        #endregion

        #region - Temperature -
        public void GenerateHeatMap()
        {
            //Create Heat Map
            float[,] heatMap = TemperatureData.GenerateHeatMap(terrainData.heightMap, seaLevel, worldGenerator.rng);

            //Pass heat values to nodes
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    WorldTile node = worldMap.GetNode(x, y);
                    node.SetTemperatureValues(heatMap[x, y], GetTemperatureZone(heatMap[x, y]));
                }
            }

            //probably pass this to terrainData
            //return heatMap;
        }

        private TemperatureZone GetTemperatureZone(float heatValue)
        {
            for (int i = 0; i < mapFeatures.TemperatureZones.Length; i++)
            {
                if (heatValue <= mapFeatures.TemperatureZones[i].TemperatureValue)
                {
                    return mapFeatures.TemperatureZones[i];
                }
            }
            throw new UnityException("Node temperature is outside bounds of designated zones. " + heatValue);
        }
        #endregion

        #region - Precipitation -
        public void GeneratePrecipitationMap()
        {
            var heightMap = terrainData.heightMap;
            //Create Wind Map
            SecondaryDirections[,] windMap = AirPressureData.GetWindMap(heightMap);
            //Other factors that need to be taken into account
                //Coriolis effect
                //Convergence zones

            //Pass prevailing wind direction to nodes
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    WorldTile node = worldMap.GetNode(x, y);
                    node.windDirection = windMap[x, y];
                }
            }

            //Create Initial Moisture Map - !!!FLAWED!!!
            float[,] moistureMap = DampedCosine.GetMoistureMap(heightMap, seaLevel, worldGenerator.rng);
            //This is entirely onteologic at the moment

            //Apply effects of prevailing winds to generate rain shadows
            CreateRainShadows(terrainData.Mountains, windMap);

            //Pass precipitation values to nodes
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    WorldTile node = worldMap.GetNode(x, y);
                    node.SetPrecipitationValues(moistureMap[x, y], GetPrecipitationZone(moistureMap[x, y]));
                }
            }

            //pass to terrain data
            //return moistureMap;
        }

        private PrecipitationZone GetPrecipitationZone(float moistureValue)
        {
            for (int i = mapFeatures.PrecipitationZones.Length - 1; i >= 0; i--)
            {
                if (moistureValue >= mapFeatures.PrecipitationZones[i].PrecipitationValue)
                {
                    return mapFeatures.PrecipitationZones[i];
                }
            }
            throw new UnityException("Node precipitation is outside bounds of designated zones. " + moistureValue);
        }

        /// <summary>
        /// Removes moisture from leeward side of mountains and moves to windward sides
        /// </summary>
        private void CreateRainShadows(MountainRange[] mountains, SecondaryDirections[,] windMap)
        {
            for (int i = 0; i < mountains.Length; i++)
            {
                //get dominant prevailing wind direction

                //need to figure out windward side and leeward side

                //from there, the rain shadow should extend out from the center 


            }
        }
        #endregion

        #region - Biomes -
        public void GenerateBiomes()
        {
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    WorldTile node = worldMap.GetNode(x, y);
                    if (node.isNotWater) node.SetBiome(GetWhittakerTableBiome(node));
                }
            }

            ConsolidateBiomes(2);

            FindBiomeGroups();
        }
        
        //Notes: Change boreal to Taiga, tropical rainforest to jungle

        private Biome GetWhittakerTableBiome(WorldTile node)
        {
            int temperatureIndex = TemperatureIndex(node.temperatureZone);
            int precipitationIndex = PrecipitationIndex(node.precipitationZone);

            switch (temperatureIndex)
            {
                case 0:
                    if (precipitationIndex == 5) return Taiga;
                    return Tundra;
                case 1:
                    if (precipitationIndex >= 3) return Taiga;
                    return Tundra;
                case 2:
                    if (precipitationIndex >= 3) return DeciduousForest;
                    if (precipitationIndex == 2) return Shrubland;
                    return TemperateGrassland;
                case 3:
                    if (precipitationIndex == 0) return Desert;
                    if (precipitationIndex == 1) return TemperateGrassland;
                    if (precipitationIndex == 4) return DeciduousForest;
                    if (precipitationIndex == 5) return TropicalSeasonalForest;
                    return Shrubland;
                case 4:
                    if (precipitationIndex < 2) return Desert;
                    if (precipitationIndex == 2) return Savanna;
                    if (precipitationIndex == 3) return TropicalSeasonalForest;
                    return Jungle;
                case 5:
                    if (precipitationIndex <= 1) return Desert;
                    if (precipitationIndex <= 3) return Savanna;
                    return Jungle;
                default: return Shrubland;
            }
        }



        private int TemperatureIndex(TemperatureZone zone)
        {
            if (zone == mapFeatures.TemperatureZones[0]) return 0;      //Coldest
            else if (zone == mapFeatures.TemperatureZones[1]) return 1; //Colder
            else if (zone == mapFeatures.TemperatureZones[2]) return 2; //Cold
            else if (zone == mapFeatures.TemperatureZones[3]) return 3; //Warm
            else if (zone == mapFeatures.TemperatureZones[4]) return 4; //Warmer
            else if (zone == mapFeatures.TemperatureZones[5]) return 5; //Warmest
            else throw new UnityException("What the fuck.");
        }

        private int PrecipitationIndex(PrecipitationZone zone)
        {
            if (zone == mapFeatures.PrecipitationZones[0]) return 0;
            else if (zone == mapFeatures.PrecipitationZones[1]) return 1;
            else if (zone == mapFeatures.PrecipitationZones[2]) return 2;
            else if (zone == mapFeatures.PrecipitationZones[3]) return 3;
            else if (zone == mapFeatures.PrecipitationZones[4]) return 4;
            else return 5;
        }

        private void ConsolidateBiomes(int iterations)
        {
            if (iterations <= 0) return;

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    WorldTile node = worldMap.GetNode(x, y);
                    node.AdjustToNeighborBiomes();
                }
            }

            ConsolidateBiomes(iterations - 1);
        }

        private void FindBiomeGroups()
        {
            var biomes = new List<BiomeGroup>();
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    WorldTile node = worldMap.GetNode(x, y);
                    if (node.BiomeGroup != null && !biomes.Contains(node.BiomeGroup))
                    {
                        node.BiomeGroup.FinalizeValues(biomes.Count);
                        biomes.Add(node.BiomeGroup);
                        //Debug.Log("New biome group found! " + node.BiomeGroup.biome + " (" + node.BiomeGroup.ID + ")");
                    }
                }
            }
            terrainData.SetBiomes(biomes.ToArray());
        }
        #endregion

        private void OnValidate()
        {
            if (lacunarity < 1) lacunarity = 1;
            if (octaves < 0) octaves = 0;
        }


        #region - Obsolete -
        /*
        private Biome GetBiomeByTable(TerrainNode node)
        {
            int temperatureIndex = TemperatureIndex(node.temperatureZone);
            int precipitationIndex = PrecipitationIndex(node.precipitationZone);

            BiomeTypes type = BiomeTable[precipitationIndex, temperatureIndex];
            for (int i = 0; i < mapFeatures.Biomes.Length; i++)
            {
                if (mapFeatures.Biomes[i].BiomeType == type) return mapFeatures.Biomes[i];
            }
            throw new UnityException("Node temperature and precipitation outside bounds of designated zones. " +
                temperatureIndex + ", " + precipitationIndex);
        }

        private BiomeTypes[,] BiomeTable = new BiomeTypes[6, 6] {   
        //COLDEST                   //COLDER                    //COLD                          //HOT                                   //HOTTER                            //HOTTEST
        { BiomeTypes.Tundra,        BiomeTypes.Tundra,       BiomeTypes.TemperateGrassland,    BiomeTypes.Desert,                   BiomeTypes.Desert,                   BiomeTypes.Desert },              //DRYEST
        { BiomeTypes.Tundra,        BiomeTypes.Tundra,       BiomeTypes.TemperateGrassland,    BiomeTypes.TemperateGrassland,       BiomeTypes.Desert,                   BiomeTypes.Desert },              //DRYER
        { BiomeTypes.Tundra,        BiomeTypes.Tundra,       BiomeTypes.Woodland,              BiomeTypes.Woodland,                 BiomeTypes.Savanna,                  BiomeTypes.Savanna },             //DRY
        { BiomeTypes.Tundra,        BiomeTypes.BorealForest, BiomeTypes.DeciduousForest,       BiomeTypes.Woodland,                 BiomeTypes.TropicalSeasonalForest,   BiomeTypes.Savanna },             //WET
        { BiomeTypes.Tundra,        BiomeTypes.BorealForest, BiomeTypes.DeciduousForest,       BiomeTypes.DeciduousForest,          BiomeTypes.TropicalRainforest,       BiomeTypes.TropicalRainforest },  //WETTER
        { BiomeTypes.BorealForest,  BiomeTypes.BorealForest, BiomeTypes.DeciduousForest,       BiomeTypes.TropicalSeasonalForest,   BiomeTypes.TropicalRainforest,       BiomeTypes.TropicalRainforest }   //WETTEST
        };
        */
        #endregion
    }
}

public enum WorldSize { Tiny, Small, Medium, Large, Huge }
public enum Direction { North, South, East, West}
public enum SecondaryDirections { NorthEast, NorthWest, SouthEast, SouthWest }

public enum Compass { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest }