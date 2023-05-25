using UnityEngine;

namespace JS.WorldMap
{
    [CreateAssetMenu(fileName = "Biome Helper", menuName = "World Generation/Terrain/Biome Helper")]
    public class BiomeHelper : ScriptableObject
    {
        [SerializeField] private WorldGenerationParameters worldParams;
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
        [Space]
        [SerializeField] private Biome oceanSurface;
        [SerializeField] private Biome oceanDeep;
        [Space]
        [SerializeField] private Biome mountain;
        public Biome Mountain => mountain;

        public Biome GetWhittakerTableBiome(WorldTile node)
        {
            int temperatureIndex = node.TempZoneID;
            int precipitationIndex = node.PrecipitationZoneID;

            switch (temperatureIndex)
            {
                case 0:
                    if (precipitationIndex == 5) return taiga;
                    return tundra;
                case 1:
                    if (precipitationIndex >= 3) return taiga;
                    return tundra;
                case 2:
                    if (precipitationIndex >= 3) return deciduousForest;
                    if (precipitationIndex == 2) return shrubland;
                    return temperateGrassland;
                case 3:
                    if (precipitationIndex == 0) return desert;
                    if (precipitationIndex == 1) return temperateGrassland;
                    if (precipitationIndex == 4) return deciduousForest;
                    if (precipitationIndex == 5) return tropicalSeasonalForest;
                    return shrubland;
                case 4:
                    if (precipitationIndex < 2) return desert;
                    if (precipitationIndex == 2) return savanna;
                    if (precipitationIndex == 3) return tropicalSeasonalForest;
                    return jungle;
                case 5:
                    if (precipitationIndex <= 1) return desert;
                    if (precipitationIndex <= 3) return savanna;
                    return jungle;
                default: return shrubland;
            }
        }

        public Biome GetAquaticBiome(WorldTile tile)
        {
            if (tile.Altitude >= worldParams.SeaLevel * 0.5f) return oceanSurface;
            else return oceanDeep;
        }
    }
}