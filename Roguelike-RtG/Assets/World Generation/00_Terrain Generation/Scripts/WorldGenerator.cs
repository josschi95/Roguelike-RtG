using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using JS.EventSystem;
using TMPro;
using JS.CommandSystem;

namespace JS.WorldMap.Generation
{
    public class WorldGenerator : MonoBehaviour
    {
        public System.Random rng { get; private set; }
        public int seed { get; private set; }
            
        [SerializeField] private WorldSize worldSize;
        [SerializeField] private WorldGenerationParameters mapFeatures;
        [SerializeField] private WorldData worldMap;
        [SerializeField] private SettlementData settlementData;
        [SerializeField] private PlayerData playerData;
        [SerializeField] private TimeKeeper timeKeeper;

        [Space]

        [SerializeField] private TerrainGenerator mapGenerator;
        [SerializeField] private SettlementGenerator settlementGenerator;
        //private FloraFaunaGenerator floraFaunaGenerator;
        //private ResourceGenerator resourceGenerator;
        //private HistoryGenerator historyGenerator

        [Space]

        [SerializeField] private Image progressBar;
        [SerializeField] private TMP_Text progressText;

        [Header("Game Events")]
        [SerializeField] private GameEvent worldGenCompleteEvent;

        [Space]

        [SerializeField] private SaveWorldDataCommand saveWorldDataCommand;

        private float initialTime;
        //private void Awake() => SetRandomSeed();

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
            seed = value;
            worldMap.Seed = seed;
        }
        #endregion

        //Called from World Generation Settings Menu
        public void OnBeginWorldGeneration()
        {
            timeKeeper.ResetTime();

            StartCoroutine(GenerateWorld());

            //Generate Plant and Wildlife

            //Generate Resources

            //Generate Settlements

            //Generate Historical Figures, Locations, Items, and Events
        }

        private IEnumerator GenerateWorld()
        {
            yield return StartCoroutine(HandleTerrainGeneration());

            yield return StartCoroutine(settlementGenerator.PlaceSettlements());
            progressBar.fillAmount = 0.9f;
            yield return StartCoroutine(UpdateProgress("Finalizing"));

            PlacePlayerAtStart();

            saveWorldDataCommand?.Invoke();
            worldGenCompleteEvent?.Invoke();
        }

        private void SetNewWorldValues()
        {
            rng = new System.Random(seed);

            mapGenerator.SetInitialValues(worldSize, seed); //also creates grid
            settlementGenerator.SetInitialValues(worldSize);

            PlantSeeds();
        }

        private void PlantSeeds()
        {
            var seedMap = new int[worldMap.Width, worldMap.Height];
            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    seedMap[x, y] = rng.Next();
                }
            }
            worldMap.TerrainData.SeedMap = seedMap;
        }

        private IEnumerator HandleTerrainGeneration()
        {
            initialTime = Time.realtimeSinceStartup;
            progressText.text = "Loading";
            progressBar.fillAmount = 0;
            SetNewWorldValues();
            yield return StartCoroutine(UpdateProgress("Generating Tectonic Plates"));

            mapGenerator.PlaceTectonicPlates();
            progressBar.fillAmount = 0.1f;
            yield return StartCoroutine(UpdateProgress("Generating Land Masses"));

            mapGenerator.GenerateHeightMap();
            progressBar.fillAmount = 0.2f;
            yield return StartCoroutine(UpdateProgress("Eroding Land Masses"));

            mapGenerator.ErodeLandMasses();
            progressBar.fillAmount = 0.2f;
            yield return StartCoroutine(UpdateProgress("Allocating Height Map"));

            mapGenerator.SetNodeAltitudeValues();
            progressBar.fillAmount = 0.2f;
            yield return StartCoroutine(UpdateProgress("Identifying Mountains"));

            mapGenerator.IdentifyCoasts();
            mapGenerator.IdentifyMountains();
            progressBar.fillAmount = 0.3f;
            yield return StartCoroutine(UpdateProgress("Identifying Bodies of Water"));

            yield return StartCoroutine(mapGenerator.IdentifyBodiesOfWater());
            progressBar.fillAmount = 0.3f;
            yield return StartCoroutine(UpdateProgress("Identifying Land Masses"));

            yield return StartCoroutine(mapGenerator.IdentifyLandMasses());
            progressBar.fillAmount = 0.3f;
            yield return StartCoroutine(UpdateProgress("Generating Rivers"));

            mapGenerator.GenerateRivers();
            progressBar.fillAmount = 0.5f;
            yield return StartCoroutine(UpdateProgress("Generating Heat Map"));

            mapGenerator.GenerateHeatMap();
            progressBar.fillAmount = 0.6f;
            yield return StartCoroutine(UpdateProgress("Generating Precipitation Map"));

            mapGenerator.GeneratePrecipitationMap();
            progressBar.fillAmount = 0.7f;
            yield return StartCoroutine(UpdateProgress("Generating Biomes"));

            mapGenerator.GenerateBiomes();
            progressBar.fillAmount = 0.8f;
            yield return StartCoroutine(UpdateProgress("Generating Settlements"));
        }

        private IEnumerator UpdateProgress(string message)
        {
            //Debug.Log(progressText.text + ": " + (Time.realtimeSinceStartup - initialTime));
            progressText.text = message;
            yield return new WaitForSeconds(0.01f);
            initialTime = Time.realtimeSinceStartup;
        }

        private void PlacePlayerAtStart()
        {
            int index = rng.Next(0, settlementData.Settlements.Length);
            var settlement = settlementData.Settlements[index];
            
            playerData.worldX = settlement.X;
            playerData.worldY = settlement.Y;

            playerData.regionX = Mathf.FloorToInt(mapFeatures.RegionDimensions.x * 0.5f);
            playerData.regionY = Mathf.FloorToInt(mapFeatures.RegionDimensions.y * 0.5f);

            playerData.localX = Mathf.FloorToInt(mapFeatures.LocalDimensions.x * 0.5f);
            playerData.localY = Mathf.FloorToInt(mapFeatures.LocalDimensions.y * 0.5f);
        }
    }
}