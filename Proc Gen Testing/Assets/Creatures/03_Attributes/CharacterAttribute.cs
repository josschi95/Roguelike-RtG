using UnityEngine;

namespace JS.CharacterSystem
{
    [CreateAssetMenu(menuName = "Characters/Stats/Attribute")]
    public class CharacterAttribute : ScriptableObject
    {
        [field: SerializeField] public int ID { get; private set; }

        [field: SerializeField] public string FullName { get; private set; }
        [field: SerializeField] public string ShortName { get; private set; }

        [field: SerializeField] public bool PrimaryAttribute { get; private set; }

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