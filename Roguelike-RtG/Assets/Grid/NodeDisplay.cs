using UnityEngine;
using TMPro;

namespace JS.WorldMap
{
    public class NodeDisplay : MonoBehaviour
    {
        [SerializeField] private WorldGenerationParameters worldGenerationParameters;
        [SerializeField] private TerrainData terrainData;
        [SerializeField] private SettlementData settlementData;

        [SerializeField] private TMP_Text nodeCoordinates;
        [SerializeField] private TMP_Text nodeElevation;
        [SerializeField] private TMP_Text nodeBiome;

        [SerializeField] private TMP_Text nodeTemperature;
        [SerializeField] private TMP_Text nodePrecipitation;
        [SerializeField] private TMP_Text nodeRiverText;

        [Space]

        [SerializeField] private GameObject mountainPanel;
        [SerializeField] private TMP_Text nodeMountainRange;

        [Space]

        [SerializeField] private GameObject settlementPanel;
        [SerializeField] private TMP_Text settlementText;

        public void DisplayNodeValues(WorldTile node)
        {
            gameObject.SetActive(node != null);
            if (node == null) return;

            MountainDisplay(node);

            DisplaySettlementInfo(node);

            nodeCoordinates.text = "[X,Y] = " + node.x + "," + node.y;
            nodeElevation.text = "Elevation = " + terrainData.HeightMap[node.x, node.y];

            if (node.hasBiome) nodeBiome.text = "Biome: " + worldGenerationParameters.Biomes[node.BiomeID].BiomeName;

            nodeTemperature.text = "Avg. Temperature: " + (Temperature.FloatToCelsius(terrainData.HeatMap[node.x, node.y])).ToString("00") + "\u00B0" + "C";
            nodePrecipitation.text = "Annual Rainfall: " + (terrainData.MoistureMap[node.x, node.y] * 400).ToString("00");

            if (node.rivers.Count > 0) nodeRiverText.text = "River (" + node.rivers.Count + ")";
            else nodeRiverText.text = "";
        }

        private void DisplaySettlementInfo(WorldTile node)
        {
            var settlement = settlementData.FindSettlement(node.x, node.y);
            settlementPanel.SetActive(settlement  != null);
            if (settlement == null) return;

            settlementText.text = settlement.name;
            settlementText.text += "\nTribe: " + settlement.tribe.name + "\n";
            settlementText.text += settlement.type.TypeName;

            settlementText.text += "\nPopulation: " + settlement.population.ToString() + settlement.type.ToString();
            settlementText.text += "\nTerritory Size: " + settlement.Territory.Count;
        }

        private void MountainDisplay(WorldTile node)
        {
            mountainPanel.SetActive(node.Mountain != null);

            if (node.Mountain != null)
            {
                nodeMountainRange.text = "Mountain (" + node.Mountain.ID + ")" + "\n" +
                    "Avg: " + node.Mountain.AverageAltitude + "\n" +
                    " Peak: " + node.Mountain.peakAltitude;
            }
        }
    }
}