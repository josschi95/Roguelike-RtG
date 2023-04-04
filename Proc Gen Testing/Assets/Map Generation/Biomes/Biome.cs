using UnityEngine;

[CreateAssetMenu(fileName = "New Biome", menuName = "Scriptable Objects/Map/Biome")]
public class Biome : ScriptableObject
{
    [field: SerializeField] public BiomeTypes BiomeType { get; private set; }
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
    [Range(0,1)]
    [SerializeField] private float treeDensity;
    [Range(0,1)]
    [SerializeField] private float shrubDensity;
    [Range(0,1)]
    [SerializeField] private float grassDensity;


    //have an array of animals which would be present

    //and a separate array of monsters



    public bool FitsWhittakerModel(float avgAnnualTemp, float annualPrecipitation)
    {
        if (avgAnnualTemp < minAvgAnnualTemperature) return false;
        if (avgAnnualTemp > maxAvgAnnualTemperature) return false;
        if (annualPrecipitation < minAnnualPrecipitation) return false;
        if (annualPrecipitation > maxAnnualPrecipitation) return false;
        return true;
    }
}

public enum BiomeTypes
{
    BorealForest,
    Desert,
    Savanna,
    DeciduousForest,
    TemperateGrassland,
    //TemperateRainforest,
    TropicalSeasonalForest,
    TropicalRainforest,
    Tundra,
    Woodland
}