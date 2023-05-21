using UnityEngine;
using TMPro;

namespace JS.WorldMap
{
    public class NodeDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text nodeCoordinates;
        [SerializeField] private TMP_Text nodeElevation;
        [SerializeField] private TMP_Text nodeBiome;

        [SerializeField] private TMP_Text nodeTemperature;
        [SerializeField] private TMP_Text nodePrecipitation;
        [SerializeField] private TMP_Text nodeRiverText;

        [Space]

        [SerializeField] private GameObject mountainPanel;
        [SerializeField] private TMP_Text nodeMountainRange;

        public void DisplayNodeValues(WorldTile node)
        {
            gameObject.SetActive(node != null);
            if (node == null) return;

            MountainDisplay(node);

            if (node.Settlement != null)
            {
                DisplaySettlementInfo(node.Settlement);
                return;
            }

            nodeCoordinates.text = "[X,Y] = " + node.x + "," + node.y;
            nodeElevation.text = "Elevation = " + node.altitude;

            if (node.PrimaryBiome != null) nodeBiome.text = "Biome: " + node.PrimaryBiome.BiomeName;

            nodeTemperature.text = "Temperature: " + node.temperatureZone.name +
                " " + (node.avgAnnualTemperature_C).ToString("00") + "\u00B0" + "C";
            nodePrecipitation.text = node.precipitationZone.name + ": " + node.annualPrecipitation_cm.ToString("00");

            if (node.rivers.Count > 0) nodeRiverText.text = "River (" + node.rivers.Count + ")";
            else nodeRiverText.text = "";
        }

        private void DisplaySettlementInfo(Settlement settlement)
        {
            nodeCoordinates.text = settlement.name;
            nodeElevation.text = "[X,Y] = " + settlement.Node.x + "," + settlement.Node.y;
            nodeBiome.text = "Tribe: " + settlement.tribe.name;
            nodeTemperature.text = "Population: " + settlement.population.ToString() + settlement.type.ToString();

            nodePrecipitation.text = "Territory Size: " + settlement.territory.Count;
            nodeRiverText.text = "";
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