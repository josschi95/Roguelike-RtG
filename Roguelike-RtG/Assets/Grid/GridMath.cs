using UnityEngine;

public static class GridMath
{
    public static float GetStraightDist(int fromX, int fromY, int toX, int toY)
    {
        return Mathf.Sqrt(Mathf.Pow(fromX - toX, 2) + Mathf.Pow(fromY - toY, 2));
    }

    public static float GetStraightDist(Vector2Int from, Vector2Int to)
    {
        return Mathf.Sqrt(Mathf.Pow(from.x - to.x, 2) + Mathf.Pow(from.y - to.y, 2));
    }

    public static float GetStraightDist(GridCoordinates fromNode, GridCoordinates toNode)
    {
        return GetStraightDist(fromNode.x, fromNode.y, toNode.x, toNode.y);
    }

    public static float GetStraightDist(GridCoordinates fromNode, int toX, int toY)
    {
        return GetStraightDist(fromNode.x, fromNode.y, toX, toY);
    }
}