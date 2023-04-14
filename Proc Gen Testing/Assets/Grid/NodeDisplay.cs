using UnityEngine;
using TMPro;
using JS.WorldGeneration;

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

    public void DisplayNodeValues(TerrainNode node)
    {
        gameObject.SetActive(node != null);
        if (node == null) return;

        if (node.Settlement != null)
        {
            DisplaySettlementInfo(node.Settlement);
            mountainPanel.SetActive(false);
            return;
        }

        nodeCoordinates.text = "[X,Y] = " + node.x + "," + node.y;
        nodeElevation.text = "Elevation = " + node.altitude;

        if (node.biome != null) nodeBiome.text = "Biome: " + node.biome.BiomeName;

        nodeTemperature.text = "Temperature: " + node.temperatureZone.name + 
            " " + (node.avgAnnualTemperature_C).ToString("00") + "\u00B0" + "C";
        nodePrecipitation.text = node.precipitationZone.name +  ": " + node.annualPrecipitation_cm.ToString("00");

        if (node.rivers.Count > 0) nodeRiverText.text = "River (" + node.rivers.Count + ")";
        else nodeRiverText.text = "";

        MountainDisplay(node);
    }

    private void DisplaySettlementInfo(Settlement settlement)
    {
        nodeCoordinates.text = settlement.name;
        nodeElevation.text = "[X,Y] = " + settlement.Node.x + "," + settlement.Node.y;
        nodeBiome.text = "Tribe: " + settlement.tribe.name;
        nodeTemperature.text = "Population: " + settlement.population.ToString();

        nodePrecipitation.text = "";
        nodeRiverText.text = "";
    }

    private void MountainDisplay(TerrainNode node)
    {
        mountainPanel.SetActive(node.Mountain != null);

        if (node.Mountain != null)
        {
            nodeMountainRange.text = "Mountain (" + node.Mountain.ID + ")" + "\n" +
                "Avg: " + node.Mountain.AverageAltitude +"\n" +
                " Peak: " + node.Mountain.peakAltitude;
        }
    }
}