using JS.DomainSystem;
using UnityEngine;

namespace JS.CharacterSystem
{
    [CreateAssetMenu(menuName = "Characters/Presets/Preset SO")]
    public class CreaturePresetSO : ScriptableObject
    {
        [SerializeField] private string characterName;
        public string Name => characterName;

        [SerializeField] private CharacterRace race;
        public CharacterRace Race => race;

        [SerializeField] private bool isUndead;
        public bool IsUndead => isUndead;

        [SerializeField] private CharacterClass characterClass;
        public CharacterClass CharacterClass => characterClass;

        [SerializeField] private Domain domain;
        public Domain Domain => domain;

        [SerializeField] private CharacterGender characterGender;
        public CharacterGender CharacterGender => characterGender;
        
        [SerializeField] private int characterAge;
        [SerializeField] private AgeCategory ageCategory;
        public int CharacterAge => characterAge;

        [SerializeField] private AttributeReference[] attributeValues;
        public AttributeReference[] AttributeValues => attributeValues;

        [SerializeField] private SkillReference[] skillValues;
        public SkillReference[] SkillValues => skillValues;
        
        public CharacterSheet GetCharacterSheet()
        {
            var attributes = new int[attributeValues.Length];
            for (int i = 0; i < attributeValues.Length; i++)
            {
                attributes[i] = attributeValues[i].value;
            }

            var skills = new int[skillValues.Length];
            for (int i = 0; i < skillValues.Length; i++)
            {
                skills[i] = skillValues[i].value;
            }

            var sheet = new CharacterSheet
                (
                    characterName,
                    race,
                    characterClass,
                    attributes,
                    skills
                );

            return sheet;
        }
    }
}