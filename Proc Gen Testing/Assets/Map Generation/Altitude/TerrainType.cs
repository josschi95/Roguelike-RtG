using UnityEngine;

[CreateAssetMenu(fileName = "New Terrain", menuName = "Scriptable Objects/Map/Terrain Type")]
public class TerrainType : ScriptableObject
{
    [field: SerializeField] public string TerrainName { get; private set; }
    [Range(0, 1)]
    [SerializeField] private float height;
    [field: SerializeField] public RuleTile RuleTile { get; private set; }
    [field: SerializeField] public TerrainLayer layer { get; private set; }
    [field: SerializeField] public bool isLand { get; private set; }

    public float Height => height;
}

public enum TerrainLayer { Ground, Hill, Mountain }