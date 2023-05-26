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
        [SerializeField] private SizeCategory raceSize;

        [Space]

        [SerializeField] private CreatureType type;
        [SerializeField] private CreatureSubType[] subtypes;

        [Space]

        [SerializeField] private Sprite raceSprite;
        [TextArea(3, 10)]
        [SerializeField] private string raceDescription;

        [Space]

        [Space]

        [SerializeField] private AttributeReference[] baseAttributes = new AttributeReference[6];

        [SerializeField] private AttributeReference[] attributePotentials = new AttributeReference[6];

        [SerializeField] private StatField racialStats;

        [Space]

        [SerializeField] private LifeExpectancy lifeExpectancy;

        [Space]

        [SerializeField] private bool hasMales = true;
        [SerializeField] private bool hasFemales = true;
        [SerializeField] private bool hasOther = true;


        public int ID => raceID;
        public string RaceName => raceName;
        public RacialCategory RaceCategory => raceCategory;
        public CreatureType Type => type;
        public CreatureSubType[] Subtypes => subtypes;
        public SizeCategory Size => raceSize;
        public Sprite RaceSprite => raceSprite;
        public string RaceDescription => raceDescription;
        public AttributeReference[] BaseAttributes => baseAttributes;
        public AttributeReference[] AttributePotentials => attributePotentials;
        public StatField RacialStats => racialStats;
        public LifeExpectancy LifeExpectancy => lifeExpectancy;
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

public enum RacialCategory { Humanoid, Demihuman, Monstrous }