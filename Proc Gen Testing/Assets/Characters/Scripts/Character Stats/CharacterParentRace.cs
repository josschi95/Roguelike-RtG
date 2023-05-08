using UnityEngine;

namespace JS.CharacterSystem
{
    [CreateAssetMenu(menuName = "Characters/Stats/Parent Race")]
    public class CharacterParentRace : ScriptableObject
    {
        [field: SerializeField] public string ParentRaceName { get; private set; }
        [field: SerializeField] public RacialArchetype Archetype { get; private set; }
        [field: SerializeField] public SizeCategory Size { get; private set; }
        [field: SerializeField] public StatField ParentStats { get; private set; }


#pragma warning disable 0414
#if UNITY_EDITOR
        // Display notes field in the inspector.
        [Multiline, SerializeField, Space(10)]
        private string developerNotes = "";
#endif
#pragma warning restore 0414
    }
}