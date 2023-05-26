using UnityEngine;

namespace JS.CharacterSystem.Creation
{
    [CreateAssetMenu(menuName = "Characters/Presets/Custom Presets")]
    public class CustomCharacterPresets : ScriptableObject
    {
        public CreaturePresetData[] presets;
    }
}