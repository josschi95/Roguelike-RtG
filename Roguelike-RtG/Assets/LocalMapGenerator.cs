using JS.WorldMap;
using JS.WorldMap.Generation;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

namespace JS.WorldMap
{
    public class LocalMapGenerator : MonoBehaviour
    {
        [SerializeField] private WorldData worldData;

        [SerializeField] private BiomeHelper biomeHelper;
        [SerializeField] private WorldGenerationParameters worldGenParams;

        [SerializeField] private SettlementData settlementData;
        [SerializeField] private PathTileHelper riverTiles;
        [SerializeField] private DirectionTiles directionTiles;
        [Space]

        [SerializeField] private Tilemap oceanMap;
        [SerializeField] private Tilemap landMap;
        [SerializeField] private Tilemap terrainFeatureMap;
        [SerializeField] private Tilemap riverMap;
        [SerializeField] private Tilemap roadMap;

        [Header("Perlin Noise")]
        [SerializeField] private float noiseScale = 30;
        [Tooltip("The number of iterations of Perlin Noise over an area")]
        [SerializeField] private int octaves = 4;
        [Range(0, 1)]
        [Tooltip("Controls decrease in amplitude of subsequent octaves")]
        [SerializeField] private float persistence = 0.5f;
        [Tooltip("Controls increase in frequency of octaves")]
        [SerializeField] private float lacunarity = 2f;
        [SerializeField] private Vector2 offset;

        [Space]

        public int nodeX, nodeY;

        [Space]
        public RuleTile lowTile;
        public RuleTile highTile;
        public RuleTile treeTile;
        public int seed = 50;

        [Space]

        public bool hasRiver;
        public Compass riverDirection;
        public int riverWidth;
        public bool roundCorner = true;


        private System.Random rng;

        private void Start()
        {
            GenerateLocalMap(nodeX, nodeY);
            PlaceTrees();
            if (hasRiver) PlaceRiver();
            PlaceOutline();
        }

        public void GenerateLocalMap(int worldX, int worldY)
        {
            //var seed = worldData.TerrainData.SeedMap[worldX, worldY];
            rng = new System.Random(seed);

            //var biome = worldData.TerrainData.GetBiome(worldX, worldY);

            float[,] perlinMap = PerlinNoise.GenerateHeightMap(worldGenParams.LocalDimensions.x, seed, noiseScale, octaves, persistence, lacunarity, offset);

            for (int x = 0; x < perlinMap.GetLength(0); x++)
            {
                for (int y = 0; y < perlinMap.GetLength(1); y++)
                {
                    var tilePos = new Vector3Int(x, y);
                    if (perlinMap[x,y] > 0.5f)
                    {
                        landMap.SetTile(tilePos, highTile);
                    }
                    else
                    {
                        landMap.SetTile(tilePos, lowTile);
                    }
                }
            }
            for (int x = 0; x < worldGenParams.LocalDimensions.x; x++)
            {
                var south = new Vector3Int(x, -1);
                roadMap.SetTile(south, directionTiles.GetTile(Compass.South));
                var north = new Vector3Int(x, perlinMap.GetLength(1));
                roadMap.SetTile(north, directionTiles.GetTile(Compass.North));
            }
            for (int y = 0; y < perlinMap.GetLength(1); y++)
            {
                var east = new Vector3Int(worldGenParams.LocalDimensions.x, y);
                roadMap.SetTile(east, directionTiles.GetTile(Compass.East));
                var west = new Vector3Int(-1, y);
                roadMap.SetTile(west, directionTiles.GetTile(Compass.West));
            }

            //ok so.... 
            //World Tiles are going to end up only serving as data holders for world map info, such as biomes etc.
            //Local maps will be made up of GridNodes which will be used for Pathfinding


            //Now what needs to be held on the local map level is....


            // x and y of course
            // pathfinding information g/h/f costs
            // do I need to hold entities? 


        }

        private void PlaceTrees()
        {
            var points = Poisson.GeneratePoints(seed, 10, worldGenParams.LocalDimensions);
            for (int i = 0; i < points.Count; i++)
            {
                var pos = new Vector3Int((int)points[i].x, (int)points[i].y);
                terrainFeatureMap.SetTile(pos, treeTile);
            }
        }

        private void PlaceOutline()
        {
            var width = worldGenParams.LocalDimensions.x;
            var height = worldGenParams.LocalDimensions.y;

            for (int x = 0; x < width; x++)
            {
                var south = new Vector3Int(x, -1);
                roadMap.SetTile(south, directionTiles.GetTile(Compass.South));
                var north = new Vector3Int(x, height);
                roadMap.SetTile(north, directionTiles.GetTile(Compass.North));
            }
            for (int y = 0; y < height; y++)
            {
                var east = new Vector3Int(width, y);
                roadMap.SetTile(east, directionTiles.GetTile(Compass.East));
                var west = new Vector3Int(-1, y);
                roadMap.SetTile(west, directionTiles.GetTile(Compass.West));
            }
        }

        #region - Rivers -
        //Note that I can use this same stuff for a road with a simple bool
        private void PlaceRiver()
        {
            var width = worldGenParams.LocalDimensions.x;
            var height = worldGenParams.LocalDimensions.y;
            int verticalOffset = Mathf.RoundToInt(width / 2);
            verticalOffset -= Mathf.RoundToInt(riverWidth / 2);
            int horizontalOffset = Mathf.RoundToInt(height / 2);
            horizontalOffset -= Mathf.RoundToInt(riverWidth / 2);

            if (riverDirection == Compass.North || riverDirection == Compass.South)
            {
                //Vertical river
                PlaceNorthRiver(height, verticalOffset, riverWidth);
                PlaceSouthRiver(height, verticalOffset, riverWidth);
            }
            else if (riverDirection == Compass.East || riverDirection == Compass.West)
            {
                //Horizontal river
                PlaceEastRiver(height, horizontalOffset, riverWidth);
                PlaceWestRiver(height, horizontalOffset, riverWidth);
            }
            else if (riverDirection == Compass.NorthEast)
            {
                PlaceNorthRiver(height, verticalOffset, riverWidth);
                PlaceEastRiver(height, horizontalOffset, riverWidth);
                RoundCorner(riverDirection, riverWidth);
            }
            else if (riverDirection == Compass.SouthEast)
            {
                PlaceSouthRiver(height, verticalOffset, riverWidth);
                PlaceEastRiver(height, horizontalOffset, riverWidth);
                RoundCorner(riverDirection, riverWidth);
            }
            else if (riverDirection == Compass.SouthWest)
            {
                PlaceSouthRiver(height, verticalOffset, riverWidth);
                PlaceWestRiver(height, horizontalOffset, riverWidth);
                RoundCorner(riverDirection, riverWidth);
            }
            else if (riverDirection == Compass.NorthWest)
            {
                PlaceNorthRiver(height, verticalOffset, riverWidth);
                PlaceWestRiver(height, horizontalOffset, riverWidth);
                RoundCorner(riverDirection, riverWidth);
            }
        }

        private void PlaceNorthRiver(int height, int offset, int riverWidth)
        {
            //Vertical river
            var halfHeight = height / 2;
            for (int i = halfHeight; i < height; i++)
            {
                for (int j = 0; j < riverWidth; j++)
                {
                    var pos = new Vector3Int(offset + j, i);
                    riverMap.SetTile(pos, riverTiles.GetRiverTile(riverDirection));
                }

            }
        }

        private void PlaceSouthRiver(int height, int offset, int riverWidth)
        {
            var halfHeight = height / 2;
            for (int i = 0; i < halfHeight; i++)
            {
                for (int j = 0; j < riverWidth; j++)
                {
                    var pos = new Vector3Int(offset + j, i);
                    riverMap.SetTile(pos, riverTiles.GetRiverTile(riverDirection));
                }
            }
        }

        private void PlaceEastRiver(int width, int offset, int riverWidth)
        {
            var halfWidth = width / 2;
            for (int i = halfWidth; i < width; i++)
            {
                for (int j = 0; j < riverWidth; j++)
                {
                    var pos = new Vector3Int(i, offset + j);
                    riverMap.SetTile(pos, riverTiles.GetRiverTile(riverDirection));
                }
            }
        }

        private void PlaceWestRiver(int width, int offset, int riverWidth)
        {
            var halfWidth = width / 2;
            for (int i = 0; i < halfWidth; i++)
            {
                for (int j = 0; j < riverWidth; j++)
                {
                    var pos = new Vector3Int(i, offset + j);
                    riverMap.SetTile(pos, riverTiles.GetRiverTile(riverDirection));
                }
            }
        }

        private void RoundCorner(Compass direction, int riverWidth)
        {
            if (!roundCorner) return;

            var width = worldGenParams.LocalDimensions.x;
            var height = worldGenParams.LocalDimensions.y;
            int halfWidth = Mathf.RoundToInt(width / 2);
            int halfHeight = Mathf.RoundToInt(height / 2);

            var offset = Mathf.RoundToInt(riverWidth / 2);
            var center = new Vector2Int(halfWidth - offset, halfHeight - offset);
            var innerCorner = new Vector2Int(halfWidth, halfHeight);
            var corner = new List<Vector2Int>();
            for (int i = 0; i < riverWidth; i++)
            {
                for (int j = 0; j < riverWidth; j++)
                {
                    var point = center + new Vector2Int(i, j);
                    if (Vector2.Distance(innerCorner, point) < riverWidth * 0.5f)
                        corner.Add(point);
                }
            }

            switch(direction)
            {
                case Compass.NorthEast:
                    for (int i = 0; i < corner.Count; i++)
                    {
                        if (corner[i].x < halfWidth && corner[i].y < halfHeight)
                        {
                            var pos = new Vector3Int(corner[i].x, corner[i].y);
                            riverMap.SetTile(pos, riverTiles.GetRiverTile(riverDirection));
                        }
                    }
                    break;
                case Compass.SouthEast:
                    for (int i = 0; i < corner.Count; i++)
                    {
                        if (corner[i].x < halfWidth && corner[i].y > halfHeight)
                        {
                            var pos = new Vector3Int(corner[i].x, corner[i].y);
                            riverMap.SetTile(pos, riverTiles.GetRiverTile(riverDirection));
                        }
                    }
                    break;
                case Compass.SouthWest:
                    for (int i = 0; i < corner.Count; i++)
                    {
                        if (corner[i].x > halfWidth - 1 && corner[i].y > halfHeight)
                        {
                            var pos = new Vector3Int(corner[i].x, corner[i].y);
                            riverMap.SetTile(pos, riverTiles.GetRiverTile(riverDirection));
                        }
                    }
                    break;
                case Compass.NorthWest:
                    for (int i = 0; i < corner.Count; i++)
                    {
                        if (corner[i].x > halfWidth - 1 && corner[i].y < halfHeight)
                        {
                            var pos = new Vector3Int(corner[i].x, corner[i].y);
                            riverMap.SetTile(pos, riverTiles.GetRiverTile(riverDirection));
                        }
                    }
                    break;
            }
        }
        #endregion
    }
}