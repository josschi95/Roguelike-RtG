using UnityEngine;

namespace JS.WorldMap
{
    [CreateAssetMenu(fileName = "New Biome", menuName = "World Generation/Terrain/Biome")]
    public class Biome : ScriptableObject
    {
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public string BiomeName { get; private set; }
        [field: SerializeField] public RuleTile WorldBase { get; private set; }
        [field: SerializeField] public RuleTile WorldAccent { get; private set; }
        [field: SerializeField] public RuleTile LocalTile { get; private set; }
        [field: SerializeField] public bool isLand { get; private set; } = true;
        //True for forested biomes and mountains
        [field: SerializeField] public bool isDifficultTerrain { get; private set; }
        [field: SerializeField] public bool CanBeHunted { get; private set; }
        [field: SerializeField] public int DangerModifier { get; private set; }
        [Header("Temperature and Precipitation")]
        [SerializeField] private int minAvgAnnualTemperature;
        [SerializeField] private int maxAvgAnnualTemperature;
        [Space]
        [SerializeField] private int minAnnualPrecipitation;
        [SerializeField] private int maxAnnualPrecipitation;

        public int MinAvgTemp => minAvgAnnualTemperature;
        public int MaxAvgTemp => maxAvgAnnualTemperature;
        public int MinPrecipitation => minAnnualPrecipitation;
        public int MaxPrecipitation => maxAnnualPrecipitation;

        /*[Header("Vegetation")]
        [Range(0, 1)]
        [SerializeField] private float treeDensity;
        [Range(0, 1)]
        [SerializeField] private float shrubDensity;
        [Range(0, 1)]
        [SerializeField] private float grassDensity;*/
        
        [Header("Resources")]
        [Range(0, 5)]
        [SerializeField] private int agricultureRating;
        [Range(0, 5)]
        [SerializeField] private int lumberRating;

#pragma warning disable 0414
#if UNITY_EDITOR
        // Display notes field in the inspector.
        [Multiline, SerializeField, Space(10)]
        private string developerNotes = "";
#endif
#pragma warning restore 0414

        public bool FitsWhittakerModel(float avgAnnualTemp, float annualPrecipitation)
        {
            if (avgAnnualTemp < minAvgAnnualTemperature) return false;
            if (avgAnnualTemp > maxAvgAnnualTemperature) return false;
            if (annualPrecipitation < minAnnualPrecipitation) return false;
            if (annualPrecipitation > maxAnnualPrecipitation) return false;
            return true;
        }
    }
}