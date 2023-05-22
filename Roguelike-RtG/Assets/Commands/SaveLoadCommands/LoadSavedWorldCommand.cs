using System.IO;
using UnityEngine;
using JS.WorldMap;

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
            terrainData.SaveExists = true;

            terrainData.MapSize = data.mapWidth;
            terrainData.Origin = new Vector3Int(data.originX, data.originY);
            terrainData.HeightMap = ArrayHelper.Convert1DFloatArrayTo2D(data.heightMap, data.mapWidth, data.mapHeight);
            terrainData.HeatMap = ArrayHelper.Convert1DFloatArrayTo2D(data.heatMap, data.mapWidth, data.mapHeight);
            terrainData.MoistureMap = ArrayHelper.Convert1DFloatArrayTo2D(data.moistureMap, data.mapWidth, data.mapHeight);

            terrainData.BiomeMap = ArrayHelper.Convert1DIntArrayTo2D(data.biomeMap, data.mapWidth, data.mapHeight);

            terrainData.Mountains = data.Mountains;
            terrainData.Lakes = data.Lakes;
            terrainData.Rivers = data.Rivers;
            terrainData.BiomeGroups = data.BiomeGroups;
            terrainData.Islands = data.Islands;
        }
    }
}