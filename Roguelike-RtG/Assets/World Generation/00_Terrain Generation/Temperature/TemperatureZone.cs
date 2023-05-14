using UnityEngine;
using UnityEngine.Tilemaps;

namespace JS.WorldGeneration
{
    [CreateAssetMenu(fileName = "New Temperature Zone", menuName = "World Generation/Terrain/Temperature Zone")]
    public class TemperatureZone : ScriptableObject
    {
        [Range(0, 1)]
        [SerializeField] private float temperatureValue;
        public float TemperatureValue => temperatureValue;

        [field: SerializeField] public TileBase Tile;
    }
}