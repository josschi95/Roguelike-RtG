using UnityEngine;

namespace JS.CharacterSystem
{
    [CreateAssetMenu(menuName = "Characters/Stats/Character Class")]
    public class CharacterClass : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private ClassArchetype archetype;
        [SerializeField] private ClassTier tier;

        [Space]

        [SerializeField] private StatField classStats;

        [Space]

        [TextArea(3,5)]
        [SerializeField] private string description;

        [Header("Requirements")]
        [SerializeField] private AttributeReference[] requiredAttributeScores;
        [SerializeField] private SkillReference[] requiredSkillScores;
        [SerializeField] private ClassReference[] requiredClassLevels;
        [SerializeField] private CharacterRace requiredRace;

        public string ClassName => _name;
        public ClassArchetype Archetype => archetype;
        public ClassTier Tier => tier;
        public StatField ClassStats => classStats;
        public string Description => description;

        public bool MeetsRequirements()
        {
            //if does not match race

            int score = 0;
            for (int i = 0; i < requiredAttributeScores.Length; i++)
            {
                if (score < requiredAttributeScores[i].value) return false;
            }
            for (int i = 0; i < requiredSkillScores.Length; i++)
            {
                if (score < requiredSkillScores[i].value) return false;
            }
            for (int i = 0; i < requiredClassLevels.Length; i++)
            {
                //if doesn't have levels or not high enough
            }


            return true;
        }

#pragma warning disable 0414
#if UNITY_EDITOR
        // Display notes field in the inspector.
        [Multiline, SerializeField, Space(10)]
        private string developerNotes = "";
#endif
#pragma warning restore 0414
    }
}

public enum ClassArchetype { Martial, Mage, Artisan, Racial }