using System.Collections.Generic;
using UnityEngine;
using JS.WorldGeneration;

[CreateAssetMenu(fileName = "New Tribe", menuName = "World Generation/Settlements/Tribe")]
public class HumanoidTribe : ScriptableObject
{
    [Range(0,1)]
    [SerializeField] private float mountainPreference = 0f; 
    [Range(0,1)]
    [SerializeField] private float islandPreference = 0f;

    [field: SerializeField] public List<Biome> preferredBiomes { get; private set; } = new List<Biome>();
    [field: SerializeField] public List<Biome> opposedBiomes { get; private set; } = new List<Biome>();

    public float MountainPreference => mountainPreference;
    public float IslandPreference => islandPreference;

    private void OnValidate()
    {
        if (mountainPreference + islandPreference > 1)
        {
            Debug.LogWarning("Mountain Preference and Island Preference cannot exceed a total of 1");
        }
    }
}
