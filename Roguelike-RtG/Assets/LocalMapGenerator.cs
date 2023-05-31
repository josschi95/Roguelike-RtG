using JS.WorldMap;
using JS.WorldMap.Generation;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace JS.WorldMap
{
    public class LocalMapGenerator : MonoBehaviour
    {
        [SerializeField] private WorldData worldData;

        [SerializeField] private BiomeHelper biomeHelper;
        [SerializeField] private WorldGenerationParameters worldGenParams;

        [SerializeField] private SettlementData settlementData;
        [SerializeField] private RiverTileHelper tileHelper;
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
        int seed = 50;
        private System.Random rng;

        private void Start()
        {
            GenerateLocalMap(nodeX, nodeY);
        }

        public void GenerateLocalMap(int worldX, int worldY)
        {
            //var seed = worldData.TerrainData.SeedMap[worldX, worldY];
            rng = new System.Random(seed);

            //var biome = worldData.TerrainData.GetBiome(worldX, worldY);

            float[,] perlinMap = PerlinNoise.GenerateHeightMap(100, seed, noiseScale, octaves, persistence, lacunarity, offset);

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
            //ok so.... 
            //World Tiles are going to end up only serving as data holders for world map info, such as biomes etc.
            //Local maps will be made up of GridNodes which will be used for Pathfinding


            //Now what needs to be held on the local map level is....


            // x and y of course
            // pathfinding information g/h/f costs
            // do I need to hold entities? 


        }
    }
}