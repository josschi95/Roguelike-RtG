using UnityEngine;

namespace JS.CharacterSystem
{
    [CreateAssetMenu(menuName = "Characters/Stats/Attribute")]
    public class CharacterAttribute : ScriptableObject
    {
        [SerializeField] private int id;
        public int ID => id;

        [SerializeField] private string _name;
        public string Name => _name;
        [SerializeField] private string shortName;
        public string ShortName => shortName;

        [SerializeField] private int minValue;
        public int MinValue => minValue;
        [SerializeField] private int maxValue;
        public int MaxValue => maxValue;

        [field: SerializeField] public string Description { get; private set; }



#pragma warning disable 0414
#if UNITY_EDITOR
        // Display notes field in the inspector.
        [Multiline, SerializeField, Space(10)]
        private string developerNotes = "";
#endif
#pragma warning restore 0414
    }
}

public enum Attributes
{
    Strength,
    Agility,
    Vitality,
    Knowledge,
    Willpower,
    Charisma,
}