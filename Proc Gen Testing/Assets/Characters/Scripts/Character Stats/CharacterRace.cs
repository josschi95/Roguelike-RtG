using UnityEngine;

namespace JS.CharacterSystem
{
    [CreateAssetMenu(menuName = "Characters/Stats/Race")]
    public class CharacterRace : ScriptableObject
    {
        [field: SerializeField] public string RaceName { get; private set; }
        [field: SerializeField] public CharacterParentRace ParentRace { get; private set; }
        [field: SerializeField] public RacialArchetype Archetype { get; private set; }  
        [field: SerializeField] public SizeCategory Size { get; private set; }
        [field: SerializeField] public StatField RacialStats { get; private set; }
        [Space]

        [Tooltip("Modifier to the rate of gaining experience. Shorter lived races level up faster.")]
        [SerializeField][Range(0.5f, 5.0f)] private float experienceRate;
        public float ExperienceRate => experienceRate;

        //[field: SerializeField] public DiceCombo StartingAge { get; private set; }

        //[field: SerializeField] public DiceCombo LifeSpan { get; private set; }

#pragma warning disable 0414
#if UNITY_EDITOR
        // Display notes field in the inspector.
        [Multiline, SerializeField, Space(10)]
        private string developerNotes = "";
#endif
#pragma warning restore 0414
    }
}