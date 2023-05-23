using System.Collections.Generic;
using UnityEngine;
using JS.WorldMap;

[CreateAssetMenu(fileName = "New Tribe", menuName = "World Generation/Settlements/Tribe")]
public class HumanoidTribe : ScriptableObject
{
    [field: SerializeField] public int ID { get; private set; }

    [field: SerializeField] public List<Biome> preferredBiomes { get; private set; } = new List<Biome>();
    [field: SerializeField] public List<Biome> opposedBiomes { get; private set; } = new List<Biome>();

    [field: SerializeField] public bool PrefersMountains { get; private set; }
    [field: SerializeField] public bool PrefersIslands { get; private set; }
}