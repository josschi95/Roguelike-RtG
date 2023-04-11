using UnityEngine;

[System.Serializable]
public class WorldFeatures
{
    [Header("Terrain")]
    public int mapSize;
    [Space]    
    public int tectonicPlateCount;
    public int tectonicPlateMinSize;
    public int tectonicPlateMaxSize;
    [Space]
    public int riverCount;
    [Space]
    public int rainDrops;

    [Header("Settlements")]
    public int minSettlements;
    public int maxSettlements;

    [Space]

    public int hamletsPerTribe;
    public int villagesPerTribe;
    public int townsPerTribe;
    public int citiesPerTribe;
}
