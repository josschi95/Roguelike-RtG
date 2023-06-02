using UnityEngine;
using TMPro;

namespace JS.WorldMap
{
    public class NodeDisplay : MonoBehaviour
    {
        [SerializeField] private WorldData worldData;
        [SerializeField] private WorldGenerationParameters worldGenParams;
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
            displayText.text += "\nElevation = " + worldData.TerrainData.HeightMap[node.x, node.y];

            displayText.text += "\nBiome: " + biomeHelper.GetBiome(node.BiomeID).BiomeName;

            displayText.text += "\nAvg. Temperature: " + (Temperature.FloatToCelsius(worldData.TerrainData.HeatMap[node.x, node.y])).ToString("00") + "\u00B0" + "C";
            displayText.text += "\nAnnual Rainfall: " + (worldData.TerrainData.MoistureMap[node.x, node.y] * 400).ToString("00");

            var river = worldData.TerrainData.FindRiverAt(node.x, node.y, out var index);
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
            var settlement = worldData.SettlementData.FindSettlement(node.x, node.y);
            if (settlement == null) return;
            displayText.text += "\n \n";

            displayText.text += settlement.Name;
            displayText.text += "\nTribe: " + worldData.SettlementData.Tribes[settlement.TribeID].name + "\n";
            displayText.text += worldData.SettlementData.Types[settlement.TypeID].TypeName;

            displayText.text += "\nPopulation: " + settlement.Population.ToString();
            displayText.text += "\nTerritory Size: " + settlement.Territory.Count;
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