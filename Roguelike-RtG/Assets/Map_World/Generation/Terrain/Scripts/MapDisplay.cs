using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace JS.WorldMap
{
    public class MapDisplay : MonoBehaviour
    {
        [SerializeField] private WorldData worldMap;
        [SerializeField] private WorldGenerationParameters worldGenParams;
        [SerializeField] private PathTileHelper riverTiles;
        [SerializeField] private PathTileHelper roadTiles;
        public BiomeHelper biomeHelper;

        [Space]

        [SerializeField] private Tilemap oceanMap;
        [SerializeField] private Tilemap landMap;
        [SerializeField] private Tilemap terrainFeatureMap;
        [SerializeField] private Tilemap riverMap;
        [SerializeField] private Tilemap roadMap;
        [SerializeField] private Tilemap settlementMap;
        [SerializeField] private Tilemap infoMap;

        [Space]

        [SerializeField] private TileBase highlightTile;
        [SerializeField] private DirectionTiles directionTiles;
        [SerializeField] private TileBase coastalWaterTile;
        [SerializeField] private RuleTile bridgeVertical, bridgeHorizontal;
        [SerializeField] private Color[] dangerColors;

        public Biome biomeToHighlight { get; set; }
        public Ores resourceToHighlight { get; set; }

        private Color red = new Color(255f, 0f, 0f, 100f);


        //Called on scene load from GameEventListener
        public void DisplayWorldMap()
        {
            oceanMap.ClearAllTiles();
            landMap.ClearAllTiles();

            DisplayBiomes();
            DisplayRivers();
            DisplayRoads();
            DisplaySettlements();
        }

        private void DisplayBiomes()
        {
            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos =new Vector3Int(x, y);
                    var biome = biomeHelper.GetBiome(worldMap.TerrainData.BiomeMap[x,y]);

                    if (biome.isLand)
                    {
                        landMap.SetTile(tilePos, biome.WorldBase);
                        if (biome.WorldAccent != null)
                            terrainFeatureMap.SetTile(tilePos, biome.WorldAccent);
                    }
                    else oceanMap.SetTile(tilePos, biome.WorldBase);

                    if (worldMap.TerrainData.Coasts[x, y])
                    {
                        oceanMap.SetTile(tilePos, coastalWaterTile);
                        terrainFeatureMap.SetTile(tilePos, null);
                    }
                }
            }
        }

        private void DisplayRivers()
        {
            riverMap.ClearAllTiles();

            foreach (var river in worldMap.TerrainData.Rivers)
            {
                for (int i = 0; i < river.Nodes.Length; i++)
                {
                    var tilePos = new Vector3Int(river.Nodes[i].x, river.Nodes[i].y);
                    terrainFeatureMap.SetTile(tilePos, null);

                    RuleTile tile;
                    var existingTile = riverMap.GetTile(tilePos) as RuleTile;
                    if (existingTile != null)
                    {
                        //Debug.Log("At " + tilePos);
                        tile = riverTiles.GetIntersectionTile(existingTile, river.Nodes[i].PathDirection);
                    }
                    else tile = riverTiles.GetRiverTile(river.Nodes[i].PathDirection);
                    riverMap.SetTile(tilePos, tile);
                }
            }
        }

        private void DisplayRoads()
        {
            roadMap.ClearAllTiles();

            foreach (var road in worldMap.TerrainData.Roads)
            {
                for (int i = 0; i < road.Nodes.Length; i++)
                {
                    var tilePos = new Vector3Int(road.Nodes[i].x, road.Nodes[i].y);
                    terrainFeatureMap.SetTile(tilePos, null);

                    RuleTile tile = null;
                    var existingTile = roadMap.GetTile(tilePos) as RuleTile;
                    if (existingTile != null)
                    {
                        //Debug.Log("At " + tilePos);
                        tile = roadTiles.GetIntersectionTile(existingTile, road.Nodes[i].PathDirection);
                    }
                    else tile = roadTiles.GetRiverTile(road.Nodes[i].PathDirection);
                    roadMap.SetTile(tilePos, tile);
                }
            }

            foreach(var bridge in worldMap.TerrainData.Bridges)
            {
                var tilePos = new Vector3Int(bridge.x, bridge.y);
                if (bridge.isVertical) roadMap.SetTile(tilePos, bridgeVertical);
                else roadMap.SetTile(tilePos, bridgeHorizontal);
            }
        }

        private void DisplaySettlements()
        {
            settlementMap.ClearAllTiles();

            foreach (var settlement in worldMap.SettlementData.Settlements)
            {
                var tilePos = new Vector3Int(settlement.X, settlement.Y);
                settlementMap.SetTile(tilePos, worldMap.SettlementData.Types[settlement.TypeID].settlementTile);
            }
        }

        public void ResetInfoMap()
        {
            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos = new Vector3Int(x, y);
                    infoMap.SetTile(tilePos, highlightTile);
                    infoMap.SetColor(tilePos, Color.clear);
                }
            }
        }

        #region - Climate Values -
        public void DisplayHeatMap()
        {
            ResetInfoMap();

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos = new Vector3Int(x, y);
                    var node = worldMap.GetNode(x, y);
                    var zone = worldGenParams.TemperatureZones[node.TempZoneID];
                    infoMap.SetColor(tilePos, zone.HighlightColor);

                }
            }
        }

        public void DisplayMoistureMap()
        {
            ResetInfoMap();

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos = new Vector3Int(x, y);
                    var node = worldMap.GetNode(x, y);
                    var zone = worldGenParams.PrecipitationZones[node.PrecipitationZoneID];
                    infoMap.SetColor(tilePos, zone.HighlightColor);
                }
            }
        }

        public void DisplayWindMap()
        {
            ResetInfoMap();

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos = new Vector3Int(x, y);
                    var node = worldMap.GetNode(x, y);
                    infoMap.SetTile(tilePos, directionTiles.GetTile(node.windDirection));
                }
            }
        }
        #endregion

        public void DisplayDangerMap()
        {
            ResetInfoMap();

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos = new Vector3Int(x, y);
                    var node = worldMap.GetNode(x, y);
                    infoMap.SetColor(tilePos, dangerColors[node.DangerTier]);
                }
            }
        }

        #region - Highlight Features -
        public void HighlightTectonicPlates()
        {
            var list = new List<WorldTile>();

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var node = worldMap.GetNode(x, y);
                    if (node.isTectonicPoint) list.Add(node);
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    Debug.DrawLine(new Vector3(list[i].x, list[i].y), new Vector3(list[j].x, list[j].y), Color.blue, 20f);
                }
            }

            foreach(var point in worldMap.TerrainData.PlateBorders)
            {
                var tilePos = new Vector3Int(point.x, point.y);
                infoMap.SetColor(tilePos, Color.red);
            }
        }

        public void HighlightBiome()
        {
            ResetInfoMap();

            foreach (var biome in worldMap.TerrainData.BiomeGroups)
            {
                if (biome.BiomeID != biomeToHighlight.ID) continue;
                for (int i = 0; i < biome.Nodes.Count; i++)
                {
                    var tilePos = new Vector3Int(biome.Nodes[i].x, biome.Nodes[i].y);
                    infoMap.SetColor(tilePos, Color.red);
                }
            }
        }

        public void HighlightMountains()
        {
            ResetInfoMap();

            foreach (var mountain in worldMap.TerrainData.Mountains)
            {
                for (int i = 0; i < mountain.Nodes.Count; i++)
                {
                    var tilePos = new Vector3Int(mountain.Nodes[i].x, mountain.Nodes[i].y);
                    infoMap.SetColor(tilePos, Color.red);
                }
            }
        }

        public void HighlightIslands()
        {
            ResetInfoMap();

            foreach (var landMass in worldMap.TerrainData.LandMasses)
            {
                if (landMass.Size == LandSize.Continent) continue;

                for (int i = 0; i < landMass.GridNodes.Length; i++)
                {
                    var tilePos = new Vector3Int(landMass.GridNodes[i].x, landMass.GridNodes[i].y);
                    infoMap.SetColor(tilePos, Color.red);
                }
            }
        }

        public void HighlightLakes()
        {
            ResetInfoMap();

            foreach (var lake in worldMap.TerrainData.Lakes)
            {
                for (int i = 0; i < lake.GridNodes.Length; i++)
                {
                    var tilePos = new Vector3Int(lake.GridNodes[i].x, lake.GridNodes[i].y);
                    infoMap.SetColor(tilePos, Color.red);
                }
            }
        }

        public void HighlightSettlements()
        {
            ResetInfoMap();

            foreach (var settlement in worldMap.SettlementData.Settlements)
            {
                for (int i = 0; i < settlement.Territory.Count; i++)
                {
                    var tilePos = new Vector3Int(settlement.Territory[i].x, settlement.Territory[i].y);
                    infoMap.SetColor(tilePos, Color.red);
                }
            }
        }

        public void HighlightCoasts()
        {
            ResetInfoMap();

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos = new Vector3Int(x, y);
                    if (worldMap.TerrainData.Coasts[x, y])
                    {
                        infoMap.SetColor(tilePos, Color.red);
                    }
                }
            }
        }

        public void HighlightResources()
        {
            ResetInfoMap();
            int totalCount = 0;
            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos = new Vector3Int(x, y);

                    switch (resourceToHighlight)
                    {
                        case Ores.Coal:
                            if (worldMap.TerrainData.CoalMap[x,y] > 0)
                            {
                                infoMap.SetColor(tilePos, Color.red);
                                totalCount++;
                            }
                            break;
                        case Ores.Copper:
                            if (worldMap.TerrainData.CopperMap[x, y] > 0)
                            {
                                infoMap.SetColor(tilePos, Color.red);
                                totalCount++;
                            }
                            break;
                        case Ores.Iron:
                            if (worldMap.TerrainData.IronMap[x, y] > 0)
                            {
                                infoMap.SetColor(tilePos, Color.red);
                                totalCount++;
                            }
                            break;
                        case Ores.Silver:
                            if (worldMap.TerrainData.SilverMap[x, y] > 0)
                            {
                                infoMap.SetColor(tilePos, Color.red);
                                totalCount++;
                            }
                            break;
                        case Ores.Gold:
                            if (worldMap.TerrainData.GoldMap[x, y] > 0)
                            {
                                infoMap.SetColor(tilePos, Color.red);
                                totalCount++;
                            }
                            break;
                        case Ores.Gemstones:
                            if (worldMap.TerrainData.GemstoneMap[x, y] > 0)
                            {
                                infoMap.SetColor(tilePos, Color.red);
                                totalCount++;
                            }
                            break;
                        case Ores.Mithril:
                            if (worldMap.TerrainData.MithrilMap[x, y] > 0)
                            {
                                infoMap.SetColor(tilePos, Color.red);
                                totalCount++;
                            }
                            break;
                        case Ores.Adamantine:
                            if (worldMap.TerrainData.AdmanatineMap[x, y] > 0)
                            {
                                infoMap.SetColor(tilePos, Color.red);
                                totalCount++;
                            }
                            break;
                    }
                }
            }

            Debug.Log(resourceToHighlight.ToString() + ": " + totalCount);
        }

        public void HighlightNode(WorldTile node)
        {
            if (node == null) return;
            ResetInfoMap();
            //var worldMap = WorldMap.instance;

            var settlement = worldMap.SettlementData.FindSettlement(node.x, node.y);
            if (settlement != null)
            {
                HighlightSettlement(settlement);
                return;
            }

            var pos = worldMap.GetWorldPosition(node.x, node.y);
            Vector3Int tilePos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
            infoMap.SetColor(tilePos, Color.red);
        }

        private void HighlightSettlement(Settlement settlement)
        {
            ResetInfoMap();

            for (int i = 0; i < settlement.Territory.Count; i++)
            {
                var pos = worldMap.GetWorldPosition(settlement.Territory[i].x, settlement.Territory[i].y);
                Vector3Int tilePos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
                infoMap.SetColor(tilePos, Color.red);
            }
        }
        #endregion
    }
}

public enum Ores
{
    Coal,
    Copper,
    Iron,
    Silver,
    Gold,
    Gemstones,
    Mithril,
    Adamantine,
}