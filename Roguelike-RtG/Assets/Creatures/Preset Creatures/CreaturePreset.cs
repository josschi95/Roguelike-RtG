using UnityEngine;

namespace JS.CharacterSystem
{
    [CreateAssetMenu(menuName = "Characters/Preset")]
    public class CreaturePreset : ScriptableObject
    {
        [SerializeField] private string characterName;
        [SerializeField] private Gender characterGender;
        [SerializeField] private int characterAge;
        [SerializeField] private AgeCategory ageCategory;
        [SerializeField] private CharacterRace primaryRace;
        [SerializeField] private CharacterRace secondaryRace;
        [SerializeField] private bool isUndead = false;
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private AttributeReference[] attributeValues;
        [SerializeField] private SkillReference[] skillValues;
        
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
                    primaryRace,
                    secondaryRace,
                    characterClass,
                    attributes,
                    skills
                );

            return sheet;
        }
    }
}