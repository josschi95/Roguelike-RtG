using UnityEngine;

namespace JS.CharacterSystem
{
    [CreateAssetMenu(menuName = "Characters/Stats/Class Archetype")]
    public class ClassArchetype : ScriptableObject
    {
        [field: SerializeField] public string ClassName { get; private set; }
        [field: SerializeField] public StatField ArchetypeStas { get; private set; }

#pragma warning disable 0414
#if UNITY_EDITOR
        // Display notes field in the inspector.
        [Multiline, SerializeField, Space(10)]
        private string developerNotes = "";
#endif
#pragma warning restore 0414
    }
}