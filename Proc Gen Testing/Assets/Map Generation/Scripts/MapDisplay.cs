using UnityEngine;
using UnityEngine.Tilemaps;

public class MapDisplay : MonoBehaviour
{
    [SerializeField] private MapGenerator mapGenerator;

    [Space]

    [SerializeField] private Tilemap[] tilemaps;
    [SerializeField] private Tilemap biomeMap;
    [SerializeField] private Tilemap infoMap;
    [SerializeField] private TileBase highlightTile;

    [Space]
    
    [SerializeField] private TerrainType[] terrainTypes;
    [SerializeField] private TemperatureZone[] temperatureZones;

    [SerializeField] private TileBase[] windDirectionTiles;

    public BiomeTypes highlightedBiome { get; set; }

    public void DisplayHeightMap()
    {
        infoMap.ClearAllTiles();
        for (int i = 0; i < tilemaps.Length; i++)
        {
            tilemaps[i].ClearAllTiles();
        }

        for (int x = 0; x < mapGenerator.mapSize; x++)
        {
            for (int y = 0; y < mapGenerator.mapSize; y++)
            {
                var node = WorldMap.instance.GetNode(x, y);

                var tilePos = mapGenerator.origin + new Vector3Int(x, y);
                var terrain = GetTile(node.altitude);
                if (terrain == null) continue;

                tilemaps[(int)terrain.layer].SetTile(tilePos, node.terrainType.RuleTile);

                for (int i = (int)terrain.layer - 1; i >= 0; i--)
                {
                    tilemaps[i].SetTile(tilePos, GetUnderlyingTile(node.altitude, i).RuleTile);
                }
            }
        }
    }

    public void DisplayBiomeMap()
    {
        biomeMap.ClearAllTiles();
        var map = WorldMap.instance;

        for (int x = 0; x < mapGenerator.mapSize; x++)
        {
            for (int y = 0; y < mapGenerator.mapSize; y++)
            {
                var tilePos = mapGenerator.origin + new Vector3Int(x, y);
                var node = map.GetNode(x, y);

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
        var map = WorldMap.instance;

        for (int x = 0; x < mapGenerator.mapSize; x++)
        {
            for (int y = 0; y < mapGenerator.mapSize; y++)
            {
                var tilePos = mapGenerator.origin + new Vector3Int(x, y);
                var node = map.GetNode(x, y);

                infoMap.SetTile(tilePos, node.temperatureZone.Tile);

            }
        }
    }

    public void DisplayMoistureMap()
    {
        infoMap.ClearAllTiles();
        var map = WorldMap.instance;

        for (int x = 0; x < mapGenerator.mapSize; x++)
        {
            for (int y = 0; y < mapGenerator.mapSize; y++)
            {
                var tilePos = mapGenerator.origin + new Vector3Int(x, y);
                var node = map.GetNode(x, y);

                infoMap.SetTile(tilePos, node.precipitationZone.Tile);
            }
        }
    }

    public void DisplayWindMap()
    {
        var map = WorldMap.instance;
        infoMap.ClearAllTiles();

        for (int x = 0; x < mapGenerator.mapSize; x++)
        {
            for (int y = 0; y < mapGenerator.mapSize; y++)
            {
                var tilePos = mapGenerator.origin + new Vector3Int(x, y);
                var node = map.GetNode(x, y);

                infoMap.SetTile(tilePos, windDirectionTiles[(int)node.windDirection]);
            }
        }
    }

    public void HighlightTectonicPlates()
    {
        infoMap.ClearAllTiles();
        var map = WorldMap.instance;

        for (int x = 0; x < mapGenerator.mapSize; x++)
        {
            for (int y = 0; y < mapGenerator.mapSize; y++)
            {
                var tilePos = mapGenerator.origin + new Vector3Int(x, y);
                var node = map.GetNode(x, y);
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
        var map = WorldMap.instance;

        for (int x = 0; x < mapGenerator.mapSize; x++)
        {
            for (int y = 0; y < mapGenerator.mapSize; y++)
            {
                var tilePos = mapGenerator.origin + new Vector3Int(x, y);
                var node = map.GetNode(x, y);
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
        var map = WorldMap.instance;

        for (int x = 0; x < mapGenerator.mapSize; x++)
        {
            for (int y = 0; y < mapGenerator.mapSize; y++)
            {
                var tilePos = mapGenerator.origin + new Vector3Int(x, y);
                var node = map.GetNode(x, y);
                if (node.Mountain != null)
                {
                    infoMap.SetTile(tilePos, highlightTile);
                }
            }
        }
    }

    public void HighlightNode(TerrainNode node)
    {
        infoMap.ClearAllTiles();
        var map = WorldMap.instance;

        var pos = map.GetPosition(node);
        Vector3Int newPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
        infoMap.SetTile(newPos, highlightTile);

        /*for (int i = 0; i < node.neighbors.Length; i++)
        {
            var neighbor = map.GetPosition(node.neighbors[i]);
            Vector3Int nPos = new Vector3Int(Mathf.FloorToInt(neighbor.x), Mathf.FloorToInt(neighbor.y));
            infoMap.SetTile(nPos, highlightTile);
        }*/
    }

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
}