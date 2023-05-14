using UnityEngine;

namespace JS.CharacterSystem
{
    [CreateAssetMenu(menuName = "Characters/Stats/Race_Archetype")]
    public class CreatureType : ScriptableObject
    {
        [field: SerializeField] public int ID { get; private set; }
        [SerializeField] private string typeName; 
        [SerializeField] private bool canBeUndead = true;


        [Header("Needs")]
        [SerializeField] private bool needsAir = true;
        [SerializeField] private bool needsFood = true;
        [SerializeField] private bool needsSleep = true;

        public string TypeName => typeName;
        public bool NeedsAir => needsAir;
        public bool NeedsFood => needsFood;
        public bool NeedsSleep => needsSleep;
        public bool CanBeUndead => canBeUndead;

#pragma warning disable 0414
#if UNITY_EDITOR
        // Display notes field in the inspector.
        [Multiline, SerializeField, Space(10)]
        private string developerNotes = "";
#endif
#pragma warning restore 0414
    }
}