using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using JS.EventSystem;
using JS.CommandSystem;
using TMPro;
using JS.Primitives;

namespace JS.WorldMap.Generation
{
    public class WorldGenerator : MonoBehaviour
    {
        public System.Random PRNG { get; private set; }
        public int Seed { get; private set; }

        [SerializeField] private BoolVariable AutoGenSavedWorld;
        [SerializeField] private GameObject generationPanel;
        [Space]
            
        [SerializeField] private WorldSize worldSize;
        [SerializeField] private WorldGenerationParameters worldGenParams;
        [SerializeField] private WorldData worldMap;
        [SerializeField] private SettlementData settlementData;
        [SerializeField] private PlayerData playerData;
        [SerializeField] private TimeKeeper timeKeeper;

        [Space]

        [SerializeField] private TerrainGenerator terrainGenerator;
        [SerializeField] private RiverGenerator riverGenerator;
        [SerializeField] private SettlementGenerator settlementGenerator;
        [SerializeField] private RoadGenerator roadGenerator;
        //private FloraFaunaGenerator floraFaunaGenerator;
        //private ResourceGenerator resourceGenerator;
        //private HistoryGenerator historyGenerator

        [Space]

        [SerializeField] private Image progressBar;
        [SerializeField] private TMP_Text progressText;
        private float initialTime; //Used for debugging times

        [Header("Game Events")]
        [SerializeField] private GameEvent worldGenCompleteEvent;
        [SerializeField] private GetWorldSaveCommand getWorldSaveCommand;
        [SerializeField] private SaveWorldDataCommand saveWorldDataCommand;

        private void Awake() => CheckForAutoGen();

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
            initialTime = Time.realtimeSinceStartup;

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

            settlementGenerator.SetInitialValues(worldSize);

            PlantSeeds();
        }

        /// <summary>
        /// Assigns a random seed to each World Tile
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
            
            //Generate Settlements, Roads, and Relations
            yield return StartCoroutine(HandleSettlementGeneration());

            //Generate Plant and Wildlife

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

            terrainGenerator.ErodeLandMasses();
            progressBar.fillAmount = 0.2f;
            yield return StartCoroutine(UpdateProgress("Allocating Height Map"));

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
            settlementGenerator.PlaceSettlements();
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
            //Debug.Log(progressText.text + ": " + (Time.realtimeSinceStartup - initialTime));
            progressText.text = message;
            yield return new WaitForEndOfFrame();
            initialTime = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// Places player at a random settlement
        /// </summary>
        private void PlacePlayerAtStart()
        {
            int index = PRNG.Next(0, settlementData.Settlements.Length);
            var settlement = settlementData.Settlements[index];
            
            playerData.Position.x = settlement.X * worldGenParams.RegionDimensions.x;
            playerData.Position.y = settlement.Y * worldGenParams.RegionDimensions.y;
            playerData.Position.z = 0;

            playerData.LocalPosition.x = Mathf.FloorToInt(worldGenParams.LocalDimensions.x * 0.5f);
            playerData.LocalPosition.y = Mathf.FloorToInt(worldGenParams.LocalDimensions.y * 0.5f);
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
            //worldMap.TerrainData.Mountains = data.Mountains;
            worldMap.TerrainData.Lakes = data.Lakes;
            worldMap.TerrainData.LandMasses = data.Land;

            riverGenerator.GenerateRivers(worldGenParams.LocalDimensions.x, worldGenParams.RiverCount(worldSize));
            terrainGenerator.GenerateHeatMap();
            terrainGenerator.GeneratePrecipitationMap();
            terrainGenerator.GenerateBiomes();
            terrainGenerator.GenerateOreDeposits();
        }

        private void RecreateSettlements(WorldSaveData data)
        {
            settlementGenerator.PlaceSettlements();
            roadGenerator.GenerateRoads();
        }
        #endregion
    }
}