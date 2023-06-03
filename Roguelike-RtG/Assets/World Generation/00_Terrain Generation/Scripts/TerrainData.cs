using UnityEngine;

namespace JS.WorldMap
{
    [CreateAssetMenu(fileName = "Terrain Data", menuName = "World Generation/Terrain/Terrain Data")]
    public class TerrainData : ScriptableObject
    {
        #region - Map Data -
        private int mapSize;
        public int MapSize
        {
            get => mapSize;
            set => mapSize = value;
        }

        private int[,] seedMap;
        public int[,] SeedMap
        {
            get => seedMap;
            set
            {
                seedMap = value;
            }
        }

        //Elevation
        private float[,] heightMap;
        public float[,] HeightMap
        {
            get => heightMap;
            set => heightMap = value;
        }

        //Temperature
        private float[,] heatMap;
        public float[,] HeatMap
        {
            get => heatMap;
            set => heatMap = value;
        }

        //Moisture
        private float[,] moistureMap;
        public float[,] MoistureMap
        {
            get => moistureMap;
            set => moistureMap = value;
        }

        //Biomes
        private int[,] biomeMap;
        public int[,] BiomeMap
        {
            get => biomeMap;
            set => biomeMap = value;
        }

        //Coasts
        private bool[,] coasts;
        public bool[,] Coasts
        {
            get => coasts;
            set => coasts = value;
        }

        #region - Resources -
        //Resources
        private float[,] coalMap;
        public float[,] CoalMap
        {
            get => coalMap;
            set => coalMap = value;
        }
        private float[,] copperMap;
        public float[,] CopperMap
        {
            get => copperMap;
            set => copperMap = value;
        }
        private float[,] ironMap;
        public float[,] IronMap
        {
            get => ironMap;
            set => ironMap = value;
        }
        private float[,] silverMap;
        public float[,] SilverMap
        {
            get => silverMap;
            set => silverMap = value;
        }
        private float[,] goldMap;
        public float[,] GoldMap
        {
            get => goldMap;
            set => goldMap = value;
        }
        private float[,] mithrilMap;
        public float[,] MithrilMap
        {
            get => mithrilMap;
            set => mithrilMap = value;
        }
        private float[,] adamantineMap;
        public float[,] AdmanatineMap
        {
            get => adamantineMap;
            set => adamantineMap = value;
        }
        private float[,] gemstoneMap;
        public float[,] GemstoneMap
        {
            get => gemstoneMap;
            set => gemstoneMap = value;
        }
        #endregion

        #region - Terrain Features -
        private MountainRange[] mountains;
        public MountainRange[] Mountains
        {
            get => mountains;
            set => mountains = value;
        }

        private Lake[] lakes;
        public Lake[] Lakes
        {
            get => lakes;
            set => lakes = value;
        }

        private River[] rivers;
        public River[] Rivers
        {
            get => rivers;
            set => rivers = value;
        }

        private BiomeGroup[] biomeGroups;
        public BiomeGroup[] BiomeGroups
        {
            get => biomeGroups;
            set => biomeGroups = value;
        }

        private Island[] islands;
        public Island[] Islands
        {
            get => islands;
            set => islands = value;
        }

        private Road[] roads;
        public Road[] Roads
        {
            get => roads;
            set => roads = value;
        }

        private Bridge[] bridges;
        public Bridge[] Bridges
        {
            get => bridges;
            set => bridges = value;
        }
        #endregion

        #endregion

        public River FindRiverAt(int x, int y, out int index)
        {
            index = 0;
            foreach (var river in rivers)
            {
                for (int i = 0; i < river.Nodes.Length; i++)
                {
                    if (river.Nodes[i].x == x && river.Nodes[i].y == y)
                    {
                        index = i;
                        return river;
                    }
                }
            }
            return null;
        }
    }
}