using UnityEngine;

namespace JS.WorldGeneration
{
    [CreateAssetMenu(fileName = "Map Features", menuName = "World Generation/Terrain/Map Features")]
    public class WorldGenerationParameters : ScriptableObject
    {
        [field: SerializeField] public float SeaLevel { get; private set; } = 0.4f;
        [field: SerializeField] public float MountainHeight { get; private set; } = 0.8f;
        [field: SerializeField] public int MaxIslandSize { get; private set; } = 250;

        [Header("Map Sizes")]
        [SerializeField] private WorldFeatures tinyMap;
        [SerializeField] private WorldFeatures smallMap;
        [SerializeField] private WorldFeatures mediumMap;
        [SerializeField] private WorldFeatures largeMap;
        [SerializeField] private WorldFeatures hugeMap;

        [Header("Altitude")]
        [SerializeField] private AltitudeZone[] altitudeZones;
        public AltitudeZone[] AltitudeZones => altitudeZones;

        [Header("Temperature")]
        [SerializeField] private TemperatureZone[] temperatureZones;
        public TemperatureZone[] TemperatureZones => temperatureZones;

        [Header("Precipitation")]
        [SerializeField] private PrecipitationZone[] precipitationZones;
        public PrecipitationZone[] PrecipitationZones => precipitationZones;

        [Header("Biomes")]
        [SerializeField] private Biome[] biomes;
        public Biome[] Biomes => biomes;

        #region - Terrain Generation -
        //MapDimensions
        public int MapSize(WorldSize s) => GetMapSize(s);
        private int GetMapSize(WorldSize size)
        {
            switch (size)
            {
                case WorldSize.Tiny: return tinyMap.mapSize;
                case WorldSize.Small: return smallMap.mapSize;
                case WorldSize.Medium: return mediumMap.mapSize;
                case WorldSize.Large: return largeMap.mapSize;
                case WorldSize.Huge: return hugeMap.mapSize;
                default: return tinyMap.mapSize;
            }
        }

        //Number of Tectonic Plates
        public int TectonicPlates(WorldSize s) => GetTectonicPlateCount(s);
        private int GetTectonicPlateCount(WorldSize size)
        {
            switch (size)
            {
                case WorldSize.Tiny: return tinyMap.tectonicPlateCount;
                case WorldSize.Small: return smallMap.tectonicPlateCount;
                case WorldSize.Medium: return mediumMap.tectonicPlateCount;
                case WorldSize.Large: return largeMap.tectonicPlateCount;
                case WorldSize.Huge: return hugeMap.tectonicPlateCount;
                default: return tinyMap.tectonicPlateCount;
            }
        }

        public int MinPlateSize(WorldSize s) => GetPlateMinSize(s);
        private int GetPlateMinSize(WorldSize size)
        {
            switch (size)
            {
                case WorldSize.Tiny: return tinyMap.tectonicPlateMinSize;
                case WorldSize.Small: return smallMap.tectonicPlateMinSize;
                case WorldSize.Medium: return mediumMap.tectonicPlateMinSize;
                case WorldSize.Large: return largeMap.tectonicPlateMinSize;
                case WorldSize.Huge: return hugeMap.tectonicPlateMinSize;
                default: return tinyMap.tectonicPlateMinSize;
            }
        }

        public int MaxPlateSize(WorldSize s) => GetPlateMaxSize(s);
        private int GetPlateMaxSize(WorldSize size)
        {
            switch (size)
            {
                case WorldSize.Tiny: return tinyMap.tectonicPlateMaxSize;
                case WorldSize.Small: return smallMap.tectonicPlateMaxSize;
                case WorldSize.Medium: return mediumMap.tectonicPlateMaxSize;
                case WorldSize.Large: return largeMap.tectonicPlateMaxSize;
                case WorldSize.Huge: return hugeMap.tectonicPlateMaxSize;
                default: return tinyMap.tectonicPlateMaxSize;
            }
        }

        //Number of Rivers to create
        public int RiverCount(WorldSize s) => GetRiverCount(s);
        private int GetRiverCount(WorldSize size)
        {
            switch (size)
            {
                case WorldSize.Tiny: return tinyMap.riverCount;
                case WorldSize.Small: return smallMap.riverCount;
                case WorldSize.Medium: return mediumMap.riverCount;
                case WorldSize.Large: return largeMap.riverCount;
                case WorldSize.Huge: return hugeMap.riverCount;
                default: return tinyMap.riverCount;
            }
        }

        //Rain Drop Algorith Iterations
        public int Raindrops(WorldSize s) => GetRainDropIterations(s);
        private int GetRainDropIterations(WorldSize size)
        {
            switch (size)
            {
                case WorldSize.Tiny: return tinyMap.rainDrops;
                case WorldSize.Small: return smallMap.rainDrops;
                case WorldSize.Medium: return mediumMap.rainDrops;
                case WorldSize.Large: return largeMap.rainDrops;
                case WorldSize.Huge: return hugeMap.rainDrops;
                default: return tinyMap.rainDrops;
            }
        }
        #endregion

        #region - Settlement Generation -
        public int CityCount(WorldSize s) => GetCityCount(s);
        private int GetCityCount(WorldSize size)
        {
            switch (size)
            {
                case WorldSize.Tiny: return tinyMap.citiesPerTribe;
                case WorldSize.Small: return smallMap.citiesPerTribe;
                case WorldSize.Medium: return mediumMap.citiesPerTribe;
                case WorldSize.Large: return largeMap.citiesPerTribe;
                case WorldSize.Huge: return hugeMap.citiesPerTribe;
                default: return tinyMap.citiesPerTribe;
            }
        }

        public int TownCount(WorldSize s) => GetTownCount(s);
        private int GetTownCount(WorldSize size)
        {
            switch (size)
            {
                case WorldSize.Tiny: return tinyMap.townsPerTribe;
                case WorldSize.Small: return smallMap.townsPerTribe;
                case WorldSize.Medium: return mediumMap.townsPerTribe;
                case WorldSize.Large: return largeMap.townsPerTribe;
                case WorldSize.Huge: return hugeMap.townsPerTribe;
                default: return tinyMap.townsPerTribe;
            }
        }

        public int VillageCount(WorldSize s) => GetVillageCount(s);
        private int GetVillageCount(WorldSize size)
        {
            switch (size)
            {
                case WorldSize.Tiny: return tinyMap.villagesPerTribe;
                case WorldSize.Small: return smallMap.villagesPerTribe;
                case WorldSize.Medium: return mediumMap.villagesPerTribe;
                case WorldSize.Large: return largeMap.villagesPerTribe;
                case WorldSize.Huge: return hugeMap.villagesPerTribe;
                default: return tinyMap.villagesPerTribe;
            }
        }

        public int TribeCount(WorldSize s) => GetTribeCount(s);
        private int GetTribeCount(WorldSize size)
        {
            switch (size)
            {
                case WorldSize.Tiny: return tinyMap.hamletsPerTribe;
                case WorldSize.Small: return smallMap.hamletsPerTribe;
                case WorldSize.Medium: return mediumMap.hamletsPerTribe;
                case WorldSize.Large: return largeMap.hamletsPerTribe;
                case WorldSize.Huge: return hugeMap.hamletsPerTribe;
                default: return tinyMap.hamletsPerTribe;
            }
        }
        #endregion

    }
}
