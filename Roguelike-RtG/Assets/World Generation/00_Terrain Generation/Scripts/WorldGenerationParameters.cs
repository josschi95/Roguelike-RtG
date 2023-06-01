using UnityEngine;

namespace JS.WorldMap
{
    [CreateAssetMenu(fileName = "Map Features", menuName = "World Generation/Terrain/Map Features")]
    public class WorldGenerationParameters : ScriptableObject
    {
        [field: SerializeField] public float SeaLevel { get; private set; } = 0.4f;
        [field: SerializeField] public float MountainHeight { get; private set; } = 0.8f;
        [field: SerializeField] public Vector2Int RegionDimensions { get; private set; } = new Vector2Int(3, 3);
        [field: SerializeField] public Vector2Int LocalDimensions { get; private set; } = new Vector2Int(100, 100);
        //[field: SerializeField] public int MaxStartingSettlementSize { get; private set; } = 750;

        [Header("Map Sizes")]
        [SerializeField] private WorldFeatures tinyMap;
        [SerializeField] private WorldFeatures smallMap;
        [SerializeField] private WorldFeatures mediumMap;
        [SerializeField] private WorldFeatures largeMap;
        [SerializeField] private WorldFeatures hugeMap;

        [Header("Temperature")]
        [SerializeField] private TemperatureZone[] temperatureZones;
        public TemperatureZone[] TemperatureZones => temperatureZones;

        [Header("Precipitation")]
        [SerializeField] private PrecipitationZone[] precipitationZones;
        public PrecipitationZone[] PrecipitationZones => precipitationZones;

        [Header("Resources")]
        [Range(0, 1)]
        [SerializeField] private float coalPrevalence = 0.4f;
        public float CoalPrevalence => coalPrevalence;
        [Range(0, 1)]
        [SerializeField] private float copperPrevalence = 0.35f;
        public float CopperPrevalence => copperPrevalence;
        [Range(0, 1)]
        [SerializeField] private float ironPrevalence = 0.3f;
        public float IronPrevalence => ironPrevalence;
        [Range(0, 1)]
        [SerializeField] private float silverPrevalence = 0.25f;
        public float SilverPrevalence => silverPrevalence;
        [Range(0, 1)]
        [SerializeField] private float goldPrevalence = 0.2f;
        public float GoldPrevalence => goldPrevalence;
        [Range(0, 1)]
        [SerializeField] private float gemstonesPrevalence = 0.15f;
        public float GemstonesPrevalence => gemstonesPrevalence;
        [Range(0, 1)]
        [SerializeField] private float mithrilPrevalence = 0.1f;
        public float MithrilPrevalence => mithrilPrevalence;
        [Range(0, 1)]
        [SerializeField] private float adamantinePrevalence = 0.05f;
        public float AdamantinePrevalence => adamantinePrevalence;

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
        public int[] SettlementCount(WorldSize s) => GetSettlementCounts(s);
        private int[] GetSettlementCounts(WorldSize size)
        {
            int[] settlements = new int[4]
            {
                GetCityCount(size),
                GetTownCount(size),
                GetVillageCount(size),
                GetHamletCount(size),
            };

            return settlements;
        }

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

        public int HamletCount(WorldSize s) => GetHamletCount(s);
        private int GetHamletCount(WorldSize size)
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
