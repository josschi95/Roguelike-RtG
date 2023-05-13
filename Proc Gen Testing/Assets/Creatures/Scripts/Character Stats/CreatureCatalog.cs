using UnityEngine;

namespace JS.CharacterSystem
{
    [CreateAssetMenu(menuName = "Characters/Creature Catalog")]
    public class CreatureCatalog : ScriptableObject
    {
        [SerializeField] private CreatureType[] creatureTypes;

        [Space]

        [SerializeField] private CreatureSubTypes[] creatureSubTypes;

        [Space]

        [SerializeField] private CharacterRace[] races;

        [Space]

        [SerializeField] private SizeCategory[] sizes;

        [Space]

        [SerializeField] private AgeCategory[] ages;

        [Space]

        [SerializeField] private CharacterAttribute[] primaryAttributes;
        [SerializeField] private CharacterAttribute[] secondaryAttributes;

        [Space]

        [SerializeField] private CharacterSkill[] skills;
    }
}