using UnityEngine;

namespace JS.CharacterSystem
{
    [CreateAssetMenu(menuName = "Characters/Age Category")]
    public class AgeCategory : ScriptableObject
    {
        [SerializeField] private int lifeStage;
        [SerializeField] private string _name;
        [TextArea(3, 5)]
        [SerializeField] private string message;

        [Space]

        [SerializeField] private AttributeReference[] ageModifiers;

        public int LifeStage => lifeStage;
        public string Name => _name;
        public string Message => message;
        public AttributeReference[] AgeModifiers => ageModifiers;


#pragma warning disable 0414
#if UNITY_EDITOR
        // Display notes field in the inspector.
        [Multiline, SerializeField, Space(10)]
        private string developerNotes = "";
#endif
#pragma warning restore 0414
    }
}