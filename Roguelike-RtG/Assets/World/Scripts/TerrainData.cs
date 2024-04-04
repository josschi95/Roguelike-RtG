using System.Collections.Generic;

namespace JS.World.Map.Features
{
    /// <summary>
    /// Class to hold active data about the world map.
    /// Altitude, Temp, Moisture, Biomes, Coasts, Resources, Features
    /// </summary>
    public static class TerrainData
    {
        #region - Map Data -
        private static int mapSize;
        public static int MapSize
        {
            get => mapSize;
            set => mapSize = value;
        }

        private static int northLatitude;
        private static int southLatitude;

        public static int NorthLatitude
        {
            get => northLatitude;
            set => northLatitude = value;
        }
        public static int SouthLatitude
        {
            get => southLatitude;
            set => southLatitude = value;
        }


        private static int[,] seedMap;
        public static int[,] SeedMap
        {
            get => seedMap;
            set => seedMap = value;
        }

        private static HashSet<WorldTile> plateBorders;
        public static HashSet<WorldTile> PlateBorders
        {
            get => plateBorders;
            set => plateBorders = value;
        }

        //Elevation
        private static float[,] heightMap;
        public static float[,] HeightMap
        {
            get => heightMap;
            set => heightMap = value;
        }

        //Temperature
        private static float[,] heatMap;
        public static float[,] HeatMap
        {
            get => heatMap;
            set => heatMap = value;
        }

        //Moisture
        private static float[,] moistureMap;
        public static float[,] MoistureMap
        {
            get => moistureMap;
            set => moistureMap = value;
        }

        //Biomes
        private static int[,] biomeMap;
        public static int[,] BiomeMap
        {
            get => biomeMap;
            set => biomeMap = value;
        }

        //Coasts
        private static bool[,] coasts;
        public static bool[,] Coasts
        {
            get => coasts;
            set => coasts = value;
        }

        #region - Resources -
        //Resources
        private static float[,] coalMap;
        public static float[,] CoalMap
        {
            get => coalMap;
            set => coalMap = value;
        }
        private static float[,] copperMap;
        public static float[,] CopperMap
        {
            get => copperMap;
            set => copperMap = value;
        }
        private static float[,] ironMap;
        public static float[,] IronMap
        {
            get => ironMap;
            set => ironMap = value;
        }
        private static float[,] silverMap;
        public static float[,] SilverMap
        {
            get => silverMap;
            set => silverMap = value;
        }
        private static float[,] goldMap;
        public static float[,] GoldMap
        {
            get => goldMap;
            set => goldMap = value;
        }
        private static float[,] mithrilMap;
        public static float[,] MithrilMap
        {
            get => mithrilMap;
            set => mithrilMap = value;
        }
        private static float[,] adamantineMap;
        public static float[,] AdmanatineMap
        {
            get => adamantineMap;
            set => adamantineMap = value;
        }
        private static float[,] gemstoneMap;
        public static float[,] GemstoneMap
        {
            get => gemstoneMap;
            set => gemstoneMap = value;
        }
        #endregion

        #region - Terrain Features -
        private static MountainRange[] mountains;
        public static MountainRange[] Mountains
        {
            get => mountains;
            set => mountains = value;
        }

        private static Lake[] lakes;
        public static Lake[] Lakes
        {
            get => lakes;
            set => lakes = value;
        }

        private static River[] rivers;
        public static River[] Rivers
        {
            get => rivers;
            set => rivers = value;
        }

        private static BiomeGroup[] biomeGroups;
        public static BiomeGroup[] BiomeGroups
        {
            get => biomeGroups;
            set => biomeGroups = value;
        }

        private static LandMass[] land;
        public static LandMass[] LandMasses
        {
            get => land;
            set => land = value;
        }

        private static Road[] roads;
        public static Road[] Roads
        {
            get => roads;
            set => roads = value;
        }

        private static Bridge[] bridges;
        public static Bridge[] Bridges
        {
            get => bridges;
            set => bridges = value;
        }
        #endregion

        #endregion

        public static River FindRiverAt(int x, int y, out int index)
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

        public static bool HasMinerals(int x, int y)
        {
            if (coalMap[x,y] > 0) return true;
            if (copperMap[x,y] > 0) return true;
            if (ironMap[x,y] > 0) return true;
            if (silverMap[x,y] > 0) return true;
            if (goldMap[x,y] > 0) return true;
            if (mithrilMap[x,y] > 0) return true;
            if (adamantineMap[x,y] > 0) return true;
            if (gemstoneMap[x,y] > 0) return true;
            return false;
        }
    }
}