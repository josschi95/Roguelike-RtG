using UnityEngine;

namespace JS.CharacterSystem
{
    [CreateAssetMenu(menuName = "Characters/Stats/Size")]
    public class SizeCategory : ScriptableObject
    {
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public SizeCategory SizeDown { get; private set; }
        [field: SerializeField] public SizeCategory SizeUp { get; private set; }

        [Space] [Space]

        [Tooltip("Roll level dX to determine Max Hit Points")]
        [SerializeField] private int hitDie;
        [Tooltip("Added to Combat Maneuver Rolls, Subtracted from Dodge Value, -2X to Stealth")]
        [SerializeField] private int sizeModifier;

        public int HitDie => hitDie;
        public int SizeModifier => sizeModifier;

#pragma warning disable 0414
#if UNITY_EDITOR
        // Display notes field in the inspector.
        [Multiline, SerializeField, Space(10)]
        private string developerNotes = "";
#endif
#pragma warning restore 0414
    }
}