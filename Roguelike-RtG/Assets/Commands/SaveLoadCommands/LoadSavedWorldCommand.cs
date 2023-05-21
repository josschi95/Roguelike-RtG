using System.IO;
using UnityEngine;

namespace JS.CommandSystem
{
    /// <summary>
    /// A command that checks for and loads the saved world data
    /// </summary>
    [CreateAssetMenu(menuName = "Command System/Load Saved World")]
    public class LoadSavedWorldCommand : CommandBase
    {
        [Space]
        [Space]

        [SerializeField] private WorldMap.TerrainData terrainData;

        protected override bool ExecuteCommand()
        {
            return CheckForExistingSave();
        }

        private bool CheckForExistingSave()
        {
            string[] saves = Directory.GetFiles(Application.persistentDataPath, "WorldData");
            if (saves.Length == 0) return false;

            foreach (string save in saves)
            {
                if (save.Contains("WorldData"))
                {
                    LoadSavedWorld(save);
                    return true;
                }
            }
            return false;
        }

        private void LoadSavedWorld(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            string json = reader.ReadToEnd();

            WorldData data = JsonUtility.FromJson<WorldData>(json);
            SetTerrainValues(data);
        }

        private void SetTerrainValues(WorldData data)
        {
            terrainData.MapSize = data.mapWidth;
            terrainData.Origin = new Vector3Int(data.originX, data.originY);
            terrainData.HeightMap = data.heightMap;
            terrainData.HeatMap = data.heatMap;
            terrainData.MoistureMap = data.moistureMap;
            terrainData.BiomeMap = data.biomeMap;


        }
    }
}