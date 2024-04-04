using UnityEngine;

namespace JS.World.Map.Features
{
    [CreateAssetMenu(fileName = "Biome Helper", menuName = "World Generation/Terrain/Biome Helper")]
    public class BiomeHelper : ScriptableObject
    {
        [SerializeField] private Biome[] biomes;
        public Biome[] Biomes => biomes;

        [Space]

        [SerializeField] private Biome tundra;
        [SerializeField] private Biome taiga;
        [SerializeField] private Biome temperateGrassland;
        [SerializeField] private Biome shrubland;
        [SerializeField] private Biome deciduousForest;
        [SerializeField] private Biome desert;
        [SerializeField] private Biome tropicalSeasonalForest;
        [SerializeField] private Biome savanna;
        [SerializeField] private Biome jungle;
        [SerializeField] private Biome mountain;

        [Space]

        [SerializeField] private Biome river;
        [SerializeField] private Biome lake;
        [SerializeField] private Biome oceanSurface;
        [SerializeField] private Biome oceanDeep;

        public Biome Mountain => mountain;
        public Biome Lake => lake;

        public Biome GetWhittakerTableBiome(WorldTile node)
        {
            int temperatureIndex = node.TemperatureIndex;
            int precipitationIndex = node.PrecipitationZoneID;

            switch (temperatureIndex)
            {
                case 0: // Coldest
                    if (precipitationIndex == 5) return taiga;
                    return tundra;
                case 1: // Colder
                    if (precipitationIndex >= 3) return taiga;
                    return tundra;
                case 2: // Cold
                    if (precipitationIndex >= 3) return deciduousForest;
                    if (precipitationIndex == 2) return shrubland;
                    return temperateGrassland;
                case 3: // Warm
                    if (precipitationIndex == 0) return desert;
                    if (precipitationIndex == 1) return temperateGrassland;
                    if (precipitationIndex == 4) return deciduousForest;
                    if (precipitationIndex == 5) return tropicalSeasonalForest;
                    return shrubland;
                case 4: // Warmer
                    if (precipitationIndex < 2) return desert;
                    if (precipitationIndex == 2) return savanna;
                    if (precipitationIndex == 3) return tropicalSeasonalForest;
                    return jungle;
                case 5: // Warmest
                    if (precipitationIndex <= 1) return desert;
                    if (precipitationIndex <= 3) return savanna;
                    return jungle;
                default: return shrubland;
            }
        }

        public Biome GetOceanBiome(WorldTile tile)
        {
            if (tile.Altitude >= WorldParameters.SEA_LEVEL * 0.5f) return oceanSurface;
            else return oceanDeep;
        }

        public Biome GetBiome(int ID)
        {
            for (int i = 0; i < biomes.Length; i++)
            {
                if (biomes[i].ID == ID) return biomes[i];
            }
            throw new System.Exception("Biome ID not found.");
        }
    }
}