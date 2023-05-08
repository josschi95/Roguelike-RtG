using UnityEngine;

namespace JS.CharacterSystem
{
    [CreateAssetMenu(menuName = "Characters/Stats/Racial Archetype")]
    public class RacialArchetype : ScriptableObject
    {
        [field: SerializeField] public string ArchetypeName { get; private set; }

        [field: SerializeField] public bool needsAir { get; private set; } = true;
        [field: SerializeField] public bool needsFood { get; private set; } = true;
        [field: SerializeField] public bool needsSleep { get; private set; } = true;

#pragma warning disable 0414
#if UNITY_EDITOR
        // Display notes field in the inspector.
        [Multiline, SerializeField, Space(10)]
        private string developerNotes = "";
#endif
#pragma warning restore 0414
    }
}