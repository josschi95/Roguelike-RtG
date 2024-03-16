using System.IO;
using UnityEngine;
using JS.World.Map;
using JS.World.Time;

namespace JS.Architecture.CommandSystem
{
    /// <summary>
    /// A command that checks for and loads the saved world data
    /// </summary>
    [CreateAssetMenu(menuName = "Command System/Load Saved World")]
    public class GetWorldSaveCommand : CommandBase
    {
        [Space]
        [Space]
        [SerializeField] private WorldData _worldMapData;
        [SerializeField] private TimeKeeper timeData;

        protected override bool ExecuteCommand()
        {
            return CheckForExistingSave();
        }

        //Called from WorldGenerator when continuing game
        public WorldSaveData GetWorldSaveData()
        {
            string[] saves = Directory.GetFiles(Application.persistentDataPath);
            if (saves.Length == 0)
            {
                _worldMapData.SaveExists = false;
                _worldMapData.IsLoaded = false;
                return null;
            }

            foreach (string save in saves)
            {
                if (save.Contains("WorldData"))
                {
                    return ProcessSaveFile(save);
                }
            }
            _worldMapData.SaveExists = false;
            _worldMapData.IsLoaded = false;
            return null;
        }

        private WorldSaveData ProcessSaveFile(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            string json = reader.ReadToEnd();

            WorldSaveData data = JsonUtility.FromJson<WorldSaveData>(json);

            SetTimeData(data);

            return data;
        }

        private void SetTimeData(WorldSaveData data)
        {
            timeData.SetSavedTime(data.seconds, data.minutes, data.hours, data.days, data.months, data.years);
        }

        //Called through the Invoke menu from the Main Menu
        //Used to determine if the "Continue" button can be clicked
        private bool CheckForExistingSave()
        {
            string[] saves = Directory.GetFiles(Application.persistentDataPath);
            if (saves.Length == 0)
            {
                _worldMapData.SaveExists = false;
                _worldMapData.IsLoaded = false;
                return false;
            }
            
            foreach (string save in saves)
            {
                if (save.Contains("WorldData"))
                {
                    //LoadSavedWorld(save);
                    _worldMapData.SaveExists = true;
                    return true;
                }
            }
            _worldMapData.SaveExists = false;
            _worldMapData.IsLoaded = false;
            return false;
        }

        /*****************************************************************
         This is no longer being used with no need to save the information
         ****************************************************************/

        /*
         This is no longer being used with no need to save the information
        private void LoadSavedWorld(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            string json = reader.ReadToEnd();

            WorldSaveData data = JsonUtility.FromJson<WorldSaveData>(json);

            SetTimeData(data);
            SetTerrainValues(data);
            SetSettlementValues(data);
        }

        private void SetTerrainValues(WorldSaveData data)
        {
            _worldMapData.SaveExists = true;
            _worldMapData.Seed = data.seed;
            var terrain = _worldMapData.TerrainData;
            
            terrain.MapSize = data.mapWidth;

            terrain.SeedMap = ArrayHelper.Convert1DIntArrayTo2D(data.seedMap, data.mapWidth, data.mapHeight);
            terrain.HeightMap = ArrayHelper.Convert1DFloatArrayTo2D(data.heightMap, data.mapWidth, data.mapHeight);
            terrain.HeatMap = ArrayHelper.Convert1DFloatArrayTo2D(data.heatMap, data.mapWidth, data.mapHeight);
            terrain.MoistureMap = ArrayHelper.Convert1DFloatArrayTo2D(data.moistureMap, data.mapWidth, data.mapHeight);
            terrain.BiomeMap = ArrayHelper.Convert1DIntArrayTo2D(data.biomeMap, data.mapWidth, data.mapHeight);
            terrain.Coasts = ArrayHelper.Convert1DBoolArrayTo2D(data.coasts, data.mapWidth, data.mapHeight);
            terrain.CoalMap = ArrayHelper.Convert1DFloatArrayTo2D(data.CoalMap, data.mapWidth, data.mapHeight);
            terrain.CopperMap = ArrayHelper.Convert1DFloatArrayTo2D(data.CopperMap, data.mapWidth, data.mapHeight);
            terrain.IronMap = ArrayHelper.Convert1DFloatArrayTo2D(data.IronMap, data.mapWidth, data.mapHeight);
            terrain.SilverMap = ArrayHelper.Convert1DFloatArrayTo2D(data.SilverMap, data.mapWidth, data.mapHeight);
            terrain.GoldMap = ArrayHelper.Convert1DFloatArrayTo2D(data.GoldMap, data.mapWidth, data.mapHeight);
            terrain.MithrilMap = ArrayHelper.Convert1DFloatArrayTo2D(data.MithrilMap, data.mapWidth, data.mapHeight);
            terrain.AdmanatineMap = ArrayHelper.Convert1DFloatArrayTo2D(data.AdmanatineMap, data.mapWidth, data.mapHeight);
            terrain.GemstoneMap = ArrayHelper.Convert1DFloatArrayTo2D(data.GemstoneMap, data.mapWidth, data.mapHeight);
            terrain.BiomeGroups = data.BiomeGroups;
            terrain.Mountains = data.Mountains;
            terrain.Rivers = data.Rivers;
            
            terrain.Lakes = data.Lakes;
            terrain.LandMasses = data.Land;

            _worldMapData.CreateGrid(data.mapWidth, data.mapHeight);
        }

        private void SetSettlementValues(WorldSaveData data)
        {
            var settlements = _worldMapData.SettlementData;
            settlements.PlaceSettlements(data.Settlements);
            _worldMapData.TerrainData.Roads = data.Roads;
            _worldMapData.TerrainData.Bridges = data.Bridges;
        }
        */
    }
}