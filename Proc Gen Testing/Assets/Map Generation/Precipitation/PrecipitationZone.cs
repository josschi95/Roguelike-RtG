using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Precipitation Zone", menuName = "Scriptable Objects/Map/Precipitation Zone")]
public class PrecipitationZone : ScriptableObject
{
    [Range(0, 1)]
    [SerializeField] private float precipitationValue;
    public float PrecipitationValue => precipitationValue;

    [field: SerializeField] public TileBase Tile;
}
