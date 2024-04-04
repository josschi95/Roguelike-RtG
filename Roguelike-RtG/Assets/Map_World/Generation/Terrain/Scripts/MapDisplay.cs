using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using JS.World.Map.Features;

namespace JS.World.Map
{
    public class MapDisplay : MonoBehaviour
    {
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

        [SerializeField] private RuleTile[] settlementTiles;

        public Biome biomeToHighlight { get; set; }
        public Ores resourceToHighlight { get; set; }

        private Color red = new Color(255f, 0f, 0f, 100f);

        private Color[] _heatColors =
        {
            new Color(0.0f, 1.0f, 1.0f), // Coldest #00FFFF
            new Color(0.666f, 1.0f, 1.0f), // Colder #AAFFFF
            new Color(0.0f, 0.91f, 0.53f), // Cold #00E786
            new Color(1.0f, 1.0f, 0.39f), // Warm #FFFF63
            new Color(1.0f, 0.4f, 0.0f), // Warmer #FF6500
            new Color(1.0f, 0.0f, 0.0f), // Warmest #FF0000
        };

        private Color[] _wetColors =
        {
            new Color(1.0f, 0.55f, 0.066f), // Dryest #FF8B11
            new Color(0.96f, 0.96f, 0.09f), // Dryer #F5F517
            new Color(0.31f, 1.0f, 0.0f), // Dry #50FF00

            new Color(0.333f, 1.0f, 1.0f), // Wet #55FFFF
            new Color(0.078f, 0.28f, 1.0f), // Wetter #1446FF
            new Color(0.0f, 0.0f, 0.39f), // Wettest #000064
        };


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
            for (int x = 0; x < WorldMap.Width; x++)
            {
                for (int y = 0; y < WorldMap.Height; y++)
                {
                    var tilePos =new Vector3Int(x, y);
                    var biome = biomeHelper.GetBiome(Features.TerrainData.BiomeMap[x,y]);

                    if (biome.isLand)
                    {
                        landMap.SetTile(tilePos, biome.WorldBase);
                        if (biome.WorldAccent != null)
                            terrainFeatureMap.SetTile(tilePos, biome.WorldAccent);
                    }
                    else oceanMap.SetTile(tilePos, biome.WorldBase);

                    if (Features.TerrainData.Coasts[x, y])
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

            foreach (var river in Features.TerrainData.Rivers)
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

            if (Features.TerrainData.Roads == null) return;

            foreach (var road in Features.TerrainData.Roads)
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

            foreach(var bridge in Features.TerrainData.Bridges)
            {
                var tilePos = new Vector3Int(bridge.x, bridge.y);
                if (bridge.isVertical) roadMap.SetTile(tilePos, bridgeVertical);
                else roadMap.SetTile(tilePos, bridgeHorizontal);
            }
        }

        private void DisplaySettlements()
        {
            settlementMap.ClearAllTiles();

            if (SettlementData.Settlements == null) return;

            foreach (var settlement in SettlementData.Settlements)
            {
                var tilePos = new Vector3Int(settlement.x, settlement.y);
                settlementMap.SetTile(tilePos, settlementTiles[(int)settlement.Category]);
            }
        }

        public void ResetInfoMap()
        {
            for (int x = 0; x < WorldMap.Width; x++)
            {
                for (int y = 0; y < WorldMap.Height; y++)
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

            for (int x = 0; x < WorldMap.Width; x++)
            {
                for (int y = 0; y < WorldMap.Height; y++)
                {
                    var tilePos = new Vector3Int(x, y);
                    var node = WorldMap.GetNode(x, y);
                    infoMap.SetColor(tilePos, _heatColors[node.TemperatureIndex]);

                }
            }
        }

        public void DisplayMoistureMap()
        {
            ResetInfoMap();

            for (int x = 0; x < WorldMap.Width; x++)
            {
                for (int y = 0; y < WorldMap.Height; y++)
                {
                    var tilePos = new Vector3Int(x, y);
                    var node = WorldMap.GetNode(x, y);
                    infoMap.SetColor(tilePos, _wetColors[node.PrecipitationZoneID]);
                }
            }
        }

        public void DisplayWindMap()
        {
            ResetInfoMap();

            for (int x = 0; x < WorldMap.Width; x++)
            {
                for (int y = 0; y < WorldMap.Height; y++)
                {
                    var tilePos = new Vector3Int(x, y);
                    var node = WorldMap.GetNode(x, y);
                    infoMap.SetTile(tilePos, directionTiles.GetTile(node.windDirection));
                }
            }
        }
        #endregion

        public void DisplayDangerMap()
        {
            ResetInfoMap();

            for (int x = 0; x < WorldMap.Width; x++)
            {
                for (int y = 0; y < WorldMap.Height; y++)
                {
                    var tilePos = new Vector3Int(x, y);
                    var node = WorldMap.GetNode(x, y);
                    infoMap.SetColor(tilePos, dangerColors[node.DangerTier]);
                }
            }
        }

        #region - Highlight Features -
        public void HighlightTectonicPlates()
        {
            var list = new List<WorldTile>();

            for (int x = 0; x < WorldMap.Width; x++)
            {
                for (int y = 0; y < WorldMap.Height; y++)
                {
                    var node = WorldMap.GetNode(x, y);
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

            foreach(var point in Features.TerrainData.PlateBorders)
            {
                var tilePos = new Vector3Int(point.x, point.y);
                infoMap.SetColor(tilePos, Color.red);
            }
        }

        public void HighlightBiome()
        {
            ResetInfoMap();

            foreach (var biome in Features.TerrainData.BiomeGroups)
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

            foreach (var mountain in Features.TerrainData.Mountains)
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

            foreach (var landMass in Features.TerrainData.LandMasses)
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

            foreach (var lake in Features.TerrainData.Lakes)
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

            foreach (var settlement in SettlementData.Settlements)
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

            for (int x = 0; x < WorldMap.Width; x++)
            {
                for (int y = 0; y < WorldMap.Height; y++)
                {
                    var tilePos = new Vector3Int(x, y);
                    if (Features.TerrainData.Coasts[x, y])
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
            for (int x = 0; x < WorldMap.Width; x++)
            {
                for (int y = 0; y < WorldMap.Height; y++)
                {
                    var tilePos = new Vector3Int(x, y);

                    switch (resourceToHighlight)
                    {
                        case Ores.Coal:
                            if (Features.TerrainData.CoalMap[x,y] > 0)
                            {
                                infoMap.SetColor(tilePos, Color.red);
                                totalCount++;
                            }
                            break;
                        case Ores.Copper:
                            if (Features.TerrainData.CopperMap[x, y] > 0)
                            {
                                infoMap.SetColor(tilePos, Color.red);
                                totalCount++;
                            }
                            break;
                        case Ores.Iron:
                            if (Features.TerrainData.IronMap[x, y] > 0)
                            {
                                infoMap.SetColor(tilePos, Color.red);
                                totalCount++;
                            }
                            break;
                        case Ores.Silver:
                            if (Features.TerrainData.SilverMap[x, y] > 0)
                            {
                                infoMap.SetColor(tilePos, Color.red);
                                totalCount++;
                            }
                            break;
                        case Ores.Gold:
                            if (Features.TerrainData.GoldMap[x, y] > 0)
                            {
                                infoMap.SetColor(tilePos, Color.red);
                                totalCount++;
                            }
                            break;
                        case Ores.Gemstones:
                            if (Features.TerrainData.GemstoneMap[x, y] > 0)
                            {
                                infoMap.SetColor(tilePos, Color.red);
                                totalCount++;
                            }
                            break;
                        case Ores.Mithril:
                            if (Features.TerrainData.MithrilMap[x, y] > 0)
                            {
                                infoMap.SetColor(tilePos, Color.red);
                                totalCount++;
                            }
                            break;
                        case Ores.Adamantine:
                            if (Features.TerrainData.AdmanatineMap[x, y] > 0)
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

            var settlement = SettlementData.FindSettlement(node.x, node.y);
            if (settlement != null)
            {
                HighlightSettlement(settlement);
                return;
            }

            var pos = WorldMap.GetWorldPosition(node.x, node.y);
            Vector3Int tilePos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
            infoMap.SetColor(tilePos, Color.red);
        }

        private void HighlightSettlement(Settlement settlement)
        {
            ResetInfoMap();

            for (int i = 0; i < settlement.Territory.Count; i++)
            {
                var pos = WorldMap.GetWorldPosition(settlement.Territory[i].x, settlement.Territory[i].y);
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