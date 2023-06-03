using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Features To Add:
//      Rain Shadows
//      Ocean Currents
//

namespace JS.WorldMap.Generation
{
    /// <summary>
    /// Generates world altitude, climate, and biomes
    /// </summary>
    public class TerrainGenerator : MonoBehaviour 
    {
        public int mapSize { get; private set; }
        private WorldSize worldSize;

        [SerializeField] private WorldGenerator worldGenerator;
        [SerializeField] private RiverGenerator riverGenerator;
        [SerializeField] private Erosion erosion;

        [Space]

        [SerializeField] private WorldGenerationParameters worldGenParams;
        [SerializeField] private TerrainData terrainData;
        [SerializeField] private WorldData worldMap;
        [SerializeField] private BiomeHelper biomeHelper;

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

        public void SetInitialValues(WorldSize size)
        {
            worldSize = size;
            water = new List<WorldTile>();
            land = new List<WorldTile>();

            mapSize = worldGenParams.MapSize(worldSize);
            terrainData.MapSize = mapSize;
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

            int count = worldGenParams.TectonicPlates(worldSize);
            int border = Mathf.RoundToInt(worldGenParams.MinPlateSize(worldSize) * 0.5f);
            for (int points = 0; points < count; points++)
            {
                
                int nodeX, nodeY; //select a random point on the map
                //nodeX = worldGenerator.rng.Next(border, mapSize - border);
                //nodeY = worldGenerator.rng.Next(border, mapSize - border);
                if (points < count / 2) //First half will favor the center of the map
                {
                    nodeX = worldGenerator.PRNG.Next(border * 2, mapSize - border * 2);
                    nodeY = worldGenerator.PRNG.Next(border * 2, mapSize - border * 2);
                }
                else
                {
                    nodeX = worldGenerator.PRNG.Next(border, mapSize - border);
                    nodeY = worldGenerator.PRNG.Next(border, mapSize - border);
                }

                WorldTile tectonicNode = worldMap.GetNode(nodeX, nodeY);
                tectonicNode.isTectonicPoint = true;
                float range = worldGenerator.PRNG.Next(worldGenParams.MinPlateSize(worldSize), worldGenParams.MaxPlateSize(worldSize));

                //Grab all nodes within range
                var nodesInRange = worldMap.GetNodesInRange_Circle(tectonicNode, (int)range);

                //Calculate their base height based on distance from the tectonic point
                for (int i = 0; i < nodesInRange.Count; i++)
                {
                    //nodesInRange[i].isTectonicPoint = true; //Mark as tectonic node, mainly for visual referencing

                    //The relative distance from this node to the tectonic node
                    float x = GridMath.GetStraightDist(tectonicNode.x, tectonicNode.y, nodesInRange[i].x, nodesInRange[i].y) / range;

                    float n = 6; //affects the width of the flat-top on the gaussian
                    float y = 0.5f * Mathf.Exp(-10 * Mathf.Pow(x, n)); //flat-top gaussian
                    
                    //Only set it to be y if the point is lower, so plates don't sink each other
                    if (heightMap[nodesInRange[i].x, nodesInRange[i].y] < y)
                        heightMap[nodesInRange[i].x, nodesInRange[i].y] = y;
                }
            }
            terrainData.HeightMap = heightMap;
        }

        /// <summary>
        /// Generates a height map using randomly placed tectonic plates and Perlin Noise
        /// </summary>
        public void GenerateHeightMap()
        {
            float[,] heightMap = terrainData.HeightMap;

            float[,] perlinMap = PerlinNoise.GenerateHeightMap(mapSize, worldMap.Seed, noiseScale, octaves, persistence, lacunarity, offset);
            for (int x = 0; x < perlinMap.GetLength(0); x++)
            {
                for (int y = 0; y < perlinMap.GetLength(1); y++)
                {
                    perlinMap[x, y] -= 0.25f;
                    heightMap[x, y] = Mathf.Clamp(heightMap[x, y] + perlinMap[x, y], 0, 1);
                }
            }

            terrainData.HeightMap = heightMap;
        }

        /// <summary>
        /// Simulates erosion on the height map using a Raindrop algorithm.
        /// </summary>
        public void ErodeLandMasses()
        {
            float[,] heightMap = terrainData.HeightMap;
            heightMap = erosion.Erode(heightMap, worldGenParams.Raindrops(worldSize), worldMap.Seed);
            terrainData.HeightMap = heightMap;
        }

        public void SetNodeAltitudeValues()
        {
            float[,] heightMap = terrainData.HeightMap;

            //This is doing nothing and is EXTREMELY FLAWED only taking into account altitude
            //float[,] airPressureMap = AirPressureData.GetAirPressureMap(heightMap);

            //Pass height and air pressure values to nodes
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    WorldTile node = worldMap.GetNode(x, y);
                    bool isLand = heightMap[x, y] >= worldGenParams.SeaLevel;

                    node.SetAltitude(heightMap[x, y], isLand);
                    if (isLand) land.Add(node);
                    else water.Add(node);

                    //node.airPressure = airPressureMap[x, y];
                }
            }
            terrainData.HeightMap = heightMap;
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
                bool trueLake = newLake.IsLandLocked(mapSize);
                if (trueLake)
                {
                    newLake.ID = lakes.Count;
                    lakes.Add(newLake);
                }
                else newLake.DeconstructLake();

                for (int i = 0; i < body.Count; i++)
                {
                    water.Remove(body[i]);
                    if (trueLake) body[i].SetBiome(biomeHelper.Lake);
                }

                yield return null;
            }
            terrainData.Lakes = lakes.ToArray();
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
                    var coords = new GridCoordinates[body.Count];
                    for (int i = 0; i < body.Count; i++)
                    {
                        coords[i] = new GridCoordinates(body[i].x, body[i].y);
                        if (newIsland.Nodes.Contains(body[i]))
                        {
                            newIsland.Nodes.Add(body[i]);
                        }
                    }
                    newIsland.GridNodes = coords;
                    islands.Add(newIsland);
                }

                for (int i = 0; i < body.Count; i++)
                {
                    land.Remove(body[i]);
                }

                yield return null;
            }
            terrainData.Islands = islands.ToArray();
        }

        /// <summary>
        /// Flood Fill Algorithm to find all neighbor land/water tiles
        /// </summary>
        private List<WorldTile> FloodFillRegion(WorldTile startNode, bool isLand)
        {
            var tiles = new List<WorldTile>();

            if (startNode.IsLand != isLand) throw new UnityException("Start Node does not align with given parameters!");

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
                    if (mapFlags[neighbor.x, neighbor.y] == 0 && neighbor.IsLand == isLand)
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
            bool[,] coasts = new bool[mapSize, mapSize];

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    WorldTile node = worldMap.GetNode(x, y);
                    if (!node.IsLand) continue;
                    for (int i = 0; i < node.neighbors_all.Count; i++)
                    {
                        if (!node.neighbors_all[i].IsLand && node.rivers.Count == 0)
                        {
                            coasts[x, y] = true;
                        }
                    }
                }
            }
            terrainData.Coasts = coasts;
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
                    if (terrainData.HeightMap[node.x, node.y] >= worldGenParams.MountainHeight)
                    {
                        node.SetBiome(biomeHelper.Mountain);
                        node.CheckNeighborMountains();
                    }
                }
            }
            var ranges = Topography.FindMountainRanges(worldMap, mapSize);
            terrainData.Mountains = ranges.ToArray();
        }

        public void GenerateRivers()
        {
            //Create rivers
            terrainData.Rivers = riverGenerator.GenerateRivers(mapSize, worldGenParams.RiverCount(worldSize)).ToArray();
        }
        #endregion

        #region - Temperature -
        public void GenerateHeatMap()
        {
            //Create Heat Map
            float[,] heatMap = TemperatureData.GenerateHeatMap(terrainData.HeightMap, worldGenParams.SeaLevel, worldGenerator.PRNG);

            //Pass heat values to nodes
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    WorldTile node = worldMap.GetNode(x, y);
                    node.SetTemperatureValues(heatMap[x, y], GetTemperatureZone(heatMap[x, y]));
                }
            }

            terrainData.HeatMap = heatMap;
        }

        private int GetTemperatureZone(float heatValue)
        {
            for (int i = 0; i < worldGenParams.TemperatureZones.Length; i++)
            {
                if (heatValue <= worldGenParams.TemperatureZones[i].TemperatureValue)
                {
                    return worldGenParams.TemperatureZones[i].ID;
                }
            }
            throw new UnityException("Node temperature is outside bounds of designated zones. " + heatValue);
        }
        #endregion

        #region - Precipitation -
        public void GeneratePrecipitationMap()
        {
            var heightMap = terrainData.HeightMap;
            //Create Wind Map
            Compass[,] windMap = AirPressureData.GetWindMap(heightMap);
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
            float[,] moistureMap = DampedCosine.GetMoistureMap(heightMap, worldGenParams.SeaLevel, worldGenerator.PRNG);
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
            float[,] adjustedMoistureMap = new float[mapSize, mapSize];
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    WorldTile node = worldMap.GetNode(x, y);
                    adjustedMoistureMap[x, y] = node.precipitationValue;
                }
            }
            terrainData.MoistureMap = adjustedMoistureMap;
        }

        private int GetPrecipitationZone(float moistureValue)
        {
            for (int i = worldGenParams.PrecipitationZones.Length - 1; i >= 0; i--)
            {
                if (moistureValue >= worldGenParams.PrecipitationZones[i].PrecipitationValue)
                {
                    return worldGenParams.PrecipitationZones[i].ID;
                }
            }
            throw new UnityException("Node precipitation is outside bounds of designated zones. " + moistureValue);
        }

        /// <summary>
        /// Removes moisture from leeward side of mountains and moves to windward sides
        /// </summary>
        private void CreateRainShadows(MountainRange[] mountains, Compass[,] windMap)
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
                    if (node.IsLand) node.SetBiome(biomeHelper.GetWhittakerTableBiome(node));
                    else if (!node.hasBiome) node.SetBiome(biomeHelper.GetOceanBiome(node)); //exclude lakes
                }
            }

            OverrideMountainBiomes();
            ConsolidateBiomes(2);
            SaveBiomeMap();
            FindBiomeGroups();
        }

        private void ConsolidateBiomes(int iterations)
        {
            if (iterations <= 0) return;

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    CheckBiomeNeighbors(worldMap.GetNode(x, y));
                }
            }

            ConsolidateBiomes(iterations - 1);
        }

        //Check if this tile is surrounded by a biome of different types
        private void CheckBiomeNeighbors(WorldTile tile)
        {
            if (!tile.IsLand) return;
            int differentNeighbors = 0; //number of neighbors with a different biome
            int neighborBiome = 0; //the biome this tile will switch to

            for (int i = 0; i < tile.neighbors_adj.Count; i++)
            {
                if (!tile.neighbors_adj[i].hasBiome) continue;
                if (!tile.neighbors_adj[i].IsLand) continue; //don't adjust to water biomes
                if (tile.neighbors_adj[i].BiomeID == tile.BiomeID) continue;
                neighborBiome = tile.neighbors_adj[i].BiomeID;
                differentNeighbors++;
            }

            if (differentNeighbors >= 3) AdjustNodeBiome(tile, neighborBiome);
        }

        private void AdjustNodeBiome(WorldTile tile, int biomeID)
        {
            var newBiome = biomeHelper.GetBiome(biomeID);

            tile.SetBiome(newBiome);
            var newTemp = Mathf.Clamp(tile.heatValue,
                Temperature.CelsiusToFarenheit(newBiome.MinAvgTemp) / 100f,
                Temperature.CelsiusToFarenheit(newBiome.MaxAvgTemp) / 100f);
            var newZone = GetTemperatureZone(newTemp);
            tile.SetTemperatureValues(newTemp, newZone);

            var newPrecip = Mathf.Clamp(tile.precipitationValue,
                newBiome.MinPrecipitation / 400f,
                newBiome.MaxPrecipitation / 400f);
            var precipZone = GetPrecipitationZone(newPrecip);
            tile.SetPrecipitationValues(newPrecip, precipZone);
        }

        private void SaveBiomeMap()
        {
            int[,] biomeMap = new int[mapSize, mapSize];
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    WorldTile node = worldMap.GetNode(x, y);
                    if (node.hasBiome) biomeMap[x, y] = node.BiomeID;
                }
            }
            terrainData.BiomeMap = biomeMap;
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
                        node.BiomeGroup.ID = biomes.Count;
                        biomes.Add(node.BiomeGroup);
                        //Debug.Log("New biome group found! " + node.BiomeGroup.biome + " (" + node.BiomeGroup.ID + ")");
                    }
                }
            }
            terrainData.BiomeGroups = biomes.ToArray();
        }

        private void OverrideMountainBiomes()
        {
            foreach(var range in terrainData.Mountains)
            {
                for (int i = 0; i < range.Nodes.Count; i++)
                {
                    range.Nodes[i].SetBiome(biomeHelper.Mountain);
                }
            }
        }
        #endregion

        #region - Resources -
        /// <summary>
        /// Generates maps for precious metals and gemstones
        /// </summary>
        public void GenerateOreDeposits()
        {
            float[,] coalMap = PerlinNoise.GenerateHeightMap(mapSize, worldGenerator.PRNG.Next(), noiseScale, octaves, persistence, lacunarity, offset);
            float[,] copperMap = PerlinNoise.GenerateHeightMap(mapSize, worldGenerator.PRNG.Next(), noiseScale, octaves, persistence, lacunarity, offset);
            float[,] ironMap = PerlinNoise.GenerateHeightMap(mapSize, worldGenerator.PRNG.Next(), noiseScale, octaves, persistence, lacunarity, offset);
            float[,] silverMap = PerlinNoise.GenerateHeightMap(mapSize, worldGenerator.PRNG.Next(), noiseScale, octaves, persistence, lacunarity, offset);
            float[,] goldMap = PerlinNoise.GenerateHeightMap(mapSize, worldGenerator.PRNG.Next(), noiseScale, octaves, persistence, lacunarity, offset);
            float[,] mithrilMap = PerlinNoise.GenerateHeightMap(mapSize, worldGenerator.PRNG.Next(), noiseScale, octaves, persistence, lacunarity, offset);
            float[,] adamantineMap = PerlinNoise.GenerateHeightMap(mapSize, worldGenerator.PRNG.Next(), noiseScale, octaves, persistence, lacunarity, offset);
            float[,] gemstoneMap = PerlinNoise.GenerateHeightMap(mapSize, worldGenerator.PRNG.Next(), noiseScale, octaves, persistence, lacunarity, offset);

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    if (terrainData.HeightMap[x, y] < worldGenParams.SeaLevel)
                    {
                        coalMap[x, y] = 0;
                        copperMap[x, y] = 0;
                        ironMap[x, y] = 0;
                        silverMap[x, y] = 0;
                        goldMap[x, y] = 0;
                        gemstoneMap[x, y] = 0;
                        mithrilMap[x, y] = 0;
                        adamantineMap[x, y] = 0;
                        continue;
                    }

                    if (coalMap[x, y] < 1 - worldGenParams.CoalPrevalence) coalMap[x, y] = 0;
                    if (copperMap[x, y] < 1 - worldGenParams.CopperPrevalence) copperMap[x, y] = 0;
                    if (ironMap[x, y] < 1 - worldGenParams.IronPrevalence) ironMap[x, y] = 0;
                    if (silverMap[x, y] < 1 - worldGenParams.SilverPrevalence) silverMap[x, y] = 0;
                    if (goldMap[x, y] < 1 - worldGenParams.GoldPrevalence) goldMap[x, y] = 0;
                    if (gemstoneMap[x, y] < 1 - worldGenParams.GemstonesPrevalence) gemstoneMap[x, y] = 0;
                    if (mithrilMap[x, y] < 1 - worldGenParams.MithrilPrevalence) mithrilMap[x, y] = 0;
                    if (adamantineMap[x, y] < 1 - worldGenParams.AdamantinePrevalence) adamantineMap[x, y] = 0;
                }
            }

            terrainData.CoalMap = coalMap;
            terrainData.CopperMap = copperMap;
            terrainData.IronMap = ironMap;
            terrainData.SilverMap = silverMap;
            terrainData.GoldMap = goldMap;
            terrainData.MithrilMap = mithrilMap;
            terrainData.AdmanatineMap = adamantineMap;
            terrainData.GemstoneMap = gemstoneMap;
        }
        #endregion

        private void OnValidate()
        {
            if (lacunarity < 1) lacunarity = 1;
            if (octaves < 0) octaves = 0;
        }
    }
}

public enum WorldSize { Tiny, Small, Medium, Large, Huge }
public enum Direction { North, South, East, West}

public enum Compass { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest }