using UnityEngine;

namespace JS.CharacterSystem
{
    [CreateAssetMenu(menuName = "Characters/Stats/Race_Archetype")]
    public class CreatureType : ScriptableObject
    {
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public string ArchetypeName { get; private set; }
        [field: SerializeField] public bool NeedsAir { get; private set; } = true;
        [field: SerializeField] public bool NeedsFood { get; private set; } = true;
        [field: SerializeField] public bool NeedsSleep { get; private set; } = true;

#pragma warning disable 0414
#if UNITY_EDITOR
        // Display notes field in the inspector.
        [Multiline, SerializeField, Space(10)]
        private string developerNotes = "";
#endif
#pragma warning restore 0414
    }
}