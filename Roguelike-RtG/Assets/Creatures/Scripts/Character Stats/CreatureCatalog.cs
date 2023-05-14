using UnityEngine;

namespace JS.CharacterSystem
{
    [CreateAssetMenu(menuName = "Characters/Creature Catalog")]
    public class CreatureCatalog : ScriptableObject
    {
        [SerializeField] private CreatureType[] creatureTypes;

        [Space]

        [SerializeField] private CreatureSubType[] creatureSubtypes;

        [Space]

        [SerializeField] private CharacterRace[] races;
        public CharacterRace[] Races => races;

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