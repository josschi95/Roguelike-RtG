using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using JS.EventSystem;
using JS.CommandSystem;
using TMPro;
using JS.Primitives;
using System.Diagnostics;
using JS.World.Alchemy;

namespace JS.WorldMap.Generation
{
    public class WorldGenerator : MonoBehaviour
    {
        public bool useErosion = true;

        public System.Random PRNG { get; private set; }
        public int Seed { get; private set; }

        [SerializeField] private BoolVariable AutoGenSavedWorld;
        [SerializeField] private GameObject generationPanel;
        [Space]
            
        [SerializeField] private WorldSize worldSize;
        [SerializeField] private int yearsOfHistory = 10;

        [Space]

        [SerializeField] private WorldGenerationParameters worldGenParams;
        [SerializeField] private WorldData worldMap;
        [SerializeField] private SettlementData settlementData;
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

        //Set from the World Generation Settings Menu
        public void SetSize(int size)
        {
            worldSize = (WorldSize)size;
        }

        #region - Seed -
        //Called from World Generation Settings Menu
        public void SetRandomSeed()
        {
            SetSeed(Random.Range(1, int.MaxValue));
        }

        public void SetSeed(int value)
        {
            Seed = value;
            worldMap.Seed = Seed;
        }
        #endregion

        //Called from World Generation Settings Menu
        public void OnBeginWorldGeneration()
        {
            timeKeeper.ResetTime();
            SetWorldValues();

            stopWatch = Stopwatch.StartNew();
            progressText.text = "Loading";
            progressBar.fillAmount = 0;

            StartCoroutine(GenerateWorld());
        }

        private void SetWorldValues()
        {
            PRNG = new System.Random(Seed);

            terrainGenerator.SetInitialValues(worldSize);

            var size = worldMap.TerrainData.MapSize;
            worldMap.CreateWorldGrid(size, size);

            PlantSeeds();
        }

        /// <summary>
        /// Assigns a random seed to each World Tile.
        /// </summary>
        private void PlantSeeds()
        {
            var seedMap = new int[worldMap.Width, worldMap.Height];
            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    seedMap[x, y] = PRNG.Next();
                }
            }
            worldMap.TerrainData.SeedMap = seedMap;
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
            yield return StartCoroutine(historyGenerator.RunHistory(yearsOfHistory));

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
            yield return StartCoroutine(UpdateProgress("Generating Tectonic Plates"));
            terrainGenerator.PlaceTectonicPlates();
            progressBar.fillAmount = 0.1f;
            yield return StartCoroutine(UpdateProgress("Generating Land Masses"));

            terrainGenerator.GenerateHeightMap();
            progressBar.fillAmount = 0.2f;
            yield return StartCoroutine(UpdateProgress("Eroding Land Masses"));

            if (useErosion)
            {
                terrainGenerator.ErodeLandMasses();
                progressBar.fillAmount = 0.2f;
                yield return StartCoroutine(UpdateProgress("Allocating Height Map"));
            }

            terrainGenerator.SetNodeAltitudeValues();
            progressBar.fillAmount = 0.2f;
            yield return StartCoroutine(UpdateProgress("Identifying Mountains"));

            terrainGenerator.IdentifyCoasts();
            terrainGenerator.IdentifyMountains();
            progressBar.fillAmount = 0.3f;
            yield return StartCoroutine(UpdateProgress("Identifying Bodies of Water"));

            yield return StartCoroutine(terrainGenerator.IdentifyBodiesOfWater());
            progressBar.fillAmount = 0.3f;
            yield return StartCoroutine(UpdateProgress("Identifying Land Masses"));

            yield return StartCoroutine(terrainGenerator.IdentifyLandMasses());
            progressBar.fillAmount = 0.3f;
            yield return StartCoroutine(UpdateProgress("Generating Rivers"));

            riverGenerator.GenerateRivers(worldGenParams.LocalDimensions.x, worldGenParams.RiverCount(worldSize));
            progressBar.fillAmount = 0.5f;
            yield return StartCoroutine(UpdateProgress("Generating Heat Map"));

            terrainGenerator.GenerateHeatMap();
            progressBar.fillAmount = 0.6f;
            yield return StartCoroutine(UpdateProgress("Generating Precipitation Map"));

            terrainGenerator.GeneratePrecipitationMap();
            progressBar.fillAmount = 0.7f;
            yield return StartCoroutine(UpdateProgress("Generating Biomes"));

            //This step here is now the largest time cost for world generation, about 12 seconds for a 400x400 map
            terrainGenerator.GenerateBiomes();
            progressBar.fillAmount = 0.8f;
            yield return StartCoroutine(UpdateProgress("Generating Resources"));

            terrainGenerator.GenerateOreDeposits();
            progressBar.fillAmount = 0.8f;
            yield return StartCoroutine(UpdateProgress("Generating Settlements"));
        }

        /// <summary>
        /// Generates settlements and connections
        /// </summary>
        private IEnumerator HandleSettlementGeneration()
        {
            settlementGenerator.PlaceSeeds();
            progressBar.fillAmount = 0.9f;
            yield return StartCoroutine(UpdateProgress("Generating Roads"));

            roadGenerator.GenerateRoads();
            progressBar.fillAmount = 0.9f;
            yield return StartCoroutine(UpdateProgress("Finalizing"));
        }

        /// <summary>
        /// Updates loading bar text and adds short delay before next method
        /// </summary>
        private IEnumerator UpdateProgress(string message)
        {
            if (stopWatch.ElapsedMilliseconds > 1000) //only debug message if greater than a second
                UnityEngine.Debug.Log(progressText.text + ": " + (stopWatch.ElapsedMilliseconds + "ms"));

            progressText.text = message;
            yield return new WaitForEndOfFrame();
            stopWatch.Restart();
        }

        /// <summary>
        /// Assigns each WorldTile a Danger Level based on distance from a randomly selected point
        /// </summary>
        private void SetDangerZones()
        {
            int largest = 0;
            LandMass continent = null;
            int size = terrainGenerator.mapSize;

            foreach(var mass in worldMap.TerrainData.LandMasses)
            {
                if (mass.GridNodes == null) UnityEngine.Debug.LogWarning("Nodes are null!");
                if (mass.Size != LandSize.Continent) continue;
                if (mass.GridNodes.Length <= largest) continue;

                largest = mass.GridNodes.Length;
                continent = mass;
            }

            var center = continent.GridNodes[PRNG.Next(0, continent.GridNodes.Length)];

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    var dist = GridMath.GetStraightDist(center.x, center.y, x, y) / size;
                    var dt = Mathf.RoundToInt(dist * 10);
                    var node = worldMap.GetNode(x, y);
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
            int index = PRNG.Next(0, settlementData.Settlements.Length);
            var settlement = settlementData.Settlements[index];

            playerData.World = new Vector3Int(settlement.x, settlement.y, 0);

            var region_x = Mathf.FloorToInt(worldGenParams.RegionDimensions.x * 0.5f);
            var region_y = Mathf.FloorToInt(worldGenParams.RegionDimensions.y * 0.5f);
            playerData.Region = new Vector2Int(region_x, region_y);

            var local_x = Mathf.FloorToInt(worldGenParams.LocalDimensions.x * 0.5f);
            var local_y = Mathf.FloorToInt(worldGenParams.LocalDimensions.y * 0.5f);
            playerData.Local = new Vector2Int(local_x, local_y);
        }

        #region - World Recreation -
        /// <summary>
        /// Recreate the world map given the save data
        /// </summary>
        private void OnBeginWorldRecreation(WorldSaveData data)
        {
            SetSeed(data.seed);
            SetWorldValues();
            StartCoroutine(RecreateWorld(data));
        }

        private IEnumerator RecreateWorld(WorldSaveData data)
        {
            HandleTerrainRecreation(data);
            yield return new WaitForEndOfFrame();
            RecreateSettlements(data);

            yield return StartCoroutine(historyGenerator.RunHistory(yearsOfHistory));


            worldGenCompleteEvent?.Invoke();
        }

        /// <summary>
        /// Recreates the world using saved data and seed
        /// </summary>
        private void HandleTerrainRecreation(WorldSaveData data)
        {
            terrainGenerator.PlaceTectonicPlates();
            terrainGenerator.GenerateHeightMap();
            terrainGenerator.ErodeLandMasses();
            terrainGenerator.SetNodeAltitudeValues();

            terrainGenerator.IdentifyCoasts();
            terrainGenerator.IdentifyMountains();
            worldMap.TerrainData.Lakes = data.Lakes;
            worldMap.TerrainData.LandMasses = data.Land;

            riverGenerator.GenerateRivers(worldGenParams.LocalDimensions.x, worldGenParams.RiverCount(worldSize));
            terrainGenerator.GenerateHeatMap();
            terrainGenerator.GeneratePrecipitationMap();
            terrainGenerator.GenerateBiomes();
            terrainGenerator.GenerateOreDeposits();

            SetDangerZones();
        }

        private void RecreateSettlements(WorldSaveData data)
        {
            settlementGenerator.PlaceSeeds();
            roadGenerator.GenerateRoads();
        }
        #endregion
    }
}