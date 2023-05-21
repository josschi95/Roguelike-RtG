using UnityEngine;
using UnityEngine.Tilemaps;

namespace JS.WorldMap
{
    [CreateAssetMenu(fileName = "New Precipitation Zone", menuName = "World Generation/Terrain/Precipitation Zone")]
    public class PrecipitationZone : ScriptableObject
    {
        [Range(0, 1)]
        [SerializeField] private float precipitationValue;
        public float PrecipitationValue => precipitationValue;

        [field: SerializeField] public TileBase Tile;
    }
}