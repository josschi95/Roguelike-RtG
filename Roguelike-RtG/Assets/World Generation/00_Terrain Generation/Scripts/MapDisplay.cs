using UnityEngine;
using UnityEngine.Tilemaps;
using JS.WorldMap;

namespace JS.WorldMap
{
    public class MapDisplay : MonoBehaviour
    {
        [SerializeField] private WorldMapData worldMap;
        [SerializeField] private RiverTileHelper tileHelper;
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

        [SerializeField] private RuleTile waterTile;

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

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var node = worldMap.GetNode(x, y);
                    var tilePos = worldMap.TerrainData.Origin + new Vector3Int(x, y);

                    oceanMap.SetTile(tilePos, waterTile);
                    if (node.isNotWater)
                    {
                        landMap.SetTile(tilePos, node.PrimaryBiome.RuleTile);
                        //if (node.isCoast) oceanMap.SetTile(tilePos, waterTile);
                    }
                }
            }
        }

        private void DisplayRivers()
        {
            riverMap.ClearAllTiles();

            foreach (var river in worldMap.TerrainData.Rivers)
            {
                for (int i = 0; i < river.Nodes.Count; i++)
                {
                    var tilePos = worldMap.TerrainData.Origin + new Vector3Int(river.Nodes[i].x, river.Nodes[i].y);
                    if (riverMap.GetTile(tilePos) != null)
                    {
                        Debug.LogWarning("River intersection found. Need to account for T's");
                    }
                    var tile = tileHelper.GetRiverTile(river.Flow[i]);
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

            foreach (var settlement in worldMap.SettlementData.Settlements)
            {
                var tilePos = worldMap.TerrainData.Origin + new Vector3Int(settlement.Node.x, settlement.Node.y);
                settlementMap.SetTile(tilePos, settlement.type.settlementTile);
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

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos = worldMap.TerrainData.Origin + new Vector3Int(x, y);
                    var node = worldMap.GetNode(x, y);

                    infoMap.SetTile(tilePos, node.temperatureZone.Tile);

                }
            }
        }

        public void DisplayMoistureMap()
        {
            infoMap.ClearAllTiles();

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos = worldMap.TerrainData.Origin + new Vector3Int(x, y);
                    var node = worldMap.GetNode(x, y);

                    infoMap.SetTile(tilePos, node.precipitationZone.Tile);
                }
            }
        }

        public void DisplayWindMap()
        {
            infoMap.ClearAllTiles();

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos = worldMap.TerrainData.Origin + new Vector3Int(x, y);
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
            //var worldMap = WorldMap.instance;

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos = worldMap.TerrainData.Origin + new Vector3Int(x, y);
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
            //var worldMap = WorldMap.instance;

            foreach (var biome in worldMap.TerrainData.BiomeGroups)
            {
                if (biome.biome != biomeToHighlight) continue;
                for (int i = 0; i < biome.Nodes.Count; i++)
                {
                    var tilePos = worldMap.TerrainData.Origin + new Vector3Int(biome.Nodes[i].x, biome.Nodes[i].y);
                    infoMap.SetTile(tilePos, highlightTile);
                }
            }
        }

        public void HighlightMountains()
        {
            infoMap.ClearAllTiles();

            foreach (var mountain in worldMap.TerrainData.Mountains)
            {
                for (int i = 0; i < mountain.Nodes.Count; i++)
                {
                    var tilePos = worldMap.TerrainData.Origin + new Vector3Int(mountain.Nodes[i].x, mountain.Nodes[i].y);
                    infoMap.SetTile(tilePos, highlightTile);
                }
            }
        }

        public void HighlightIslands()
        {
            infoMap.ClearAllTiles();

            foreach (var island in worldMap.TerrainData.Islands)
            {
                for (int i = 0; i < island.Nodes.Count; i++)
                {
                    var tilePos = worldMap.TerrainData.Origin + new Vector3Int(island.Nodes[i].x, island.Nodes[i].y);
                    infoMap.SetTile(tilePos, highlightTile);
                }
            }
        }

        public void HighlightLakes()
        {
            infoMap.ClearAllTiles();

            foreach (var lake in worldMap.TerrainData.Lakes)
            {
                for (int i = 0; i < lake.Nodes.Count; i++)
                {
                    var tilePos = worldMap.TerrainData.Origin + new Vector3Int(lake.Nodes[i].x, lake.Nodes[i].y);
                    infoMap.SetTile(tilePos, highlightTile);
                }
            }
        }

        public void HighlightSettlements()
        {
            infoMap.ClearAllTiles();

            foreach (var settlement in worldMap.SettlementData.Settlements)
            {
                for (int i = 0; i < settlement.territory.Count; i++)
                {
                    var tilePos = worldMap.TerrainData.Origin + new Vector3Int(settlement.territory[i].x, settlement.territory[i].y);
                    infoMap.SetTile(tilePos, highlightTile);
                }
            }
        }

        public void HighlightCoasts()
        {
            infoMap.ClearAllTiles();

            for (int x = 0; x < worldMap.Width; x++)
            {
                for (int y = 0; y < worldMap.Height; y++)
                {
                    var tilePos = worldMap.TerrainData.Origin + new Vector3Int(x, y);
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

            if (node.Settlement != null)
            {
                HighlightSettlement(node.Settlement);
                return;
            }

            var pos = worldMap.GetPosition(node);
            Vector3Int newPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
            infoMap.SetTile(newPos, highlightTile);

            if (node.rivers.Count > 0)
            {
                Debug.Log("River Info:");
                for (int i = 0; i < node.rivers[0].Nodes.Count; i++)
                {
                    Debug.Log(node.rivers[0].Nodes[i].x + "," + node.rivers[0].Nodes[i].y + " : " + node.rivers[0].Flow[i].ToString());
                }
            }

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

            for (int i = 0; i < settlement.territory.Count; i++)
            {
                var pos = worldMap.GetPosition(settlement.territory[i]);
                Vector3Int newPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
                infoMap.SetTile(newPos, highlightTile);
            }
        }
        #endregion
    }
}