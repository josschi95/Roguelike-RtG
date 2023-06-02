using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace JS.WorldMap
{
    public class LocalMapGenerator : MonoBehaviour
    {
        [SerializeField] private NodeReference nodeReference;
        [SerializeField] private LocalMapDisplay localMapDisplay;
        [SerializeField] private WorldData worldData;

        [SerializeField] private BiomeHelper biomeHelper;
        [SerializeField] private WorldGenerationParameters worldGenParams;

        [SerializeField] private SettlementData settlementData;
        [SerializeField] private PathTileHelper riverTiles;
        [Space]

        [SerializeField] private Tilemap landMap;
        [SerializeField] private Tilemap featuresMap;
        [SerializeField] private Tilemap riverMap;

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

        public bool roundCorner = true;
        private int seed;
        private System.Random prng;

        public void CheckForNode()
        {
            GenerateLocalMap(nodeReference.x, nodeReference.y);
        }

        private void GenerateLocalMap(int worldX, int worldY)
        {
            var node = worldData.GetNode(worldX, worldY);
            seed = worldData.TerrainData.SeedMap[worldX, worldY];
            prng = new System.Random(seed);
            var biome = biomeHelper.GetBiome(node.BiomeID);

            LocalMapInfo info = new LocalMapInfo();
            info.x = worldX; info.y = worldY;
            info.PerlinMap = PerlinNoise.GenerateHeightMap(worldGenParams.LocalDimensions.x, seed, noiseScale, octaves, persistence, lacunarity, offset);
            info.featurePoints = Poisson.GeneratePoints(seed, 10, worldGenParams.LocalDimensions);

            var river = worldData.TerrainData.FindRiverAt(worldX, worldY, out var index);
            if (river != null)
            {
                //Block out poisson map
            }

            //ok so.... 
            //World Tiles are going to end up only serving as data holders for world map info, such as biomes etc.
            //Local maps will be made up of GridNodes which will be used for Pathfinding


            //Now what needs to be held on the local map level is....


            // x and y of course
            // pathfinding information g/h/f costs
            // do I need to hold entities? 

            localMapDisplay.DisplayMap(info);
        }

        #region - Rivers -
        //Note that I can use this same stuff for a road with a simple bool
        private void PlaceRiver(RiverNode node)
        {
            var width = worldGenParams.LocalDimensions.x;
            var height = worldGenParams.LocalDimensions.y;
            int verticalOffset = Mathf.RoundToInt(width / 2);
            verticalOffset -= Mathf.RoundToInt(node.Size / 2);

            int horizontalOffset = Mathf.RoundToInt(height / 2);
            horizontalOffset -= Mathf.RoundToInt(node.Size / 2);

            if (node.Flow == Compass.North || node.Flow == Compass.South)
            {
                //Vertical river
                PlaceNorthRiver(height, verticalOffset, node.Size, node.Flow);
                PlaceSouthRiver(height, verticalOffset, node.Size, node.Flow);
            }
            else if (node.Flow == Compass.East || node.Flow == Compass.West)
            {
                //Horizontal river
                PlaceEastRiver(height, horizontalOffset, node.Size, node.Flow);
                PlaceWestRiver(height, horizontalOffset, node.Size, node.Flow);
            }
            else if (node.Flow == Compass.NorthEast)
            {
                PlaceNorthRiver(height, verticalOffset, node.Size, node.Flow);
                PlaceEastRiver(height, horizontalOffset, node.Size, node.Flow);
                RoundCorner(node.Flow, node.Size);
            }
            else if (node.Flow == Compass.SouthEast)
            {
                PlaceSouthRiver(height, verticalOffset, node.Size, node.Flow);
                PlaceEastRiver(height, horizontalOffset, node.Size, node.Flow);
                RoundCorner(node.Flow, node.Size);
            }
            else if (node.Flow == Compass.SouthWest)
            {
                PlaceSouthRiver(height, verticalOffset, node.Size, node.Flow);
                PlaceWestRiver(height, horizontalOffset, node.Size, node.Flow);
                RoundCorner(node.Flow, node.Size);
            }
            else if (node.Flow == Compass.NorthWest)
            {
                PlaceNorthRiver(height, verticalOffset, node.Size, node.Flow);
                PlaceWestRiver(height, horizontalOffset, node.Size, node.Flow);
                RoundCorner(node.Flow, node.Size);
            }
        }

        private void PlaceNorthRiver(int height, int offset, int riverWidth, Compass direction)
        {
            //Vertical river
            var halfHeight = height / 2;
            for (int i = halfHeight; i < height; i++)
            {
                for (int j = 0; j < riverWidth; j++)
                {
                    var pos = new Vector3Int(offset + j, i);
                    riverMap.SetTile(pos, riverTiles.GetRiverTile(direction));
                }

            }
        }

        private void PlaceSouthRiver(int height, int offset, int riverWidth, Compass direction)
        {
            var halfHeight = height / 2;
            for (int i = 0; i < halfHeight; i++)
            {
                for (int j = 0; j < riverWidth; j++)
                {
                    var pos = new Vector3Int(offset + j, i);
                    riverMap.SetTile(pos, riverTiles.GetRiverTile(direction));
                }
            }
        }

        private void PlaceEastRiver(int width, int offset, int riverWidth, Compass direction)
        {
            var halfWidth = width / 2;
            for (int i = halfWidth; i < width; i++)
            {
                for (int j = 0; j < riverWidth; j++)
                {
                    var pos = new Vector3Int(i, offset + j);
                    riverMap.SetTile(pos, riverTiles.GetRiverTile(direction));
                }
            }
        }

        private void PlaceWestRiver(int width, int offset, int riverWidth, Compass direction)
        {
            var halfWidth = width / 2;
            for (int i = 0; i < halfWidth; i++)
            {
                for (int j = 0; j < riverWidth; j++)
                {
                    var pos = new Vector3Int(i, offset + j);
                    riverMap.SetTile(pos, riverTiles.GetRiverTile(direction));
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
                            riverMap.SetTile(pos, riverTiles.GetRiverTile(direction));
                        }
                    }
                    break;
                case Compass.SouthEast:
                    for (int i = 0; i < corner.Count; i++)
                    {
                        if (corner[i].x < halfWidth && corner[i].y > halfHeight)
                        {
                            var pos = new Vector3Int(corner[i].x, corner[i].y);
                            riverMap.SetTile(pos, riverTiles.GetRiverTile(direction));
                        }
                    }
                    break;
                case Compass.SouthWest:
                    for (int i = 0; i < corner.Count; i++)
                    {
                        if (corner[i].x > halfWidth - 1 && corner[i].y > halfHeight)
                        {
                            var pos = new Vector3Int(corner[i].x, corner[i].y);
                            riverMap.SetTile(pos, riverTiles.GetRiverTile(direction));
                        }
                    }
                    break;
                case Compass.NorthWest:
                    for (int i = 0; i < corner.Count; i++)
                    {
                        if (corner[i].x > halfWidth - 1 && corner[i].y < halfHeight)
                        {
                            var pos = new Vector3Int(corner[i].x, corner[i].y);
                            riverMap.SetTile(pos, riverTiles.GetRiverTile(direction));
                        }
                    }
                    break;
            }
        }
        #endregion
    }
}