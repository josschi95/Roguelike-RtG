using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JS.CharacterSystem
{
    [CreateAssetMenu(menuName = "Characters/Stats/Race")]
    public class CharacterRace : ScriptableObject
    {
        [SerializeField] private int raceID;
        [SerializeField] private string raceName;

        [Space]

        [SerializeField] private RacialCategory raceCategory;
        [SerializeField] private CreatureType raceArchetype;
        [SerializeField] private SizeCategory raceSize;

        [Space]

        [SerializeField] private Sprite raceSprite;
        [TextArea(3, 10)]
        [SerializeField] private string raceDescription;

        [Space]

        [SerializeField] private StatField racialStats;

        [Space]

        [SerializeField] private DiceCombo startingAge;
        [SerializeField] private DiceCombo lifespan;
        [SerializeField] private AgeRanges[] ageRanges;

        [Space]

        [SerializeField] private CharacterRace[] validCrossBreeds;

        [Space]

        [SerializeField] private bool hasMales = true;
        [SerializeField] private bool hasFemales = true;
        [SerializeField] private bool hasOther = true;



        public int ID => raceID;
        public string RaceName => raceName;
        public RacialCategory RaceCategory => raceCategory;
        public CreatureType Archetype => raceArchetype;
        public SizeCategory Size => raceSize;
        public Sprite RaceSprite => raceSprite;
        public string RaceDescription => raceDescription;
        public StatField RacialStats => racialStats;
        public DiceCombo StartingAge => startingAge;
        public DiceCombo LifeSpan => lifespan;
        public AgeRanges[] AgeRanges => ageRanges;
        public CharacterRace[] ValidCrossBreeds => validCrossBreeds;
        public bool HasMales => hasMales;
        public bool HasFemale => hasFemales;
        public bool HasOther => hasOther;


#pragma warning disable 0414
#if UNITY_EDITOR
        // Display notes field in the inspector.
        [Multiline, SerializeField, Space(10)]
        private string developerNotes = "";
#endif
#pragma warning restore 0414
    }
}

public enum RacialCategory { Humanoid, DemiHuman, Monstrous }