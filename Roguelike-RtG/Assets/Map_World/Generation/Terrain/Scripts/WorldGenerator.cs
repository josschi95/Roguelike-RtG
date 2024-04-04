using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JS.Architecture.EventSystem;
using JS.Architecture.CommandSystem;
using JS.Architecture.Primitives;
using JS.World.Map.Features;
using JS.World.Alchemy;
using JS.World.Time;

namespace JS.World.Map.Generation
{
    public class WorldGenerator : MonoBehaviour
    {
        public bool useErosion = true;

        public System.Random PRNG { get; private set; }
        public int Seed { get; private set; }

        [SerializeField] private BoolVariable AutoGenSavedWorld;
        [SerializeField] private GameObject generationPanel;
        [Space]
        
        private WorldGenParameters _worldGenParams;
        private WorldSize worldSize;
        private WorldAge worldAge;

        [Space]

        [SerializeField] private PlayerData playerData;
        [SerializeField] private TimeKeeper timeKeeper;
        [SerializeField] private BiomeHelper biomeHelper;

        [Space]

        [SerializeField] private TerrainGenerator terrainGenerator;
        [SerializeField] private RiverGenerator riverGenerator;
        [SerializeField] private SettlementGenerator settlementGenerator;
        [SerializeField] private RoadGenerator roadGenerator;
        [SerializeField] private HistoryGenerator historyGenerator;

        [Space]

        [SerializeField] private Image progressBar;
        [SerializeField] private TMP_Text progressText;

        [Header("Game Events")]
        [SerializeField] private GameEvent worldGenCompleteEvent;
        [SerializeField] private GetWorldSaveCommand getWorldSaveCommand;
        [SerializeField] private SaveWorldDataCommand saveWorldDataCommand;

        private Stopwatch stopWatch;

        private void Awake() => CheckForAutoGen();

        /// <summary>
        /// Automatically generates the world from a save file. Only occurs if selecting Continue from Main Menu
        /// </summary>
        private void CheckForAutoGen()
        {
            if (AutoGenSavedWorld.Value == true)
            {
                //recreate world from save
                var data = getWorldSaveCommand.GetWorldSaveData();
                if (data != null) OnBeginWorldRecreation(data);
                else generationPanel.SetActive(true);
            }
            else
            {
                //behave normally
                generationPanel.SetActive(true);
            }
        }

        #region - World Settings -
        //Set from the World Generation Settings Menu
        public void SetSize(int size)
        {
            worldSize = (WorldSize)size;
        }

        public void SetWorldAge(int index)
        {
            worldAge = (WorldAge)index;
        }

        public void SetNorthLatitude(float value)
        {
            Features.TerrainData.NorthLatitude = Mathf.RoundToInt(value);
        }

        public void SetSouthLatitude(float value)
        {
            Features.TerrainData.SouthLatitude = Mathf.RoundToInt(value);
        }
        #endregion

        #region - Seed -
        //Called from World Generation Settings Menu
        public void SetRandomSeed()
        {
            SetSeed(Random.Range(1, int.MaxValue));
        }

        public void SetSeed(int value)
        {
            Seed = value;
            WorldMap.Seed = Seed;
        }
        #endregion

        //Called from World Generation Settings Menu
        public void OnBeginWorldGeneration()
        {
            timeKeeper.ResetTime();

            LoadJSONFiles();
            SetWorldValues();

            stopWatch = Stopwatch.StartNew();
            progressText.text = "Loading";
            progressBar.fillAmount = 0;

            StartCoroutine(GenerateWorld());
        }

        private void LoadJSONFiles()
        {
            var worldParamText = Resources.Load("WorldGenParameters") as TextAsset;
            StringReader reader = new StringReader(worldParamText.text);
            string worldJSON = reader.ReadToEnd();

            _worldGenParams = JsonUtility.FromJson<WorldGenParameters>(worldJSON);

            WorldParameters.REGION_WIDTH = _worldGenParams.REGION_WIDTH;
            WorldParameters.REGION_HEIGHT = _worldGenParams.REGION_HEIGHT;

            WorldParameters.LOCAL_WIDTH = _worldGenParams.LOCAL_WIDTH;
            WorldParameters.LOCAL_HEIGHT = _worldGenParams.LOCAL_HEIGHT;

            WorldParameters.SEA_LEVEL = _worldGenParams.SEA_LEVEL;
            WorldParameters.MOUNTAIN_HEIGHT = _worldGenParams.MOUNTAIN_HEIGHT;

            ClimateMath.TemperatureZones = _worldGenParams.TEMPERATURE_ZONES;
            ClimateMath.PrecipitationZones = _worldGenParams.PRECIPITATION_ZONES;
        }

        private void SetWorldValues()
        {
            PRNG = new System.Random(Seed);

            terrainGenerator.SetInitialValues(_worldGenParams.WORLD_SIZE_PARAMS.WORLD_WIDTH[(int)worldSize]);

            var size = Features.TerrainData.MapSize;
            WorldMap.CreateWorldGrid(size, size);

            PlantSeeds();
        }

        /// <summary>
        /// Assigns a random seed to each World Tile.
        /// </summary>
        private void PlantSeeds()
        {
            var seedMap = new int[WorldMap.Width, WorldMap.Height];
            for (int x = 0; x < WorldMap.Width; x++)
            {
                for (int y = 0; y < WorldMap.Height; y++)
                {
                    seedMap[x, y] = PRNG.Next();
                }
            }
            Features.TerrainData.SeedMap = seedMap;
        }

        private IEnumerator GenerateWorld()
        {
            //Generate Terrain and Terrain Features
            yield return StartCoroutine(HandleTerrainGeneration());
            
            SetDangerZones();

            //Generate Plantlife
            FloraGenerator.GenerateFlora(PRNG);

            //Generate Settlements, Roads, and Relations
            yield return StartCoroutine(HandleSettlementGeneration());

            //Generate History
            yield return StartCoroutine(historyGenerator.RunHistory(_worldGenParams.YEARS_OF_HISTORY[(int)worldAge]));

            //Generate Historical Figures, Locations, Items, and Events

            //Place points of interest using Poisson

            PlacePlayerAtStart();

            saveWorldDataCommand?.Invoke();
            worldGenCompleteEvent?.Invoke();
        }

        /// <summary>
        /// Generate terrestrial features and environments
        /// </summary>
        private IEnumerator HandleTerrainGeneration()
        {
            yield return StartCoroutine(UpdateProgress("Generating Tectonic Plates", 0.0f));
            terrainGenerator.PlaceTectonicPlates(
                _worldGenParams.WORLD_SIZE_PARAMS.PLATE_COUNT[(int)worldSize],
                _worldGenParams.WORLD_SIZE_PARAMS.PLATE_MIN_SIZE[(int)worldSize],
                _worldGenParams.WORLD_SIZE_PARAMS.PLATE_MAX_SIZE[(int)worldSize]);
            yield return StartCoroutine(UpdateProgress("Generating Land Masses", 0.1f));

            terrainGenerator.GenerateHeightMap();
            yield return StartCoroutine(UpdateProgress("Eroding Land Masses", 0.2f));

            if (useErosion)
            {
                terrainGenerator.ErodeLandMasses(_worldGenParams.WORLD_SIZE_PARAMS.EROSION[(int)worldSize]);
                yield return StartCoroutine(UpdateProgress("Allocating Height Map", 0.2f));
            }

            terrainGenerator.SetNodeAltitudeValues();
            yield return StartCoroutine(UpdateProgress("Identifying Mountains", 0.2f));

            terrainGenerator.IdentifyCoasts();
            terrainGenerator.IdentifyMountains();
            yield return StartCoroutine(UpdateProgress("Identifying Bodies of Water", 0.3f));

            yield return StartCoroutine(terrainGenerator.IdentifyBodiesOfWater());
            yield return StartCoroutine(UpdateProgress("Identifying Land Masses", 0.3f));

            yield return StartCoroutine(terrainGenerator.IdentifyLandMasses());
            yield return StartCoroutine(UpdateProgress("Generating Rivers", 0.3f));

            riverGenerator.GenerateRivers(_worldGenParams.WORLD_SIZE_PARAMS.RIVERS[(int)worldSize]);
            yield return StartCoroutine(UpdateProgress("Generating Heat Map", 0.5f));

            terrainGenerator.GenerateHeatMap(Features.TerrainData.NorthLatitude, Features.TerrainData.SouthLatitude);
            yield return StartCoroutine(UpdateProgress("Generating Precipitation Map", 0.6f));

            terrainGenerator.GeneratePrecipitationMap();
            yield return StartCoroutine(UpdateProgress("Generating Biomes", 0.7f));

            //This step here is now the largest time cost for world generation, about 12 seconds for a 400x400 map
            terrainGenerator.GenerateBiomes();
            yield return StartCoroutine(UpdateProgress("Generating Resources", 0.8f));

            terrainGenerator.GenerateOreDeposits(
                _worldGenParams.MINERAL_PREVALENCE.COAL[3],
                _worldGenParams.MINERAL_PREVALENCE.COPPER[3],
                _worldGenParams.MINERAL_PREVALENCE.IRON[3],
                _worldGenParams.MINERAL_PREVALENCE.SILVER[3],
                _worldGenParams.MINERAL_PREVALENCE.GOLD[3],
                _worldGenParams.MINERAL_PREVALENCE.GEMSTONE[3],
                _worldGenParams.MINERAL_PREVALENCE.MITHRIL[3],
                _worldGenParams.MINERAL_PREVALENCE.ADAMANTINE[3]);
            yield return StartCoroutine(UpdateProgress("Generating Settlements", 0.8f));
        }

        /// <summary>
        /// Generates settlements and connections
        /// </summary>
        private IEnumerator HandleSettlementGeneration()
        {
            settlementGenerator.PlaceSeeds();
            yield return StartCoroutine(UpdateProgress("Generating Roads", 0.9f));

            roadGenerator.GenerateRoads();
            yield return StartCoroutine(UpdateProgress("Finalizing", 0.9f));
        }

        /// <summary>
        /// Updates loading bar text and adds short delay before next method
        /// </summary>
        private IEnumerator UpdateProgress(string message, float progress)
        {
            progressBar.fillAmount = progress;

            if (stopWatch.ElapsedMilliseconds > 1000) //only debug message if greater than a second
                UnityEngine.Debug.Log(progressText.text + ": " + (stopWatch.ElapsedMilliseconds + "ms"));

            progressText.text = message;
            yield return new WaitForEndOfFrame();
            stopWatch.Restart();
        }

        /// <summary>
        /// Assigns each WorldTile a Danger Level based on distance from a pseudo-randomly selected point
        /// </summary>
        private void SetDangerZones()
        {
            int largest = 0;
            LandMass continent = null;
            int size = terrainGenerator.mapSize;
            // Selects the largest continent
            foreach(var mass in Features.TerrainData.LandMasses)
            {
                if (mass.GridNodes == null) UnityEngine.Debug.LogWarning("Nodes are null!");
                if (mass.Size != LandSize.Continent) continue;
                if (mass.GridNodes.Length <= largest) continue;

                largest = mass.GridNodes.Length;
                continent = mass;
            }

            var center = continent.GridNodes[PRNG.Next(0, continent.GridNodes.Length)];
            // Sets Danger Level based on distance from random point in largest continent
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    var dist = GridMath.GetStraightDist(center.x, center.y, x, y) / size;
                    var dt = Mathf.RoundToInt(dist * 10);
                    var node = WorldMap.GetNode(x, y);
                    dt = Mathf.Clamp(dt + biomeHelper.GetBiome(node.BiomeID).DangerModifier, 0, 9);
                    node.DangerTier = dt;
                }
            }
        }

        /// <summary>
        /// Places player at a random settlement
        /// </summary>
        private void PlacePlayerAtStart()
        {
            if (SettlementData.Settlements == null || SettlementData.Settlements.Length == 0)
            {
                playerData.World = Vector3Int.zero;
                playerData.Region = Vector2Int.zero;
                playerData.Local = Vector2Int.zero;
                return;

            }
            int index = PRNG.Next(0, SettlementData.Settlements.Length);
            var settlement = SettlementData.Settlements[index];

            playerData.World = new Vector3Int(settlement.x, settlement.y, 0);

            var region_x = Mathf.FloorToInt(WorldParameters.REGION_WIDTH * 0.5f);
            var region_y = Mathf.FloorToInt(WorldParameters.REGION_HEIGHT * 0.5f);
            playerData.Region = new Vector2Int(region_x, region_y);

            var local_x = Mathf.FloorToInt(WorldParameters.LOCAL_WIDTH * 0.5f);
            var local_y = Mathf.FloorToInt(WorldParameters.LOCAL_HEIGHT * 0.5f);
            playerData.Local = new Vector2Int(local_x, local_y);
        }

        #region - World Recreation -
        /// <summary>
        /// Recreate the world map given the save data
        /// </summary>
        private void OnBeginWorldRecreation(WorldSaveData data)
        {
            LoadJSONFiles();
            SetSeed(data.seed);
            SetWorldValues();
            StartCoroutine(RecreateWorld(data));
        }

        private IEnumerator RecreateWorld(WorldSaveData data)
        {
            HandleTerrainRecreation(data);
            yield return new WaitForEndOfFrame();
            RecreateSettlements(data);

            yield return StartCoroutine(historyGenerator.RunHistory(_worldGenParams.YEARS_OF_HISTORY[(int)worldAge]));


            worldGenCompleteEvent?.Invoke();
        }

        /// <summary>
        /// Recreates the world using saved data and seed
        /// </summary>
        private void HandleTerrainRecreation(WorldSaveData data)
        {
            terrainGenerator.PlaceTectonicPlates(
                _worldGenParams.WORLD_SIZE_PARAMS.PLATE_COUNT[(int)worldSize],
                _worldGenParams.WORLD_SIZE_PARAMS.PLATE_MIN_SIZE[(int)worldSize],
                _worldGenParams.WORLD_SIZE_PARAMS.PLATE_MAX_SIZE[(int)worldSize]);

            terrainGenerator.GenerateHeightMap();
            terrainGenerator.ErodeLandMasses(_worldGenParams.WORLD_SIZE_PARAMS.EROSION[(int)worldSize]);
            terrainGenerator.SetNodeAltitudeValues();

            terrainGenerator.IdentifyCoasts();
            terrainGenerator.IdentifyMountains();
            Features.TerrainData.Lakes = data.Lakes;
            Features.TerrainData.LandMasses = data.Land;

            riverGenerator.GenerateRivers(_worldGenParams.WORLD_SIZE_PARAMS.RIVERS[(int)worldSize]);
            terrainGenerator.GenerateHeatMap(data.northLatitude, data.southLatitude);
            terrainGenerator.GeneratePrecipitationMap();
            terrainGenerator.GenerateBiomes();
            terrainGenerator.GenerateOreDeposits(
                _worldGenParams.MINERAL_PREVALENCE.COAL[3],
                _worldGenParams.MINERAL_PREVALENCE.COPPER[3],
                _worldGenParams.MINERAL_PREVALENCE.IRON[3],
                _worldGenParams.MINERAL_PREVALENCE.SILVER[3],
                _worldGenParams.MINERAL_PREVALENCE.GOLD[3],
                _worldGenParams.MINERAL_PREVALENCE.GEMSTONE[3],
                _worldGenParams.MINERAL_PREVALENCE.MITHRIL[3],
                _worldGenParams.MINERAL_PREVALENCE.ADAMANTINE[3] );

            SetDangerZones();
        }

        private void RecreateSettlements(WorldSaveData data)
        {
            settlementGenerator.PlaceSeeds();
            roadGenerator.GenerateRoads();
        }
        #endregion
    }

    public enum WorldSize
    { 
        Tiny, 
        Small, 
        Medium, 
        Large, 
        Huge 
    }

    public enum WorldAge
    {
        Infant,
        Young,
        Mature,
        Old,
        Ancient,
    }

    #region - JSON -
    // These classes are need to convert the JSON files
    [System.Serializable]
    public class WorldGenParameters
    {
        public int REGION_WIDTH;
        public int REGION_HEIGHT;

        public int LOCAL_WIDTH;
        public int LOCAL_HEIGHT;

        public float SEA_LEVEL;
        public float MOUNTAIN_HEIGHT;

        public float[] TEMPERATURE_ZONES;
        public float[] PRECIPITATION_ZONES;

        public WorldSizeParams WORLD_SIZE_PARAMS;

        public int[] YEARS_OF_HISTORY;

        public MineralPrevalence MINERAL_PREVALENCE;
    }

    [System.Serializable]
    public class WorldSizeParams
    {
        public int[] WORLD_WIDTH;
        public int[] WORLD_HEIGHT;
        public int[] PLATE_COUNT;
        public int[] PLATE_MIN_SIZE;
        public int[] PLATE_MAX_SIZE;
        public int[] EROSION;
        public int[] RIVERS;
    }

    [System.Serializable]
    public class MineralPrevalence
    {
        public float[] COAL;
        public float[] COPPER;
        public float[] IRON;
        public float[] SILVER;
        public float[] GOLD;
        public float[] GEMSTONE;
        public float[] MITHRIL;
        public float[] ADAMANTINE;
    }
    #endregion
}

/*
 *     "SETTLEMENT_COUNT":{
        "VERY_LOW":  [5, 7, 10, 12, 15],
        "LOW":       [5, 10, 15, 20, 40],
        "MEDIUM":    [5, 10, 15, 30, 50],
        "HIGH":      [5, 10, 20, 40, 80],
        "VERY_HIGH": [10, 20, 40, 80, 160]
    }
 * 
 */