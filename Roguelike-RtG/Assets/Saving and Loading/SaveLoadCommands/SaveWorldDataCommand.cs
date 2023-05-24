using System.IO;
using UnityEngine;
using JS.WorldMap;

namespace JS.CommandSystem
{
    /// <summary>
    /// A command that checks for and loads the saved world data
    /// </summary>
    [CreateAssetMenu(menuName = "Command System/Save World")]
    public class SaveWorldDataCommand : CommandBase
    {
        [Space]
        [Space]

        [SerializeField] private WorldData worldData;
        [SerializeField] private TimeKeeper timeData;

        protected override bool ExecuteCommand()
        {
            SaveWorldData();
            return true;
        }

        private void SaveWorldData()
        {
            var data = new WorldSaveData();

            //Time
            data.seconds = timeData.Seconds;
            data.minutes = timeData.Minutes;
            data.hours = timeData.Hours;
            data.days = timeData.Days;
            data.weeks = timeData.Weeks;
            data.months = timeData.Months;
            data.years = timeData.Years;

            //Terrain
            var terrain = worldData.TerrainData;
            data.mapWidth = terrain.MapSize;
            data.mapHeight = terrain.MapSize;

            data.originX = terrain.Origin.x;
            data.originY = terrain.Origin.y;
            
            data.heightMap = ArrayHelper.Convert2DFloatArrayTo1D(terrain.HeightMap);
            data.heatMap = ArrayHelper.Convert2DFloatArrayTo1D(terrain.HeatMap);
            data.moistureMap = ArrayHelper.Convert2DFloatArrayTo1D(terrain.MoistureMap);

            data.biomeMap = ArrayHelper.Convert2DIntArrayTo1D(terrain.BiomeMap);
            data.BiomeGroups = terrain.BiomeGroups;

            data.Mountains = terrain.Mountains;
            data.Lakes = terrain.Lakes;
            data.Rivers = terrain.Rivers;
            data.Islands = terrain.Islands;

            //Settlements
            var settlements = worldData.SettlementData;
            data.Settlements = settlements.Settlements;
            SaveToJSON(data);
        }

        private void SaveToJSON(WorldSaveData data)
        {
            string savePath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "WorldData.json";

            string json = JsonUtility.ToJson(data, true);

            using StreamWriter writer = new StreamWriter(savePath);
            writer.Write(json);
        }
    }
}