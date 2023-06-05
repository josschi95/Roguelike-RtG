using JS.WorldMap;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LocalMapDisplay : MonoBehaviour
{
    [SerializeField] private WorldData worldData;

    [SerializeField] private BiomeHelper biomeHelper;
    [SerializeField] private WorldGenerationParameters worldGenParams;

    [SerializeField] private SettlementData settlementData;
    [SerializeField] private PathTileHelper riverTiles;
    [SerializeField] private DirectionTiles directionTiles;

    [Space]

    [SerializeField] private Tilemap landMap;
    [SerializeField] private Tilemap featuresMap;
    [SerializeField] private Tilemap riverMap;
    [SerializeField] private Tilemap roadMap;
    [SerializeField] private Tilemap borderMap;

    [Space]

    public RuleTile lowTile;
    public RuleTile highTile;
    public RuleTile treeTile;
    public RuleTile waterTile;

    public void DisplayMap(LocalMapInfo info)
    {
        landMap.ClearAllTiles();
        featuresMap.ClearAllTiles();
        riverMap.ClearAllTiles();
        roadMap.ClearAllTiles();
        borderMap.ClearAllTiles();

        PlacePerlin(info.PerlinMap);
        PlaceFeatures(info.featurePoints);
        PlaceRiver();
        PlaceOutline();
    }

    private void PlacePerlin(float[,] perlinMap)
    {
        for (int x = 0; x < perlinMap.GetLength(0); x++)
        {
            for (int y = 0; y < perlinMap.GetLength(1); y++)
            {
                var tilePos = new Vector3Int(x, y);
                if (perlinMap[x, y] > 0.5f)
                {
                    landMap.SetTile(tilePos, highTile);
                }
                else
                {
                    landMap.SetTile(tilePos, lowTile);
                }
            }
        }
    }

    private void PlaceFeatures(List<Vector2> points)
    {
        for (int i = 0; i < points.Count; i++)
        {
            var pos = new Vector3Int((int)points[i].x, (int)points[i].y);
            featuresMap.SetTile(pos, treeTile);
        }
    }

    #region - Rivers -
    //Note that I can use this same stuff for a road with a simple bool
    //Also note that for rivers, it may be better to remove the land tiles there and place water tiles instead
    private void PlaceRiver()
    {
        var grid = GridManager.ActiveGrid.Grid;
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                var gridNode = grid.GetGridObject(x, y);
                if (gridNode.isWater)
                {
                    var tilePos = new Vector3Int(x,y);
                    landMap.SetTile(tilePos, null);
                    riverMap.SetTile(tilePos, waterTile);
                }
            }
        }
    }
    #endregion

    private void PlaceOutline()
    {
        var width = worldGenParams.LocalDimensions.x;
        var height = worldGenParams.LocalDimensions.y;

        for (int x = 0; x < width; x++)
        {
            var south = new Vector3Int(x, -1);
            borderMap.SetTile(south, directionTiles.GetTile(Compass.South));
            var north = new Vector3Int(x, height);
            borderMap.SetTile(north, directionTiles.GetTile(Compass.North));
        }
        for (int y = 0; y < height; y++)
        {
            var east = new Vector3Int(width, y);
            borderMap.SetTile(east, directionTiles.GetTile(Compass.East));
            var west = new Vector3Int(-1, y);
            borderMap.SetTile(west, directionTiles.GetTile(Compass.West));
        }
    }
}

public class LocalMapInfo
{
    public int x, y;
    public float[,] PerlinMap;
    public List<Vector2> featurePoints;
}