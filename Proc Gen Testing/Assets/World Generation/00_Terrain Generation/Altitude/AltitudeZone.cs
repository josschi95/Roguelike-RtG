using UnityEngine;

namespace JS.WorldGeneration
{
    [CreateAssetMenu(fileName = "New Atltitude Zone", menuName = "World Generation/Terrain/Altitude Zone")]
    public class AltitudeZone : ScriptableObject
    {
        [field: SerializeField] public string ZoneName;
        [Range(0, 1)]
        [SerializeField] private float height;
        [field: SerializeField] public RuleTile RuleTile { get; private set; }
        [field: SerializeField] public bool isLand { get; private set; }
        [field: SerializeField] public bool isMountain { get; private set; }

        public float Height => height;
    }
}