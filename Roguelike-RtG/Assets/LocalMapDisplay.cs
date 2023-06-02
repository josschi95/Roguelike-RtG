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

    public void DisplayMap(LocalMapInfo info)
    {
        PlacePerlin(info.PerlinMap);
        PlaceFeatures(info.featurePoints);
        PlaceRiver(info.x, info.y);
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
    private void PlaceRiver(int x, int y)
    {
        var river = worldData.TerrainData.FindRiverAt(x, y, out var index);
        if (river == null) return;

        RiverNode node = river.Nodes[index];

        var width = worldGenParams.LocalDimensions.x;
        var height = worldGenParams.LocalDimensions.y;
        int verticalOffset = Mathf.RoundToInt(width / 2);
        verticalOffset -= Mathf.RoundToInt(node.Size / 2);

        int horizontalOffset = Mathf.RoundToInt(height / 2);
        horizontalOffset -= Mathf.RoundToInt(node.Size / 2);

        if (node.Flow == Compass.North || node.Flow == Compass.South)
        {
            //Vertical river
            PlaceNorthRiver(height, verticalOffset, node.Size, node.Flow);
            PlaceSouthRiver(height, verticalOffset, node.Size, node.Flow);
        }
        else if (node.Flow == Compass.East || node.Flow == Compass.West)
        {
            //Horizontal river
            PlaceEastRiver(height, horizontalOffset, node.Size, node.Flow);
            PlaceWestRiver(height, horizontalOffset, node.Size, node.Flow);
        }
        else if (node.Flow == Compass.NorthEast)
        {
            PlaceNorthRiver(height, verticalOffset, node.Size, node.Flow);
            PlaceEastRiver(height, horizontalOffset, node.Size, node.Flow);
            RoundCorner(node.Flow, node.Size);
        }
        else if (node.Flow == Compass.SouthEast)
        {
            PlaceSouthRiver(height, verticalOffset, node.Size, node.Flow);
            PlaceEastRiver(height, horizontalOffset, node.Size, node.Flow);
            RoundCorner(node.Flow, node.Size);
        }
        else if (node.Flow == Compass.SouthWest)
        {
            PlaceSouthRiver(height, verticalOffset, node.Size, node.Flow);
            PlaceWestRiver(height, horizontalOffset, node.Size, node.Flow);
            RoundCorner(node.Flow, node.Size);
        }
        else if (node.Flow == Compass.NorthWest)
        {
            PlaceNorthRiver(height, verticalOffset, node.Size, node.Flow);
            PlaceWestRiver(height, horizontalOffset, node.Size, node.Flow);
            RoundCorner(node.Flow, node.Size);
        }
    }

    private void PlaceNorthRiver(int height, int offset, int riverWidth, Compass direction)
    {
        //Vertical river
        var halfHeight = height / 2;
        for (int i = halfHeight; i < height; i++)
        {
            for (int j = 0; j < riverWidth; j++)
            {
                var pos = new Vector3Int(offset + j, i);
                riverMap.SetTile(pos, riverTiles.GetRiverTile(direction));
            }

        }
    }

    private void PlaceSouthRiver(int height, int offset, int riverWidth, Compass direction)
    {
        var halfHeight = height / 2;
        for (int i = 0; i < halfHeight; i++)
        {
            for (int j = 0; j < riverWidth; j++)
            {
                var pos = new Vector3Int(offset + j, i);
                riverMap.SetTile(pos, riverTiles.GetRiverTile(direction));
            }
        }
    }

    private void PlaceEastRiver(int width, int offset, int riverWidth, Compass direction)
    {
        var halfWidth = width / 2;
        for (int i = halfWidth; i < width; i++)
        {
            for (int j = 0; j < riverWidth; j++)
            {
                var pos = new Vector3Int(i, offset + j);
                riverMap.SetTile(pos, riverTiles.GetRiverTile(direction));
            }
        }
    }

    private void PlaceWestRiver(int width, int offset, int riverWidth, Compass direction)
    {
        var halfWidth = width / 2;
        for (int i = 0; i < halfWidth; i++)
        {
            for (int j = 0; j < riverWidth; j++)
            {
                var pos = new Vector3Int(i, offset + j);
                riverMap.SetTile(pos, riverTiles.GetRiverTile(direction));
            }
        }
    }

    private void RoundCorner(Compass direction, int riverWidth)
    {
        //if (!roundCorner) return;

        var width = worldGenParams.LocalDimensions.x;
        var height = worldGenParams.LocalDimensions.y;
        int halfWidth = Mathf.RoundToInt(width / 2);
        int halfHeight = Mathf.RoundToInt(height / 2);

        var offset = Mathf.RoundToInt(riverWidth / 2);
        var center = new Vector2Int(halfWidth - offset, halfHeight - offset);
        var innerCorner = new Vector2Int(halfWidth, halfHeight);
        var corner = new List<Vector2Int>();
        for (int i = 0; i < riverWidth; i++)
        {
            for (int j = 0; j < riverWidth; j++)
            {
                var point = center + new Vector2Int(i, j);
                if (Vector2.Distance(innerCorner, point) < riverWidth * 0.5f)
                    corner.Add(point);
            }
        }

        switch (direction)
        {
            case Compass.NorthEast:
                for (int i = 0; i < corner.Count; i++)
                {
                    if (corner[i].x < halfWidth && corner[i].y < halfHeight)
                    {
                        var pos = new Vector3Int(corner[i].x, corner[i].y);
                        riverMap.SetTile(pos, riverTiles.GetRiverTile(direction));
                    }
                }
                break;
            case Compass.SouthEast:
                for (int i = 0; i < corner.Count; i++)
                {
                    if (corner[i].x < halfWidth && corner[i].y > halfHeight)
                    {
                        var pos = new Vector3Int(corner[i].x, corner[i].y);
                        riverMap.SetTile(pos, riverTiles.GetRiverTile(direction));
                    }
                }
                break;
            case Compass.SouthWest:
                for (int i = 0; i < corner.Count; i++)
                {
                    if (corner[i].x > halfWidth - 1 && corner[i].y > halfHeight)
                    {
                        var pos = new Vector3Int(corner[i].x, corner[i].y);
                        riverMap.SetTile(pos, riverTiles.GetRiverTile(direction));
                    }
                }
                break;
            case Compass.NorthWest:
                for (int i = 0; i < corner.Count; i++)
                {
                    if (corner[i].x > halfWidth - 1 && corner[i].y < halfHeight)
                    {
                        var pos = new Vector3Int(corner[i].x, corner[i].y);
                        riverMap.SetTile(pos, riverTiles.GetRiverTile(direction));
                    }
                }
                break;
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