using UnityEngine;

namespace JS.Combat
{
    [CreateAssetMenu(menuName = "Combat/Damage Type")]
    public class DamageType : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Color Color { get; private set; }
        [field: SerializeField] public bool isPhysical { get; private set; }

        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public string Symbol { get; private set; }
    }
}