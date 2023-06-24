using UnityEngine;
using UnityEngine.Tilemaps;

namespace JS.WorldMap
{
    [CreateAssetMenu(fileName = "New Precipitation Zone", menuName = "World Generation/Terrain/Precipitation Zone")]
    public class PrecipitationZone : ScriptableObject
    {
        [field: SerializeField] public int ID { get; private set; }

        [Space]

        [Range(0, 1)]
        [SerializeField] private float precipitationValue;
        public float PrecipitationValue => precipitationValue;

        [field: SerializeField] public TileBase Tile;
    }
}