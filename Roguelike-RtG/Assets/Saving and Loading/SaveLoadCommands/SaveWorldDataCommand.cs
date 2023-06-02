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
            data.months = timeData.Months;
            data.years = timeData.Year;

            //Terrain
            var terrain = worldData.TerrainData;
            data.mapWidth = terrain.MapSize;
            data.mapHeight = terrain.MapSize;
            
            data.heightMap = ArrayHelper.Convert2DFloatArrayTo1D(terrain.HeightMap);
            data.heatMap = ArrayHelper.Convert2DFloatArrayTo1D(terrain.HeatMap);
            data.moistureMap = ArrayHelper.Convert2DFloatArrayTo1D(terrain.MoistureMap);
            data.biomeMap = ArrayHelper.Convert2DIntArrayTo1D(terrain.BiomeMap);

            data.CoalMap = ArrayHelper.Convert2DFloatArrayTo1D(terrain.CoalMap);
            data.CopperMap = ArrayHelper.Convert2DFloatArrayTo1D(terrain.CopperMap);
            data.IronMap = ArrayHelper.Convert2DFloatArrayTo1D(terrain.IronMap);
            data.SilverMap = ArrayHelper.Convert2DFloatArrayTo1D(terrain.SilverMap);
            data.GoldMap = ArrayHelper.Convert2DFloatArrayTo1D(terrain.GoldMap);
            data.MithrilMap = ArrayHelper.Convert2DFloatArrayTo1D(terrain.MithrilMap);
            data.AdmanatineMap = ArrayHelper.Convert2DFloatArrayTo1D(terrain.AdmanatineMap);
            data.GemstoneMap = ArrayHelper.Convert2DFloatArrayTo1D(terrain.GemstoneMap);

            data.BiomeGroups = terrain.BiomeGroups;
            data.Mountains = terrain.Mountains;
            data.Lakes = terrain.Lakes;
            data.Rivers = terrain.Rivers;
            data.Islands = terrain.Islands;

            //Settlements
            var settlements = worldData.SettlementData;
            data.Settlements = settlements.Settlements;
            data.Roads = terrain.Roads;
            data.Bridges = terrain.Bridges;



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