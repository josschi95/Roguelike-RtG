using UnityEngine;
using UnityEngine.Tilemaps;
using JS.WorldGeneration;

public class MapDisplay : MonoBehaviour
{
    [SerializeField] private WorldMapData worldMap;
    [SerializeField] private TerrainData terrainData;

    [Space]

    [SerializeField] private Tilemap oceanTileMap;
    [SerializeField] private Tilemap landTileMap;
    [SerializeField] private Tilemap biomeMap;
    [SerializeField] private Tilemap infoMap;
    [SerializeField] private TileBase highlightTile;

    [Space]
    
    [SerializeField] private TileBase[] windDirectionTiles;

    [Space]

    [SerializeField] private RuleTile landTile;
    [SerializeField] private RuleTile waterTile;

    public BiomeTypes highlightedBiome { get; set; }

    public void DisplayHeightMap()
    {
        oceanTileMap.ClearAllTiles();
        landTileMap.ClearAllTiles();
        //var worldMap = WorldMap.instance;

        for (int x = 0; x < terrainData.mapSize; x++)
        {
            for (int y = 0; y < terrainData.mapSize; y++)
            {
                var node = worldMap.GetNode(x, y);
                var tilePos = terrainData.mapOrigin + new Vector3Int(x, y);
                
                oceanTileMap.SetTile(tilePos, waterTile);
                if (node.isNotWater) landTileMap.SetTile(tilePos, landTile);
            }
        }
    }

    public void DisplayBiomeMap()
    {
        biomeMap.ClearAllTiles();
        //var worldMap = WorldMap.instance;

        for (int x = 0; x < terrainData.mapSize; x++)
        {
            for (int y = 0; y < terrainData.mapSize; y++)
            {
                var tilePos = terrainData.mapOrigin + new Vector3Int(x, y);
                var node = worldMap.GetNode(x, y);

                if (node.Settlement != null && node.Settlement.Node == node)
                {
                    biomeMap.SetTile(tilePos, node.Settlement.settlementType.settlementTile);
                    continue;
                }

                if (node.biome == null) continue;

                biomeMap.SetTile(tilePos, node.biome.RuleTile);
            }
        }
    }

    public void ClearDisplay()
    {
        infoMap.ClearAllTiles();
    }

    public void DisplayHeatMap()
    {
        infoMap.ClearAllTiles();
        //var worldMap = WorldMap.instance;

        for (int x = 0; x < terrainData.mapSize; x++)
        {
            for (int y = 0; y < terrainData.mapSize; y++)
            {
                var tilePos = terrainData.mapOrigin + new Vector3Int(x, y);
                var node = worldMap.GetNode(x, y);

                infoMap.SetTile(tilePos, node.temperatureZone.Tile);

            }
        }
    }

    public void DisplayMoistureMap()
    {
        infoMap.ClearAllTiles();
        //var worldMap = WorldMap.instance;

        for (int x = 0; x < terrainData.mapSize; x++)
        {
            for (int y = 0; y < terrainData.mapSize; y++)
            {
                var tilePos = terrainData.mapOrigin + new Vector3Int(x, y);
                var node = worldMap.GetNode(x, y);

                infoMap.SetTile(tilePos, node.precipitationZone.Tile);
            }
        }
    }

    public void DisplayWindMap()
    {
        //var worldMap = WorldMap.instance;
        infoMap.ClearAllTiles();

        for (int x = 0; x < terrainData.mapSize; x++)
        {
            for (int y = 0; y < terrainData.mapSize; y++)
            {
                var tilePos = terrainData.mapOrigin + new Vector3Int(x, y);
                var node = worldMap.GetNode(x, y);

                infoMap.SetTile(tilePos, windDirectionTiles[(int)node.windDirection]);
            }
        }
    }

    public void HighlightTectonicPlates()
    {
        infoMap.ClearAllTiles();
        //var worldMap = WorldMap.instance;

        for (int x = 0; x < terrainData.mapSize; x++)
        {
            for (int y = 0; y < terrainData.mapSize; y++)
            {
                var tilePos = terrainData.mapOrigin + new Vector3Int(x, y);
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

        for (int x = 0; x < terrainData.mapSize; x++)
        {
            for (int y = 0; y < terrainData.mapSize; y++)
            {
                var tilePos = terrainData.mapOrigin + new Vector3Int(x, y);
                var node = worldMap.GetNode(x, y);
                if (node.biome != null && node.biome.BiomeType == highlightedBiome)
                {
                    infoMap.SetTile(tilePos, highlightTile);
                }
            }
        }
    }

    public void HighlightMountains()
    {
        infoMap.ClearAllTiles();
        //var worldMap = WorldMap.instance;

        for (int x = 0; x < terrainData.mapSize; x++)
        {
            for (int y = 0; y < terrainData.mapSize; y++)
            {
                var tilePos = terrainData.mapOrigin + new Vector3Int(x, y);
                var node = worldMap.GetNode(x, y);
                if (node.Mountain != null)
                {
                    infoMap.SetTile(tilePos, highlightTile);
                }
            }
        }
    }

    public void HighlightIslands()
    {
        infoMap.ClearAllTiles();
        //var worldMap = WorldMap.instance;

        for (int x = 0; x < terrainData.mapSize; x++)
        {
            for (int y = 0; y < terrainData.mapSize; y++)
            {
                var tilePos = terrainData.mapOrigin + new Vector3Int(x, y);
                var node = worldMap.GetNode(x, y);
                if (node.Island != null)
                {
                    infoMap.SetTile(tilePos, highlightTile);
                }
            }
        }
    }

    public void HighlightLakes()
    {
        infoMap.ClearAllTiles();
        //var worldMap = WorldMap.instance;

        for (int x = 0; x < terrainData.mapSize; x++)
        {
            for (int y = 0; y < terrainData.mapSize; y++)
            {
                var tilePos = terrainData.mapOrigin + new Vector3Int(x, y);
                var node = worldMap.GetNode(x, y);
                if (node.Lake != null)
                {
                    infoMap.SetTile(tilePos, highlightTile);
                }
            }
        }
    }

    public void HighlightSettlements()
    {
        infoMap.ClearAllTiles();
        //var worldMap = WorldMap.instance;

        for (int x = 0; x < terrainData.mapSize; x++)
        {
            for (int y = 0; y < terrainData.mapSize; y++)
            {
                var tilePos = terrainData.mapOrigin + new Vector3Int(x, y);
                var node = worldMap.GetNode(x, y);
                if (node.Settlement != null)
                {
                    infoMap.SetTile(tilePos, highlightTile);
                }
            }
        }
    }

    public void HighlightNode(TerrainNode node)
    {
        infoMap.ClearAllTiles();
        //var worldMap = WorldMap.instance;

        var pos = worldMap.GetPosition(node);
        Vector3Int newPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
        infoMap.SetTile(newPos, highlightTile);

        /*for (int i = 0; i < node.neighbors.Length; i++)
        {
            var neighbor = map.GetPosition(node.neighbors[i]);
            Vector3Int nPos = new Vector3Int(Mathf.FloorToInt(neighbor.x), Mathf.FloorToInt(neighbor.y));
            infoMap.SetTile(nPos, highlightTile);
        }*/
    }

    /*
    private TerrainType GetTile(float height)
    {
        for (int i = terrainTypes.Length - 1; i >= 0; i--)
        {
            if (height >= terrainTypes[i].Height)
            {
                return terrainTypes[i];
            }
        }
        return null;
    }

    private TerrainType GetUnderlyingTile(float height, int layer)
    {
        for (int i = terrainTypes.Length - 1; i >= 0; i--)
        {
            if ((int)terrainTypes[i].layer != layer) continue;

            if (height >= terrainTypes[i].Height)
            {
                return terrainTypes[i];
            }
        }
        return null;
    }
    */
}