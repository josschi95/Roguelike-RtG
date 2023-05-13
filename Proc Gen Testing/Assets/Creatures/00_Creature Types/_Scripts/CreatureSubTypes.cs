using UnityEngine;

namespace JS.CharacterSystem
{
    [CreateAssetMenu(menuName = "Characters/Stats/Race_Child Archetype")]
    public class CreatureSubTypes : CreatureType
    {
        [Space]

        [field: SerializeField] public CreatureType parentArchetype;
    }
}