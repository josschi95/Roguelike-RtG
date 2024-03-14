using JS.World.Map.Features;

namespace JS.World.Map
{
    public class WorldSaveData
    {
        //NOTE//
        //A number of these values can be re-generated using the seed quickly enough that there's no need to actually save them. 
        //Seriously, I don't need a JSON with 5 sets of 40,000 item length arrays
        //However, the features (Mountains, Lakes, Rivers, Islands, etc.) take quite a bit more time and should definitely be saved

        public int seed; //this needs to be saved
        //public int[] seedMap; //No need to save

        //Time              // all of this info needs to be saved
        public int seconds; 
        public int minutes;
        public int hours;
        public int days;
        public int months;
        public int years;

        //Terrain Data
        public int mapWidth, mapHeight; //this needs to be saved if I have variable map sizes
        /*
        public float[] heightMap; //No need to save
        public float[] heatMap; //No need to save
        public float[] moistureMap; //No need to save
        //public float[] airPressureMap;
        public Compass[] windMap; //No need to save, this isn't even being used
        public bool[] coasts;

        //Resources              //No need to save
        public float[] CoalMap;
        public float[] CopperMap;
        public float[] IronMap;
        public float[] SilverMap;
        public float[] GoldMap;
        public float[] MithrilMap;
        public float[] AdmanatineMap;
        public float[] GemstoneMap;

        //Features              //these should be saved
        public int[] biomeMap; //reference to the ID of each biome
        public BiomeGroup[] BiomeGroups;
        public MountainRange[] Mountains;
        public River[] Rivers;
        */
        public Lake[] Lakes;
        public LandMass[] Land;


        //Settlement Data
        public Settlement[] Settlements;
        public Road[] Roads;    //Roads definitely needs to be saved, the pathfinding takes some time
        public Bridge[] Bridges;
    }
}