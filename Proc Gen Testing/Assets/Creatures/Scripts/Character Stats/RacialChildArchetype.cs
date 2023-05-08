using UnityEngine;

namespace JS.CharacterSystem
{
    [CreateAssetMenu(menuName = "Characters/Stats/Race_Child Archetype")]
    public class RacialChildArchetype : RacialArchetype
    {
        [Space]

        [field: SerializeField] public RacialArchetype parentArchetype;
    }
}