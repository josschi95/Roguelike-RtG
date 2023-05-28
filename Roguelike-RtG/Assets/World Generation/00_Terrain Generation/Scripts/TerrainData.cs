using UnityEngine;

namespace JS.WorldMap
{
    [CreateAssetMenu(fileName = "Terrain Data", menuName = "World Generation/Terrain/Terrain Data")]
    public class TerrainData : ScriptableObject
    {
        [SerializeField] private Biome[] biomes;

        public Biome GetBiome(int id)
        {
            for (int i = 0; i < biomes.Length; i++)
            {
                if (biomes[i].ID == id) return biomes[i];
            }
            throw new System.Exception("Biome ID not valid.");
        }

        public Biome GetBiome(int x, int y)
        {
            return GetBiome(biomeMap[x, y]);
        }

        public void ClearData()
        {
            Mountains = null;
            Lakes = null;
            Rivers = null;
            BiomeGroups = null;
            Islands = null;
        }

        #region - Map Data -
        private int mapSize;
        public int MapSize
        {
            get => mapSize;
            set => mapSize = value;
        }

        private int originX;
        private int originY;
        public int OriginX
        {
            get => originX;
            set => originX = value;
        }
        public int OriginY
        {
            get => originY;
            set => originY = value;
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
        #endregion
    }
}