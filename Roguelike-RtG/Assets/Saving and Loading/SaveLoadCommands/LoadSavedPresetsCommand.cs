using System.IO;
using UnityEngine;
using JS.CharacterSystem.Creation;

namespace JS.CommandSystem
{
    /// <summary>
    /// A command that loads in saved presets to a scriptable object
    /// </summary>
    [CreateAssetMenu(menuName = "Command System/Load Presets")]
    public class LoadSavedPresetsCommand : CommandBase
    {
        [Space]
        [Space]

        [SerializeField] private CustomCharacterPresets playerPresets;

        protected override bool ExecuteCommand()
        {
            CheckForPresets();
            return true;
        }

        private void CheckForPresets()
        {
            string[] saves = Directory.GetFiles(Application.persistentDataPath);
            foreach (string save in saves)
            {
                if (save.Contains("PresetData"))
                {
                    LoadPresets(save);
                    return;
                }
            }
        }

        private void LoadPresets(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            string json = reader.ReadToEnd();

            PresetSaveData data = JsonUtility.FromJson<PresetSaveData>(json);

            playerPresets.presets = new CreaturePresetData[data.presets.Length];
            for (int i = 0; i < playerPresets.presets.Length; i++)
            {
                playerPresets.presets[i] = data.presets[i];
            }
        }
    }
}

