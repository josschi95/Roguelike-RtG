using UnityEngine;
using UnityEngine.Tilemaps;
using JS.WorldMap;

namespace JS.WorldMap
{
    public class MapDisplay : MonoBehaviour
    {
        [SerializeField] private WorldData worldMap;
        [SerializeField] private SettlementData settlementData;
        [SerializeField] private RiverTileHelper tileHelper;
        [SerializeField] private WorldGenerationParameters worldGenerationParameters;
        [Space]

        [SerializeField] private Tilemap oceanMap;
        [SerializeField] private Tilemap landMap;
        [SerializeField] private Tilemap terrainFeatureMap;
        [SerializeField] private Tilemap riverMap;
        [SerializeField] private Tilemap roadMap;
        [SerializeField] private Tilemap settlementMap;
        [SerializeField] private Tilemap infoMap;
        [SerializeField] private TileBase highlightTile;

        [Space]

        [SerializeField] private TileBase[] windDirectionTiles;

        [Space]

        public Biome[] biomes;
        public Biome biomeToHighlight { get; set; }

        //Ok so this is how I'm going to have to do this
        //ocean map goes at the bottom
        //flat biome map goes on top of that
        //feature map goes on top of that - 


        //you know what, features can just go over biomes
        //so have the normal biome map
        //mountains, trees, etc. go on top of that or are included
        //then rivers go over top of that
        //then roads go over top of that
        //then settlements go over top of that

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
            var origin = new Vector3Int(worldMap.TerrainData.OriginX, worldMap.TerrainData.OriginY);
            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos = origin + new Vector3Int(x, y);
                    var biome = worldMap.TerrainData.GetBiome(x, y);

                    if (biome.isLand) landMap.SetTile(tilePos, biome.RuleTile);
                    else oceanMap.SetTile(tilePos, biome.RuleTile);
                }
            }
        }

        private void DisplayRivers()
        {
            riverMap.ClearAllTiles();
            var origin = new Vector3Int(worldMap.TerrainData.OriginX, worldMap.TerrainData.OriginY);
            foreach (var river in worldMap.TerrainData.Rivers)
            {
                for (int i = 0; i < river.Coordinates.Length; i++)
                {
                    var tilePos = origin + new Vector3Int(river.Coordinates[i].x, river.Coordinates[i].y);
                    RuleTile tile = null;
                    var existingTile = riverMap.GetTile(tilePos) as RuleTile;
                    if (existingTile != null)
                    {
                        //Debug.Log("At " + tilePos);
                        tile = tileHelper.GetIntersectionTile(existingTile, river.Flow[i]);
                    }
                    else tile = tileHelper.GetRiverTile(river.Flow[i]);
                    riverMap.SetTile(tilePos, tile);
                }
            }
        }

        private void DisplayRoads()
        {
            /*roadMap.ClearAllTiles();

            foreach (var road in worldMap.SettlementData.Roads)
            {
                for (int i = 0; i < road.Nodes.Count; i++)
                {
                    var tilePos = worldMap.TerrainData.mapOrigin + new Vector3Int(road.Nodes[i].x, road.Nodes[i].y);
                    riverMap.SetTile(tilePos, worldMap.TerrainData.RiverTile);
                }
            }*/
        }

        private void DisplaySettlements()
        {
            settlementMap.ClearAllTiles();
            var origin = new Vector3Int(worldMap.TerrainData.OriginX, worldMap.TerrainData.OriginY);

            foreach (var settlement in worldMap.SettlementData.Settlements)
            {
                var tilePos = origin + new Vector3Int(settlement.X, settlement.Y);
                settlementMap.SetTile(tilePos, settlementData.Types[settlement.TypeID].settlementTile);
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
            var origin = new Vector3Int(worldMap.TerrainData.OriginX, worldMap.TerrainData.OriginY);

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos = origin + new Vector3Int(x, y);
                    var node = worldMap.GetNode(x, y);
                    var zone = worldGenerationParameters.TemperatureZones[node.TempZoneID];
                    infoMap.SetTile(tilePos, zone.Tile);

                }
            }
        }

        public void DisplayMoistureMap()
        {
            infoMap.ClearAllTiles();
            var origin = new Vector3Int(worldMap.TerrainData.OriginX, worldMap.TerrainData.OriginY);

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos = origin + new Vector3Int(x, y);
                    var node = worldMap.GetNode(x, y);
                    var zone = worldGenerationParameters.PrecipitationZones[node.PrecipitationZoneID];
                    infoMap.SetTile(tilePos, zone.Tile);
                }
            }
        }

        public void DisplayWindMap()
        {
            infoMap.ClearAllTiles();
            var origin = new Vector3Int(worldMap.TerrainData.OriginX, worldMap.TerrainData.OriginY);

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos = origin + new Vector3Int(x, y);
                    var node = worldMap.GetNode(x, y);

                    infoMap.SetTile(tilePos, windDirectionTiles[(int)node.windDirection]);
                }
            }
        }
        #endregion

        #region - Highlight Features -
        public void HighlightTectonicPlates()
        {
            infoMap.ClearAllTiles();
            var origin = new Vector3Int(worldMap.TerrainData.OriginX, worldMap.TerrainData.OriginY);

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos = origin + new Vector3Int(x, y);
                    var node = worldMap.GetNode(x, y);
                    if (node.isTectonicPoint)
                    {
                        infoMap.SetTile(tilePos, highlightTile);
                    }
                }
            }
        }

        public void HighlightBiome()
        {
            infoMap.ClearAllTiles();
            var origin = new Vector3Int(worldMap.TerrainData.OriginX, worldMap.TerrainData.OriginY);

            foreach (var biome in worldMap.TerrainData.BiomeGroups)
            {
                if (biome.BiomeID != biomeToHighlight.ID) continue;
                for (int i = 0; i < biome.Nodes.Count; i++)
                {
                    var tilePos = origin + new Vector3Int(biome.Nodes[i].x, biome.Nodes[i].y);
                    infoMap.SetTile(tilePos, highlightTile);
                }
            }
        }

        public void HighlightMountains()
        {
            infoMap.ClearAllTiles();
            var origin = new Vector3Int(worldMap.TerrainData.OriginX, worldMap.TerrainData.OriginY);

            foreach (var mountain in worldMap.TerrainData.Mountains)
            {
                for (int i = 0; i < mountain.Nodes.Count; i++)
                {
                    var tilePos = origin + new Vector3Int(mountain.Nodes[i].x, mountain.Nodes[i].y);
                    infoMap.SetTile(tilePos, highlightTile);
                }
            }
        }

        public void HighlightIslands()
        {
            infoMap.ClearAllTiles();
            var origin = new Vector3Int(worldMap.TerrainData.OriginX, worldMap.TerrainData.OriginY);

            foreach (var island in worldMap.TerrainData.Islands)
            {
                for (int i = 0; i < island.Nodes.Count; i++)
                {
                    var tilePos = origin + new Vector3Int(island.Nodes[i].x, island.Nodes[i].y);
                    infoMap.SetTile(tilePos, highlightTile);
                }
            }
        }

        public void HighlightLakes()
        {
            infoMap.ClearAllTiles();
            var origin = new Vector3Int(worldMap.TerrainData.OriginX, worldMap.TerrainData.OriginY);

            foreach (var lake in worldMap.TerrainData.Lakes)
            {
                for (int i = 0; i < lake.Nodes.Count; i++)
                {
                    var tilePos = origin + new Vector3Int(lake.Nodes[i].x, lake.Nodes[i].y);
                    infoMap.SetTile(tilePos, highlightTile);
                }
            }
        }

        public void HighlightSettlements()
        {
            infoMap.ClearAllTiles();
            var origin = new Vector3Int(worldMap.TerrainData.OriginX, worldMap.TerrainData.OriginY);

            foreach (var settlement in worldMap.SettlementData.Settlements)
            {
                for (int i = 0; i < settlement.Territory.Count; i++)
                {
                    var tilePos = origin + new Vector3Int(settlement.Territory[i].x, settlement.Territory[i].y);
                    infoMap.SetTile(tilePos, highlightTile);
                }
            }
        }

        public void HighlightCoasts()
        {
            infoMap.ClearAllTiles();
            var origin = new Vector3Int(worldMap.TerrainData.OriginX, worldMap.TerrainData.OriginY);

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos = origin + new Vector3Int(x, y);
                    var node = worldMap.GetNode(x, y);
                    if (node.isCoast)
                    {
                        infoMap.SetTile(tilePos, highlightTile);
                    }
                }
            }
        }

        public void HighlightNode(WorldTile node)
        {
            infoMap.ClearAllTiles();
            //var worldMap = WorldMap.instance;

            var settlement = settlementData.FindSettlement(node.x, node.y);
            if (settlement != null)
            {
                HighlightSettlement(settlement);
                return;
            }

            var pos = worldMap.GetWorldPosition(node);
            Vector3Int newPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
            infoMap.SetTile(newPos, highlightTile);

            /*if (node.rivers.Count > 0)
            {
                Debug.Log("River Info:");
                for (int i = 0; i < node.rivers[0].Coordinates.Length; i++)
                {
                    Debug.Log(node.rivers[0].Coordinates[i].x + "," + node.rivers[0].Coordinates[i].y + " : " + node.rivers[0].Flow[i].ToString());
                }
            }*/

            /*for (int i = 0; i < node.neighbors.Length; i++)
            {
                var neighbor = map.GetPosition(node.neighbors[i]);
                Vector3Int nPos = new Vector3Int(Mathf.FloorToInt(neighbor.x), Mathf.FloorToInt(neighbor.y));
                infoMap.SetTile(nPos, highlightTile);
            }*/
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