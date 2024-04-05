using UnityEngine;
using TMPro;
using JS.World.Map.Features;

namespace JS.World.Map
{
    public class NodeDisplay : MonoBehaviour
    {
        [SerializeField] private BiomeHelper biomeHelper;

        [Space]

        [SerializeField] private TMP_Text displayText;

        public void HideDisplay()
        {
            gameObject.SetActive(false);
        }

        public void DisplayNodeValues(WorldTile node)
        {
            gameObject.SetActive(node != null);
            if (node == null) return;           

            displayText.text = "[X,Y] = " + node.x + "," + node.y;
            displayText.text += "\nElevation = " + Features.TerrainData.HeightMap[node.x, node.y];

            displayText.text += "\nBiome: " + biomeHelper.GetBiome(node.BiomeID).BiomeName;

            displayText.text += "\nAvg. Temperature: " + (Temperature.FloatToCelsius(Features.TerrainData.HeatMap[node.x, node.y])).ToString("00") + "\u00B0" + "C";
            displayText.text += "\nAnnual Rainfall: " + (Features.TerrainData.MoistureMap[node.x, node.y] * 400).ToString("00");

            displayText.text += $"\nAir Pressure: {node.airPressure}";
            displayText.text += $"\nWater Capacity: {node.WaterCapacity}";

            var river = Features.TerrainData.FindRiverAt(node.x, node.y, out var index);
            if (river != null)
            {
                displayText.text += "\nRiver: " + river.ID;
                for (int i = 0; i < river.Nodes.Length; i++)
                {
                    if (river.Nodes[i].x == node.x && river.Nodes[i].y == node.y)
                    {
                        displayText.text += ", Width: " + river.Nodes[i].Size;
                    }
                }
            }

            DisplaySettlementInfo(node);

            MountainDisplay(node);
        }

        private void DisplaySettlementInfo(WorldTile node)
        {
            var settlement = SettlementData.FindSettlement(node.x, node.y);
            if (settlement == null) return;
            displayText.text += "\n \n";

            displayText.text += $"{settlement.Name} {settlement.Coordinates}\n";
            displayText.text += $"\nTribe: {settlement.Race}\n";
            displayText.text += $"{settlement.Category}";

            displayText.text += "\nPopulation: " + settlement.Population.ToString();
            displayText.text += "\nTerritory Size: " + settlement.Territory.Count;
            displayText.text += "\nDefense:" + settlement.Defensibility;
            for (int i = 0; i < settlement.Facilities.Count; i++)
            {
                displayText.text += "\n" + settlement.Facilities[i].Name;
            }
        }

        private void MountainDisplay(WorldTile node)
        {
            if (node.Mountain ==  null) return;

            displayText.text += "\n \nMountain (" + node.Mountain.ID + ")" + "\n" +
                "Avg: " + node.Mountain.AverageAltitude + "\n" +
                " Peak: " + node.Mountain.peakAltitude;
        }
    }
}