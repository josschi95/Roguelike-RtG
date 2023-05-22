using System.IO;
using UnityEngine;
using JS.WorldMap;
using JS.WorldMap.Generation;

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

        [SerializeField] private WorldMap.TerrainData terrainData;

        protected override bool ExecuteCommand()
        {
            SaveWorldData();
            return true;
        }

        private void SaveWorldData()
        {
            string savePath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "WorldData.json";

            var data = new WorldData();

            data.mapWidth = terrainData.MapSize;
            data.mapHeight = terrainData.MapSize;

            data.originX = terrainData.Origin.x;
            data.originY = terrainData.Origin.y;
            
            data.heightMap = ArrayHelper.Convert2DFloatArrayTo1D(terrainData.HeightMap);
            data.heatMap = ArrayHelper.Convert2DFloatArrayTo1D(terrainData.HeatMap);
            data.moistureMap = ArrayHelper.Convert2DFloatArrayTo1D(terrainData.MoistureMap);

            data.biomeMap = ArrayHelper.Convert2DIntArrayTo1D(terrainData.BiomeMap);
            data.BiomeGroups = terrainData.BiomeGroups;

            data.Mountains = terrainData.Mountains;
            data.Lakes = terrainData.Lakes;
            data.Rivers = terrainData.Rivers;
            data.Islands = terrainData.Islands;

            string json = JsonUtility.ToJson(data, true);
            Debug.Log(json);

            using StreamWriter writer = new StreamWriter(savePath);
            writer.Write(json);
        }
    }
}

