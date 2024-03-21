using System.IO;
using UnityEngine;
using JS.World.Map;
using JS.World.Time;

namespace JS.Architecture.CommandSystem
{
    /// <summary>
    /// A command that checks for and loads the saved world data
    /// </summary>
    [CreateAssetMenu(menuName = "Command System/Save World")]
    public class SaveWorldDataCommand : CommandBase
    {
        [Space]
        [Space]

        [SerializeField] private TimeKeeper timeData;

        protected override bool ExecuteCommand()
        {
            SaveWorldData();
            return true;
        }

        private void SaveWorldData()
        {
            var data = new WorldSaveData();
            data.seed = WorldMap.Seed;
            //data.seedMap = ArrayHelper.Convert2DIntArrayTo1D(worldData.TerrainData.SeedMap);

            //Time
            data.seconds = timeData.Seconds;
            data.minutes = timeData.Minutes;
            data.hours = timeData.Hours;
            data.days = timeData.Days;
            data.months = timeData.Months;
            data.years = timeData.Year;

            //Terrain
            data.mapWidth = World.Map.Features.TerrainData.MapSize;
            data.mapHeight = World.Map.Features.TerrainData.MapSize;

            data.Lakes = World.Map.Features.TerrainData.Lakes;
            data.Land = World.Map.Features.TerrainData.LandMasses;

            //Settlements
            data.Settlements = SettlementData.Settlements;
            data.Roads = World.Map.Features.TerrainData.Roads;
            data.Bridges = World.Map.Features.TerrainData.Bridges;


            SaveToJSON(data);
            WorldMap.SaveExists = true;
            WorldMap.IsLoaded = true;
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