using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Temperature Zone", menuName = "Scriptable Objects/Map/Temperature Zone")]
public class TemperatureZone : ScriptableObject
{
    [Range(0, 1)]
    [SerializeField] private float temperatureValue;
    public float TemperatureValue => temperatureValue;

    [field: SerializeField] public TileBase Tile;
}