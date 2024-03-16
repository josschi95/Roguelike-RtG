using UnityEngine;
using System.IO;
using JS.CharacterSystem.Creation;
using System.Collections.Generic;

namespace JS.Architecture.CommandSystem
{
    /// <summary>
    /// A command that saves the currently made character from the CharacterCreation scene to a file
    /// </summary>
    [CreateAssetMenu(menuName = "Command System/Save Preset")]
    public class SaveNewPresetCommand : CommandBase
    {
        [Space]
        [Space]

        [SerializeField] private CustomCharacterPresets playerPresets;
        [SerializeField] private CharacterBuilder _characterBuilder;

        protected override bool ExecuteCommand()
        {
            SavePreset();
            return true;
        }

        private void SavePreset()
        {
            var data = new PresetSaveData();

            var presets = new List<CreaturePresetData>();
            presets.AddRange(playerPresets.presets);

            var newPreset = new CreaturePresetData();
            newPreset.presetName = _characterBuilder.CharacterName;
            newPreset.raceID = _characterBuilder.Race.ID;
            newPreset.classID = _characterBuilder.Class.ID;
            newPreset.domainID = _characterBuilder.Domain.ID;
            newPreset.gender = (int)_characterBuilder.CharacterGender;
            newPreset.age = _characterBuilder.CharacterAge;
            newPreset.isUndead = _characterBuilder.IsUndead;

            newPreset.attributeValues = new int[_characterBuilder.AttributeValues.Length];
            for (int i = 0; i < newPreset.attributeValues.Length; i++)
            {
                newPreset.attributeValues[i] = _characterBuilder.AttributeValues[i];
            }

            presets.Add(newPreset);
            data.presets = presets.ToArray();

            SaveToJSON(data);
        }

        private void SaveToJSON(PresetSaveData data)
        {
            string savePath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "PresetData.json";

            string json = JsonUtility.ToJson(data, true);

            using StreamWriter writer = new StreamWriter(savePath);
            writer.Write(json);
        }
    }
}