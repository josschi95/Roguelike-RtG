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

        //private void Awake() => SetRandomSeed();

        //Set from the World Generation Settings Menu
        public void SetSize(int size)
        {
            worldSize = (WorldSize)size;
        }

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

        //Not currently being used. 
        public void SetNewWorldValues()
        {
            rng = new System.Random(seed);

            mapGenerator.SetInitialValues(worldSize, seed);
            settlementGenerator.SetInitialValues(worldSize);
        }

        //Called from World Generation Settings Menu
        public void OnBeginWorldGeneration()
        {
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

            PlacePlayerAtStart();

            progressBar.fillAmount = 0.9f;
            yield return new WaitForSeconds(0.01f);

            saveWorldDataCommand?.Invoke();
            worldGenCompleteEvent?.Invoke();
        }

        private IEnumerator HandleTerrainGeneration()
        {
            var initialTime = Time.realtimeSinceStartup;
            progressText.text = "Loading";
            progressBar.fillAmount = 0;
            SetNewWorldValues();
            //Debug.Log("SetNewWorldValues: " + (Time.realtimeSinceStartup - initialTime));
            progressText.text = "Generating Tectonic Plates";
            yield return new WaitForSeconds(0.01f);
            //initialTime = Time.realtimeSinceStartup;

            mapGenerator.PlaceTectonicPlates();
            progressBar.fillAmount = 0.1f;
            //Debug.Log("PlaceTectonicPlates: " + (Time.realtimeSinceStartup - initialTime));
            progressText.text = "Generating Land Masses";
            yield return new WaitForSeconds(0.01f);
            //initialTime = Time.realtimeSinceStartup;

            mapGenerator.GenerateHeightMap();
            progressBar.fillAmount = 0.2f;
            //Debug.Log("GenerateHeightMap: " + (Time.realtimeSinceStartup - initialTime));
            progressText.text = "Eroding Land Masses";
            yield return new WaitForSeconds(0.01f);
            //initialTime = Time.realtimeSinceStartup;

            mapGenerator.ErodeLandMasses();
            progressBar.fillAmount = 0.2f;
            //Debug.Log("ErodeLandMasses: " + (Time.realtimeSinceStartup - initialTime));
            progressText.text = "Allocating Height Map";
            yield return new WaitForSeconds(0.01f);
            //initialTime = Time.realtimeSinceStartup;

            mapGenerator.SetNodeAltitudeValues();
            progressBar.fillAmount = 0.2f;
            //Debug.Log("SetNodeAltitudeValues: " + (Time.realtimeSinceStartup - initialTime));
            progressText.text = "Identifying Mountains";
            yield return new WaitForSeconds(0.01f);
            //initialTime = Time.realtimeSinceStartup;

            mapGenerator.IdentifyCoasts();
            mapGenerator.IdentifyMountains();
            progressBar.fillAmount = 0.3f;
            //Debug.Log("IdentifyMountains: " + (Time.realtimeSinceStartup - initialTime));
            progressText.text = "Identifying Bodies of Water";
            yield return new WaitForSeconds(0.01f);
            //initialTime = Time.realtimeSinceStartup;

            // !!! This method here is taking up the vast majority of generation time !!!
            yield return StartCoroutine(mapGenerator.IdentifyBodiesOfWater());
            progressBar.fillAmount = 0.3f;
            //Debug.Log("IdentifyBodiesOfWater: " + (Time.realtimeSinceStartup - initialTime));
            progressText.text = "Identifying Land Masses";
            yield return new WaitForSeconds(0.01f);
            //initialTime = Time.realtimeSinceStartup;

            yield return StartCoroutine(mapGenerator.IdentifyLandMasses());
            progressBar.fillAmount = 0.3f;
            //Debug.Log("IdentifyLandMasses: " + (Time.realtimeSinceStartup - initialTime));
            progressText.text = "Generating Rivers";
            yield return new WaitForSeconds(0.01f);
            //initialTime = Time.realtimeSinceStartup;

            mapGenerator.GenerateRivers();
            progressBar.fillAmount = 0.5f;
            //Debug.Log("GenerateRivers: " + (Time.realtimeSinceStartup - initialTime));
            progressText.text = "Generating Heat Map";
            yield return new WaitForSeconds(0.01f);
            //initialTime = Time.realtimeSinceStartup;

            mapGenerator.GenerateHeatMap();
            progressBar.fillAmount = 0.6f;
            //Debug.Log("GenerateHeatMap: " + (Time.realtimeSinceStartup - initialTime));
            progressText.text = "Generating Precipitation Map";
            yield return new WaitForSeconds(0.01f);
            //initialTime = Time.realtimeSinceStartup;

            mapGenerator.GeneratePrecipitationMap();
            progressBar.fillAmount = 0.7f;
            //Debug.Log("GeneratePrecipitationMap: " + (Time.realtimeSinceStartup - initialTime));
            progressText.text = "Generating Biomes";
            yield return new WaitForSeconds(0.01f);
            //initialTime = Time.realtimeSinceStartup;

            mapGenerator.GenerateBiomes();
            progressBar.fillAmount = 0.8f;
            //Debug.Log("GenerateBiomes: " + (Time.realtimeSinceStartup - initialTime));
            progressText.text = "Generating Settlements";
            yield return new WaitForSeconds(0.01f);
            //initialTime = Time.realtimeSinceStartup;
        }

        private void PlacePlayerAtStart()
        {
            int index = rng.Next(0, settlementData.Settlements.Length);
            var settlement = settlementData.Settlements[index];

            playerData.worldX = settlement.X;
            playerData.worldY = settlement.Y;
        }
    }
}