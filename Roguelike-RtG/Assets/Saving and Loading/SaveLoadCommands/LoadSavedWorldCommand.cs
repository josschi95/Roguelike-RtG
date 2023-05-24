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
        [SerializeField] private WorldData _worldMapData;
        [SerializeField] private TimeKeeper timeData;

        protected override bool ExecuteCommand()
        {
            return CheckForExistingSave();
        }

        private bool CheckForExistingSave()
        {
            string[] saves = Directory.GetFiles(Application.persistentDataPath);
            if (saves.Length == 0)
            {
                _worldMapData.SaveExists = false;
                return false;
            }

            foreach (string save in saves)
            {
                if (save.Contains("WorldData"))
                {
                    LoadSavedWorld(save);
                    return true;
                }
            }
            _worldMapData.SaveExists = false;
            return false;
        }

        private void LoadSavedWorld(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            string json = reader.ReadToEnd();

            WorldSaveData data = JsonUtility.FromJson<WorldSaveData>(json);

            SetTimeData(data);
            SetTerrainValues(data);
            SetSettlementValues(data);
        }

        private void SetTimeData(WorldSaveData data)
        {
            timeData.SetSavedTime(data.seconds, data.minutes, data.hours, data.days, data.weeks, data.months, data.years);
        }

        private void SetTerrainValues(WorldSaveData data)
        {
            _worldMapData.SaveExists = true;
            _worldMapData.Seed = data.seed;

            var terrain = _worldMapData.TerrainData;

            terrain.MapSize = data.mapWidth;
            terrain.Origin = new Vector3Int(data.originX, data.originY);
            terrain.HeightMap = ArrayHelper.Convert1DFloatArrayTo2D(data.heightMap, data.mapWidth, data.mapHeight);
            terrain.HeatMap = ArrayHelper.Convert1DFloatArrayTo2D(data.heatMap, data.mapWidth, data.mapHeight);
            terrain.MoistureMap = ArrayHelper.Convert1DFloatArrayTo2D(data.moistureMap, data.mapWidth, data.mapHeight);

            terrain.BiomeMap = ArrayHelper.Convert1DIntArrayTo2D(data.biomeMap, data.mapWidth, data.mapHeight);

            terrain.Mountains = data.Mountains;
            terrain.Lakes = data.Lakes;
            terrain.Rivers = data.Rivers;
            terrain.BiomeGroups = data.BiomeGroups;
            terrain.Islands = data.Islands;

            _worldMapData.CreateGridFromData(data);
        }

        private void SetSettlementValues(WorldSaveData data)
        {
            var settlements = _worldMapData.SettlementData;
            settlements.AddSettlements(data.Settlements);
        }
    }
}