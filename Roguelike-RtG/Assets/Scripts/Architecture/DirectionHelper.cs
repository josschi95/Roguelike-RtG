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

    public static Compass CombineDirections(Compass fromDirection, Compass toDirection)
    {
        switch (fromDirection)
        {
            case Compass.North:
                if (toDirection == Compass.East) return Compass.NorthEast;
                else if (toDirection == Compass.West) return Compass.NorthWest;
                else return Compass.North;
            case Compass.South:
                if (toDirection == Compass.East) return Compass.SouthEast;
                else if (toDirection == Compass.West) return Compass.SouthWest;
                else return Compass.North;
            case Compass.East:
                if (toDirection == Compass.North) return Compass.NorthEast;
                else if (toDirection == Compass.South) return Compass.SouthEast;
                else return Compass.East;
            case Compass.West:
                if (toDirection == Compass.North) return Compass.NorthWest;
                else if (toDirection == Compass.South) return Compass.SouthWest;
                else return Compass.East;
        }
        throw new System.Exception("Compass Direction outside parameters");
    }

    public static Compass ReflectDirection(Compass direction)
    {
        switch (direction)
        {
            case Compass.North: return Compass.South;
            case Compass.South: return Compass.North;
            case Compass.East: return Compass.West;
            case Compass.West: return Compass.East;


        }
        throw new System.Exception("Compass Direction outside parameters");
    }

    public static Compass GetRandom()
    {
        return (Compass)Random.Range(0, System.Enum.GetNames(typeof(Compass)).Length);
    }
}