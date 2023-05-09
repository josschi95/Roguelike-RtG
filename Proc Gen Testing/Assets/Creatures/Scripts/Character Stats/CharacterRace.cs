using System.Collections.Generic;
using UnityEngine;

namespace JS.CharacterSystem
{
    [CreateAssetMenu(menuName = "Characters/Stats/Race")]
    public class CharacterRace : ScriptableObject
    {
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public string RaceName { get; private set; }
        [field: SerializeField] public Sprite RaceSprite { get; private set; }

        [TextArea(3, 10)]
        [SerializeField] private string raceDescription;

        [field: SerializeField] public RacialArchetype Archetype { get; private set; }

        [field: SerializeField] public RacialType RaceType { get; private set; }

        [field: SerializeField] public SizeCategory Size { get; private set; }

        [field: SerializeField] public StatField RacialStats { get; private set; }

        [field: SerializeField] public DiceCombo StartingAge { get; private set; }

        [field: SerializeField] public DiceCombo LifeSpan { get; private set; }
        [field: SerializeField] public CharacterRace[] ValidCrossBreeds { get; private set; }

        public string RaceDescription => raceDescription;

#pragma warning disable 0414
#if UNITY_EDITOR
        // Display notes field in the inspector.
        [Multiline, SerializeField, Space(10)]
        private string developerNotes = "";
#endif
#pragma warning restore 0414
    }
}

public enum RacialType { Humanoid, DemiHuman, Monstrous }