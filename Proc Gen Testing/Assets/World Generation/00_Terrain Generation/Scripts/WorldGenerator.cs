using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace JS.WorldGeneration
{
    public class WorldGenerator : MonoBehaviour
    {
        public System.Random rng { get; private set; }

        [field: SerializeField] public int seed { get; private set; }

        [SerializeField] private TerrainGenerator mapGenerator;
        [SerializeField] private SettlementGenerator settlementGenerator;
        //private FloraFaunaGenerator floraFaunaGenerator;
        //private ResourceGenerator resourceGenerator;
        //private HistoryGenerator historyGenerator

        [SerializeField] private WorldGenerationParameters mapFeatures;
        [SerializeField] private WorldSize worldSize;

        //private void Awake() => SetRandomSeed();

        //Set from the World Generation Settings Menu
        public void SetSize(int size)
        {
            worldSize = (WorldSize)size;
        }

        //Called from World Generation Settings Menu
        public void SetRandomSeed()
        {
            seed = Random.Range(-100000, 100000);
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
            rng = new System.Random(seed);

            mapGenerator.SetInitialValues(worldSize, seed);
            settlementGenerator.SetInitialValues(worldSize);

            //Generate Map - Terrain and Biomes
            mapGenerator.BeginGenerateTerrain();

            //Generate Plant and Wildlife

            //Generate Resources

            //Generate Settlements

            //Generate Historical Figures, Locations, Items, and Events
        }
    }
}