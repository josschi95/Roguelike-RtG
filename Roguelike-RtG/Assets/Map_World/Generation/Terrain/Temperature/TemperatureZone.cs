using UnityEngine;

namespace JS.World.Map.Climate
{
    [CreateAssetMenu(fileName = "New Temperature Zone", menuName = "World Generation/Terrain/Temperature Zone")]
    public class TemperatureZone : ScriptableObject
    {
        [field: SerializeField] public int ID { get; private set; }

        [Space]

        [Range(0, 1)]
        [SerializeField] private float temperatureValue;
        public float TemperatureValue => temperatureValue;

        [field: SerializeField] public Color HighlightColor { get; private set; }
    }
}