using UnityEngine;
using TMPro;

public class NodeDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text nodeCoordinates;
    [SerializeField] private TMP_Text nodeElevation;
    [SerializeField] private TMP_Text nodeTerrain;
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

        nodeCoordinates.text = "[X,Y] = " + node.x + "," + node.y;
        nodeElevation.text = "Elevation = " + node.altitude;
        nodeTerrain.text = "Terrain: " + node.terrainType.TerrainName;

        if (node.biome != null) nodeBiome.text = "Biome: " + node.biome.BiomeName;

        nodeTemperature.text = "Temperature: " + node.temperatureZone.name + 
            " " + (node.avgAnnualTemperature_C).ToString("00") + "\u00B0" + "C";
        nodePrecipitation.text = node.precipitationZone.name +  ": " + node.annualPrecipitation_cm.ToString("00");

        if (node.rivers.Count > 0) nodeRiverText.text = "River (" + node.rivers.Count + ")";
        else nodeRiverText.text = "";

        MountainDisplay(node);
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