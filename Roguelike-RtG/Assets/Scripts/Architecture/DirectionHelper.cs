using UnityEngine;

public static class DirectionHelper
{
    public static Vector2Int GetVector(Compass compass)
    {
        switch (compass)
        {
            case Compass.North: return Vector2Int.up; ;
            case Compass.South: return Vector2Int.down;
            case Compass.East: return Vector2Int.right;
            case Compass.West: return Vector2Int.left;

            case Compass.NorthEast: return Vector2Int.one;
            case Compass.NorthWest: return Vector2Int.up + Vector2Int.left;
            case Compass.SouthEast: return Vector2Int.down + Vector2Int.right;
            case Compass.SouthWest: return Vector2Int.down + Vector2Int.left;

            default: return Vector2Int.zero;
        }
    }
}