using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DelaunayVoronoi;
using JS.World.Map.Features;
using JS.World.Map.Climate;
using JS.Math;

//Special thaks to http://entropicparticles.com/6-days-of-creation

// Features To Add:
//      Rain Shadows
//      Ocean Currents
//

namespace JS.World.Map.Generation
{
    /// <summary>
    /// Generates world altitude, climate, and biomes
    /// </summary>
    public class TerrainGenerator : MonoBehaviour 
    {
        public int mapSize { get; private set; }

        [SerializeField] private WorldGenerator worldGenerator;
        [SerializeField] private Erosion erosion;

        [Space]

        [SerializeField] private Features.TerrainData terrainData;
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

        private List<WorldTile> _water;
        private List<WorldTile> _land;

        public void SetInitialValues(int size)
        {
            _water = new List<WorldTile>();
            _land = new List<WorldTile>();

            mapSize = size;
            terrainData.MapSize = mapSize;
        }

        #region - Altitude -
        /// <summary>
        /// Randomly places tectonic plates and raises the altitude of surrouning nodes
        /// </summary>
        public void PlaceTectonicPlates(int plateCount, int minPlateSize, int maxPlateSize)
        {
            //place n tectonic points and increase the altitude of surrounding nodes within range r by  a flat-top gaussian
            //tectonic points will also result in mountains, volcanoes? Fault lines?
            //place fault lines using Voronoi polygons, this is where volcanoes and mountains will be added
            float[,] heightMap = new float[mapSize, mapSize];
            int border = Mathf.RoundToInt(minPlateSize * 0.5f);
            var plates = new List<WorldTile>();
            //Alternatively I could make this a while loop and just continue if I get a point that's too close
            for (int points = 0; points < plateCount; points++)
            {
                
                int nodeX, nodeY; //select a random point on the map
                if (points < plateCount / 2) //First half will favor the center of the map
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
                plates.Add(tectonicNode);
                float range = worldGenerator.PRNG.Next(minPlateSize, maxPlateSize);

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
            FindPlateBoundaries(plates);
            terrainData.HeightMap = heightMap;
        }

        /// <summary>
        /// Creates a Voronoi diagram given the tectonic points and calculates plate boundaries.
        /// </summary>
        private void FindPlateBoundaries(List<WorldTile> nodes)
        {
            var plateBorders = new HashSet<WorldTile>();
            var delaunay = new DelaunayTriangulator();
            var voronoi = new Voronoi();

            var points = delaunay.ConvertToPoints(nodes, mapSize);
            var triangulation = delaunay.BowyerWatson(points);
            var voronoiEdges = voronoi.GenerateEdgesFromDelaunay(triangulation);

            foreach (var edge in voronoiEdges)
            {
                var x0 = Mathf.RoundToInt(Mathf.Clamp((float)edge.Point1.X, 0, mapSize));
                var y0 = Mathf.RoundToInt(Mathf.Clamp((float)edge.Point1.Y, 0, mapSize));
                var x1 = Mathf.RoundToInt(Mathf.Clamp((float)edge.Point2.X, 0, mapSize));
                var y1 = Mathf.RoundToInt(Mathf.Clamp((float)edge.Point2.Y, 0, mapSize));

                var bresenham = Bresenham.PlotLine(x0, y0, x1, y1);
                foreach(var p in bresenham)
                {
                    var newNode = worldMap.GetNode(p.x, p.y);
                    if (newNode != null && !plateBorders.Contains(newNode))
                        plateBorders.Add(newNode);
                }
            }
            terrainData.PlateBorders = plateBorders;
        }

        /// <summary>
        /// Generates a height map using randomly placed tectonic plates and Perlin Noise
        /// </summary>
        public void GenerateHeightMap()
        {
            float[,] heightMap = terrainData.HeightMap;

            float[,] perlinMap = PerlinNoise.GenerateHeightMap(mapSize, worldMap.Seed, noiseScale, octaves, persistence, lacunarity, offset);
            float[,] falloffMap = FalloffGenerator.GenerateFalloffMap(mapSize);
            for (int x = 0; x < heightMap.GetLength(0); x++)
            {
                for (int y = 0; y < heightMap.GetLength(1); y++)
                {
                    perlinMap[x, y] -= 0.25f;
                    heightMap[x, y] += perlinMap[x, y];
                    heightMap[x, y] -= falloffMap[x, y];

                    heightMap[x, y] = Mathf.Clamp(heightMap[x, y], 0, 1);
                }
            }

            terrainData.HeightMap = heightMap;
        }

        /// <summary>
        /// Simulates erosion on the height map using a Raindrop algorithm.
        /// </summary>
        public void ErodeLandMasses(int iterations)
        {
            float[,] heightMap = terrainData.HeightMap;
            float[,] initial = terrainData.HeightMap;
            heightMap = erosion.Erode(heightMap, iterations, worldMap.Seed);
            terrainData.HeightMap = heightMap;

            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    float diff = initial[x, y] - heightMap[x, y];
                    //if (diff != 0) Debug.Log("Difference: [" + x + "," + y + "] = " + diff);
                }
            }
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
                    bool isLand = heightMap[x, y] >= WorldParameters.SEA_LEVEL;

                    node.SetAltitude(heightMap[x, y], isLand);
                    if (isLand) _land.Add(node);
                    else _water.Add(node);

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
            while(_water.Count > 0)
            {
                var body = FloodFillRegion(_water[0], false);
                bool trueLake = LakeIsLandlocked(body);

                if (trueLake)
                {
                    Lake newLake = new Lake(lakes.Count);
                    newLake.GridNodes = new GridCoordinates[body.Count];
                    lakes.Add(newLake);

                    for (int i = 0; i < body.Count; i++)
                    {
                        newLake.GridNodes[i] = new GridCoordinates(body[i].x, body[i].y);
                        body[i].SetBiome(biomeHelper.Lake);
                    }
                }

                for (int i = 0; i < body.Count; i++)
                {
                    _water.Remove(body[i]);
                }

                yield return null;
            }
            terrainData.Lakes = lakes.ToArray();
        }

        private bool LakeIsLandlocked(List<WorldTile> tiles)
        {
            if (tiles.Count >= 1000)
            {
                //UnityEngine.Debug.Log("Lake size is greater than 1,000. " + Nodes.Count);
                //return false;
            }

            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].x == 0 || tiles[i].x == mapSize - 1) return false;
                if (tiles[i].y == 0 || tiles[i].y == mapSize - 1) return false;
            }
            return true;
        }

        /// <summary>
        /// Identifies and registers Islands.
        /// </summary>
        public IEnumerator IdentifyLandMasses()
        {
            var land = new List<LandMass>();
            while (_land.Count > 0)
            {
                var body = FloodFillRegion(_land[0], true);
                LandMass newLand = new LandMass(land.Count);

                if (body.Count < mapSize / 2) newLand.Size = LandSize.Islet;
                else if (body.Count < mapSize) newLand.Size = LandSize.Island;
                else newLand.Size = LandSize.Continent;

                var coords = new GridCoordinates[body.Count];
                for (int i = 0; i < body.Count; i++)
                {
                    coords[i] = new GridCoordinates(body[i].x, body[i].y);
                    _land.Remove(body[i]);
                }
                newLand.GridNodes = coords;

                land.Add(newLand);
                yield return null;
            }
            terrainData.LandMasses = land.ToArray();
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
                        if (!node.neighbors_all[i].IsLand && node.Rivers.Count == 0)
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
                    if (terrainData.HeightMap[node.x, node.y] >= WorldParameters.MOUNTAIN_HEIGHT)
                    {
                        node.SetBiome(biomeHelper.Mountain);
                        node.CheckNeighborMountains();
                    }
                }
            }
            var ranges = Topography.FindMountainRanges(worldMap, mapSize);
            terrainData.Mountains = ranges.ToArray();
        }
        #endregion

        public void GenerateHeatMap()
        {
            //Create Heat Map
            float[,] heatMap = ClimateMath.GenerateHeatMap(terrainData.HeightMap, worldGenerator.PRNG);

            //Pass heat values to nodes
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    WorldTile node = worldMap.GetNode(x, y);
                    node.SetTemperatureValues(heatMap[x, y], ClimateMath.GetHeatIndex(heatMap[x, y]));
                }
            }

            terrainData.HeatMap = heatMap;
        }

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
            float[,] moistureMap = DampedCosine.GetMoistureMap(heightMap, WorldParameters.SEA_LEVEL, worldGenerator.PRNG);
            //This is entirely onteologic at the moment

            //Apply effects of prevailing winds to generate rain shadows
            CreateRainShadows(terrainData.Mountains, windMap);

            //Pass precipitation values to nodes
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    WorldTile node = worldMap.GetNode(x, y);
                    node.SetPrecipitationValues(moistureMap[x, y], ClimateMath.GetPrecipitationZone(moistureMap[x, y]));
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
            ConsolidateBiomes(3);
            SaveBiomeMap();
            FindBiomeGroups();
        }

        private void ConsolidateBiomes(int iterations)
        {
            for (int count = 0; count < iterations; count++)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    for (int y = 0; y < mapSize; y++)
                    {
                        CheckBiomeNeighbors(worldMap.GetNode(x, y));
                    }
                }
            }
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
            var newZone = ClimateMath.GetHeatIndex(newTemp);
            tile.SetTemperatureValues(newTemp, newZone);

            var newPrecip = Mathf.Clamp(tile.precipitationValue,
                newBiome.MinPrecipitation / 400f,
                newBiome.MaxPrecipitation / 400f);
            var precipZone = ClimateMath.GetPrecipitationZone(newPrecip);
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
        public void GenerateOreDeposits(float coal, float copper, float iron, float silver, float gold, float gems, float mithril, float adamantine)
        {
            float[,] coalMap = PerlinNoise.GenerateHeightMap(mapSize, worldGenerator.PRNG.Next(), noiseScale, octaves, persistence, lacunarity, offset);
            float[,] copperMap = PerlinNoise.GenerateHeightMap(mapSize, worldGenerator.PRNG.Next(), noiseScale, octaves, persistence, lacunarity, offset);
            float[,] ironMap = PerlinNoise.GenerateHeightMap(mapSize, worldGenerator.PRNG.Next(), noiseScale, octaves, persistence, lacunarity, offset);
            float[,] silverMap = PerlinNoise.GenerateHeightMap(mapSize, worldGenerator.PRNG.Next(), noiseScale, octaves, persistence, lacunarity, offset);
            float[,] goldMap = PerlinNoise.GenerateHeightMap(mapSize, worldGenerator.PRNG.Next(), noiseScale, octaves, persistence, lacunarity, offset);
            float[,] mithrilMap = PerlinNoise.GenerateHeightMap(mapSize, worldGenerator.PRNG.Next(), noiseScale, octaves, persistence, lacunarity, offset);
            float[,] adamantineMap = PerlinNoise.GenerateHeightMap(mapSize, worldGenerator.PRNG.Next(), noiseScale, octaves, persistence, lacunarity, offset);
            float[,] gemstoneMap = PerlinNoise.GenerateHeightMap(mapSize, worldGenerator.PRNG.Next(), noiseScale, octaves, persistence, lacunarity, offset);
            float[,] saltMap = PerlinNoise.GenerateHeightMap(mapSize, worldGenerator.PRNG.Next(), noiseScale, octaves, persistence, lacunarity, offset);


            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    if (terrainData.HeightMap[x, y] < WorldParameters.SEA_LEVEL)
                    {
                        coalMap[x, y] = 0;
                        copperMap[x, y] = 0;
                        ironMap[x, y] = 0;
                        silverMap[x, y] = 0;
                        goldMap[x, y] = 0;
                        gemstoneMap[x, y] = 0;
                        mithrilMap[x, y] = 0;
                        adamantineMap[x, y] = 0;
                        saltMap[x, y] = 0;
                        continue;
                    }

                    if (coalMap[x, y] < 1 - coal) coalMap[x, y] = 0;
                    if (copperMap[x, y] < 1 - copper) copperMap[x, y] = 0;
                    if (ironMap[x, y] < 1 - iron) ironMap[x, y] = 0;
                    if (silverMap[x, y] < 1 - silver) silverMap[x, y] = 0;
                    if (goldMap[x, y] < 1 - gold) goldMap[x, y] = 0;
                    if (gemstoneMap[x, y] < 1 - gems) gemstoneMap[x, y] = 0;
                    if (mithrilMap[x, y] < 1 - mithril) mithrilMap[x, y] = 0;
                    if (adamantineMap[x, y] < 1 - adamantine) adamantineMap[x, y] = 0;
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

public enum Compass { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest }