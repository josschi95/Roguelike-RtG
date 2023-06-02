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

        public Biome biomeToHighlight { get; set; }
        public Resources resourceToHighlight { get; set; }

        //Called on scene load from GameEventListener
        public void DisplayWorldMap()
        {
            oceanMap.ClearAllTiles();
            landMap.ClearAllTiles();

            DisplayRivers();
            DisplayRoads();
            DisplaySettlements();
            DisplayBiomes();
        }

        private void DisplayBiomes()
        {
            var list = new List<WorldTile>();

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos =new Vector3Int(x, y);
                    var node = worldMap.GetNode(x,y);

                    var biome = biomeHelper.GetBiome(node.BiomeID);

                    if (biome.isLand) landMap.SetTile(tilePos, biome.WorldTile);
                    else oceanMap.SetTile(tilePos, biome.WorldTile);

                    if (node.isTectonicPoint) list.Add(node);
                    if (node != null && node.isCoast)
                    {
                        oceanMap.SetTile(tilePos, coastalWaterTile);
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

                    RuleTile tile = null;
                    var existingTile = riverMap.GetTile(tilePos) as RuleTile;
                    if (existingTile != null)
                    {
                        //Debug.Log("At " + tilePos);
                        tile = riverTiles.GetIntersectionTile(existingTile, river.Nodes[i].Flow);
                    }
                    else tile = riverTiles.GetRiverTile(river.Nodes[i].Flow);
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

                    RuleTile tile = null;
                    var existingTile = roadMap.GetTile(tilePos) as RuleTile;
                    if (existingTile != null)
                    {
                        //Debug.Log("At " + tilePos);
                        tile = roadTiles.GetIntersectionTile(existingTile, road.Nodes[i].Flow);
                    }
                    else tile = roadTiles.GetRiverTile(road.Nodes[i].Flow);
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
            //var origin = new Vector3Int(worldMap.TerrainData.OriginX, worldMap.TerrainData.OriginY);

            foreach (var settlement in worldMap.SettlementData.Settlements)
            {
                var tilePos = new Vector3Int(settlement.X, settlement.Y);
                //var tilePos = origin + new Vector3Int(settlement.X, settlement.Y);
                settlementMap.SetTile(tilePos, worldMap.SettlementData.Types[settlement.TypeID].settlementTile);
            }
        }

        public void ClearInfoMap()
        {
            infoMap.ClearAllTiles();
        }

        #region - Climate Values -
        public void DisplayHeatMap()
        {
            infoMap.ClearAllTiles();
            //var origin = new Vector3Int(worldMap.TerrainData.OriginX, worldMap.TerrainData.OriginY);

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos = new Vector3Int(x, y);
                    //var tilePos = origin + new Vector3Int(x, y);
                    var node = worldMap.GetNode(x, y);
                    var zone = worldGenParams.TemperatureZones[node.TempZoneID];
                    infoMap.SetTile(tilePos, zone.Tile);

                }
            }
        }

        public void DisplayMoistureMap()
        {
            infoMap.ClearAllTiles();
            //var origin = new Vector3Int(worldMap.TerrainData.OriginX, worldMap.TerrainData.OriginY);

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos = new Vector3Int(x, y);
                    //var tilePos = origin + new Vector3Int(x, y);
                    var node = worldMap.GetNode(x, y);
                    var zone = worldGenParams.PrecipitationZones[node.PrecipitationZoneID];
                    infoMap.SetTile(tilePos, zone.Tile);
                }
            }
        }

        public void DisplayWindMap()
        {
            infoMap.ClearAllTiles();
            //var origin = new Vector3Int(worldMap.TerrainData.OriginX, worldMap.TerrainData.OriginY);

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos = new Vector3Int(x, y);
                    //var tilePos = origin + new Vector3Int(x, y);
                    var node = worldMap.GetNode(x, y);
                    infoMap.SetTile(tilePos, directionTiles.GetTile(node.windDirection));
                }
            }
        }
        #endregion

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
        }

        public void HighlightBiome()
        {
            infoMap.ClearAllTiles();
            //var origin = new Vector3Int(worldMap.TerrainData.OriginX, worldMap.TerrainData.OriginY);

            foreach (var biome in worldMap.TerrainData.BiomeGroups)
            {
                if (biome.BiomeID != biomeToHighlight.ID) continue;
                for (int i = 0; i < biome.Nodes.Count; i++)
                {
                    var tilePos = new Vector3Int(biome.Nodes[i].x, biome.Nodes[i].y);
                    //var tilePos = origin + new Vector3Int(biome.Nodes[i].x, biome.Nodes[i].y);
                    infoMap.SetTile(tilePos, highlightTile);
                }
            }
        }

        public void HighlightMountains()
        {
            infoMap.ClearAllTiles();
            //var origin = new Vector3Int(worldMap.TerrainData.OriginX, worldMap.TerrainData.OriginY);

            foreach (var mountain in worldMap.TerrainData.Mountains)
            {
                for (int i = 0; i < mountain.Nodes.Count; i++)
                {
                    var tilePos = new Vector3Int(mountain.Nodes[i].x, mountain.Nodes[i].y);
                    //var tilePos = origin + new Vector3Int(mountain.Nodes[i].x, mountain.Nodes[i].y);
                    infoMap.SetTile(tilePos, highlightTile);
                }
            }
        }

        public void HighlightIslands()
        {
            infoMap.ClearAllTiles();
            //var origin = new Vector3Int(worldMap.TerrainData.OriginX, worldMap.TerrainData.OriginY);

            foreach (var island in worldMap.TerrainData.Islands)
            {
                for (int i = 0; i < island.Nodes.Count; i++)
                {
                    var tilePos = new Vector3Int(island.Nodes[i].x, island.Nodes[i].y);
                    //var tilePos = origin + new Vector3Int(island.Nodes[i].x, island.Nodes[i].y);
                    infoMap.SetTile(tilePos, highlightTile);
                }
            }
        }

        public void HighlightLakes()
        {
            infoMap.ClearAllTiles();
            //var origin = new Vector3Int(worldMap.TerrainData.OriginX, worldMap.TerrainData.OriginY);

            foreach (var lake in worldMap.TerrainData.Lakes)
            {
                for (int i = 0; i < lake.Nodes.Count; i++)
                {
                    var tilePos = new Vector3Int(lake.Nodes[i].x, lake.Nodes[i].y);
                    //var tilePos = origin + new Vector3Int(lake.Nodes[i].x, lake.Nodes[i].y);
                    infoMap.SetTile(tilePos, highlightTile);
                }
            }
        }

        public void HighlightSettlements()
        {
            infoMap.ClearAllTiles();
            //var origin = new Vector3Int(worldMap.TerrainData.OriginX, worldMap.TerrainData.OriginY);

            foreach (var settlement in worldMap.SettlementData.Settlements)
            {
                for (int i = 0; i < settlement.Territory.Count; i++)
                {
                    var tilePos = new Vector3Int(settlement.Territory[i].x, settlement.Territory[i].y);
                    //var tilePos = origin + new Vector3Int(settlement.Territory[i].x, settlement.Territory[i].y);
                    infoMap.SetTile(tilePos, highlightTile);
                }
            }
        }

        public void HighlightCoasts()
        {
            infoMap.ClearAllTiles();
            //var origin = new Vector3Int(worldMap.TerrainData.OriginX, worldMap.TerrainData.OriginY);

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos = new Vector3Int(x, y);
                    //var tilePos = origin + new Vector3Int(x, y);
                    var node = worldMap.GetNode(x, y);
                    if (node.isCoast)
                    {
                        infoMap.SetTile(tilePos, highlightTile);
                    }
                }
            }
        }

        public void HighlightResources()
        {
            infoMap.ClearAllTiles();
            int totalCount = 0;
            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos = new Vector3Int(x, y);

                    switch (resourceToHighlight)
                    {
                        case Resources.Coal:
                            if (worldMap.TerrainData.CoalMap[x,y] > 0)
                            {
                                infoMap.SetTile(tilePos, highlightTile);
                                totalCount++;
                            }
                            break;
                        case Resources.Copper:
                            if (worldMap.TerrainData.CopperMap[x, y] > 0)
                            {
                                infoMap.SetTile(tilePos, highlightTile);
                                totalCount++;
                            }
                            break;
                        case Resources.Iron:
                            if (worldMap.TerrainData.IronMap[x, y] > 0)
                            {
                                infoMap.SetTile(tilePos, highlightTile);
                                totalCount++;
                            }
                            break;
                        case Resources.Silver:
                            if (worldMap.TerrainData.SilverMap[x, y] > 0)
                            {
                                infoMap.SetTile(tilePos, highlightTile);
                                totalCount++;
                            }
                            break;
                        case Resources.Gold:
                            if (worldMap.TerrainData.GoldMap[x, y] > 0)
                            {
                                infoMap.SetTile(tilePos, highlightTile);
                                totalCount++;
                            }
                            break;
                        case Resources.Gemstones:
                            if (worldMap.TerrainData.GemstoneMap[x, y] > 0)
                            {
                                infoMap.SetTile(tilePos, highlightTile);
                                totalCount++;
                            }
                            break;
                        case Resources.Mithril:
                            if (worldMap.TerrainData.MithrilMap[x, y] > 0)
                            {
                                infoMap.SetTile(tilePos, highlightTile);
                                totalCount++;
                            }
                            break;
                        case Resources.Adamantine:
                            if (worldMap.TerrainData.AdmanatineMap[x, y] > 0)
                            {
                                infoMap.SetTile(tilePos, highlightTile);
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
            infoMap.ClearAllTiles();
            //var worldMap = WorldMap.instance;

            var settlement = worldMap.SettlementData.FindSettlement(node.x, node.y);
            if (settlement != null)
            {
                HighlightSettlement(settlement);
                return;
            }

            var pos = worldMap.GetWorldPosition(node);
            Vector3Int newPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
            infoMap.SetTile(newPos, highlightTile);
        }

        private void HighlightSettlement(Settlement settlement)
        {
            infoMap.ClearAllTiles();

            for (int i = 0; i < settlement.Territory.Count; i++)
            {
                var pos = worldMap.GetWorldPosition(settlement.Territory[i].x, settlement.Territory[i].y);
                Vector3Int newPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
                infoMap.SetTile(newPos, highlightTile);
            }
        }
        #endregion
    }
}

public enum Resources
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