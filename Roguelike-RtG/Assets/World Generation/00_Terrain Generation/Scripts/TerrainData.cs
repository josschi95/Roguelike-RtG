using UnityEngine;

namespace JS.WorldMap
{
    [CreateAssetMenu(fileName = "Terrain Data", menuName = "World Generation/Terrain/Terrain Data")]
    public class TerrainData : ScriptableObject
    {
        private bool saveExists;
        public bool SaveExists
        {
            get => saveExists; 
            set => saveExists = value;
        }

        private int mapSize;
        public int MapSize
        {
            get => mapSize;
            set => mapSize = value;
        }

        private Vector3Int origin;
        public Vector3Int Origin
        {
            get => origin;
            set => origin = value;
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

        public void ClearData()
        {
            Mountains = null;
            Lakes = null;
            Rivers = null;
            BiomeGroups = null;
            Islands = null;
        }
    }
}