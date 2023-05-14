using UnityEngine;

namespace JS.WorldGeneration
{
    [CreateAssetMenu(fileName = "New Biome", menuName = "World Generation/Terrain/Biome")]
    public class Biome : ScriptableObject
    {
        [field: SerializeField] public string BiomeName { get; private set; }
        [field: SerializeField] public RuleTile RuleTile { get; private set; }

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

        [Header("Vegetation")]
        [Range(0, 1)]
        [SerializeField] private float treeDensity;
        [Range(0, 1)]
        [SerializeField] private float shrubDensity;
        [Range(0, 1)]
        [SerializeField] private float grassDensity;

        [Header("Resources")]
        [Range(0, 1)]
        [SerializeField] private float foodAvailability;
        [Range(0, 1)]
        [SerializeField] private float waterAvailability;
        [Range(0, 1)]
        [SerializeField] private float lumberAvailability;

        [Space][Space]

        //have an array of animals which would be present
        [SerializeField] private string[] vegetation;
        [SerializeField] private string[] animals;
        //and a separate array of monsters
        [SerializeField] private string[] monsters;

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