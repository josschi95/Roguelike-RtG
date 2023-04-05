using UnityEngine;

[CreateAssetMenu(fileName = "Map Features", menuName = "Scriptable Objects/Map/Map Features")]
public class MapFeatures : ScriptableObject
{
    [Header("Map Sizes")]

    [Header("Map Dimensions")]
    [SerializeField] private int tinyMapSize = 200;
    [SerializeField] private int smallMapSize = 400;
    [SerializeField] private int mediumMapSize = 800;
    [SerializeField] private int largeMapSize = 1500;
    [SerializeField] private int hugeMapSize = 3000;

    [Header("Tectonic Plate Count")]
    [SerializeField] private int tinyTectonicPlates = 4;
    [SerializeField] private int smallTectonicPlates = 6;
    [SerializeField] private int mediumTectonicPlates = 8;
    [SerializeField] private int largeTectonicPlates = 10;
    [SerializeField] private int hugeTectonicPlates = 12;

    [Header("Tectonic Plate Size")]
    [SerializeField] private int tinyPlateMinSize = 100;
    [SerializeField] private int tinyPlateMaxSize = 125;

    [SerializeField] private int smallPlateMinSize = 125;
    [SerializeField] private int smallPlateMaxSize = 150;

    [SerializeField] private int mediumPlateMinSize = 150;
    [SerializeField] private int mediumPlateMaxSize = 175;

    [SerializeField] private int largePlateMinSize = 175;
    [SerializeField] private int largePlateMaxSize = 200;

    [SerializeField] private int hugePlateMinSize = 200;
    [SerializeField] private int hugePlateMaxSize = 250;

    [Header("River Count")]
    [SerializeField] private int tinyRiverCount = 25;
    [SerializeField] private int smallRiverCount = 50;
    [SerializeField] private int mediumRiverCount = 75;
    [SerializeField] private int largeRiverCount = 100;
    [SerializeField] private int hugeRiverCount = 125;

    [Header("Rain Drop Iterations")]
    [SerializeField] private int tinyRainDrops = 40000;
    [SerializeField] private int smallRainDrops = 80000;
    [SerializeField] private int mediumRainDrops = 160000;
    [SerializeField] private int largeRainDrops = 320000;
    [SerializeField] private int hugeRainDrops = 640000;

    [Space]

    [Tooltip("Used to reference Sea Level")]
    [SerializeField] private TerrainType water;

    //MapDimensions
    public int MapSize(WorldSize s) => GetMapSize(s);
    private int GetMapSize(WorldSize size)
    {
        switch (size)
        {
            case WorldSize.Tiny: return tinyMapSize;
            case WorldSize.Small: return smallMapSize;
            case WorldSize.Medium: return mediumMapSize;
            case WorldSize.Large: return largeMapSize;
            case WorldSize.Huge: return hugeMapSize;
            default: return tinyMapSize;
        }
    }

    //Number of Tectonic Plates
    public int TectonicPlates(WorldSize s) => GetTectonicPlateCount(s);
    private int GetTectonicPlateCount(WorldSize size)
    {
        switch (size)
        {
            case WorldSize.Tiny: return tinyTectonicPlates;
            case WorldSize.Small: return smallTectonicPlates;
            case WorldSize.Medium: return mediumTectonicPlates;
            case WorldSize.Large: return largeTectonicPlates;
            case WorldSize.Huge: return hugeTectonicPlates;
            default: return tinyTectonicPlates;
        }
    }

    public int MinPlateSize(WorldSize s) => GetPlateMinSize(s);
    private int GetPlateMinSize(WorldSize size)
    {
        switch (size)
        {
            case WorldSize.Tiny: return tinyPlateMinSize;
            case WorldSize.Small: return smallPlateMinSize;
            case WorldSize.Medium: return mediumPlateMinSize;
            case WorldSize.Large: return largePlateMinSize;
            case WorldSize.Huge: return hugePlateMinSize;
            default: return tinyPlateMinSize;
        }
    }

    public int MaxPlateSize(WorldSize s) => GetPlateMaxSize(s);
    private int GetPlateMaxSize(WorldSize size)
    {
        switch (size)
        {
            case WorldSize.Tiny: return tinyPlateMaxSize;
            case WorldSize.Small: return smallPlateMaxSize;
            case WorldSize.Medium: return mediumPlateMaxSize;
            case WorldSize.Large: return largePlateMaxSize;
            case WorldSize.Huge: return hugePlateMaxSize;
            default: return tinyPlateMaxSize;
        }
    }

    //Number of Rivers to create
    public int RiverCount(WorldSize s) => GetRiverCount(s);
    private int GetRiverCount(WorldSize size)
    {
        switch (size)
        {
            case WorldSize.Tiny: return tinyRiverCount;
            case WorldSize.Small: return smallRiverCount;
            case WorldSize.Medium: return mediumRiverCount;
            case WorldSize.Large: return largeRiverCount;
            case WorldSize.Huge: return hugeRiverCount;
            default: return tinyRiverCount;
        }
    }

    //Rain Drop Algorith Iterations
    public int Raindrops(WorldSize s) => GetRainDropIterations(s);
    private int GetRainDropIterations(WorldSize size)
    {
        switch (size)
        {
            case WorldSize.Tiny: return tinyRainDrops;
            case WorldSize.Small: return smallRainDrops;
            case WorldSize.Medium: return mediumRainDrops;
            case WorldSize.Large: return largeRainDrops;
            case WorldSize.Huge: return hugeRainDrops;
            default: return tinyRainDrops;
        }
    }

    public float SeaLevel => water.Height;
}
