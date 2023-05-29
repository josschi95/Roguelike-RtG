using UnityEngine;

namespace JS.ECS
{
    public class LocomotionSystem : SystemBase<Locomotion>
    {
        private static LocomotionSystem instance;

        public const float movementDividend = 100000;

        private Vector2Int _north = Vector2Int.up;
        private Vector2Int _south = Vector2Int.down;
        private Vector2Int _east = Vector2Int.right;
        private Vector2Int _west = Vector2Int.left;

        private Vector2Int _northEast = Vector2Int.one;
        private Vector2Int _northWest = Vector2Int.up + Vector2Int.left;
        private Vector2Int _southEast = Vector2Int.down + Vector2Int.right;
        private Vector2Int _southWest = Vector2Int.down + Vector2Int.left;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;
        }

        public static bool CanMoveToPosition(Vector2Int position)
        {
            //Is position valid?
            //Is position Obstructed/Blocked?

            return true;
        }

        public static bool TryMoveObject(Physics obj, Compass direction, out int cost)
        {
            cost = 0;
            if (!CanMoveToPosition(GetDirection(direction))) return false;

            //the returned cost will have to be equal to the movement penalty for the declared space
            //by default this is 0, but can be modified by difficult terrain, terrain type, etc.

            obj.Transform.LocalPosition += GetDirection(direction);
            return true;
        }

        public static Vector2Int GetDirection(Compass compass)
        {
            return instance.GetDir(compass);
        }

        private  Vector2Int GetDir(Compass compass)
        {
            switch(compass)
            {
                case Compass.North: return _north;
                case Compass.South: return _south;
                case Compass.East: return _east;
                case Compass.West: return _west;

                case Compass.NorthEast: return _northEast;
                case Compass.NorthWest: return _northWest;
                case Compass.SouthEast: return _southEast;
                case Compass.SouthWest: return _southWest;

                default: return Vector2Int.zero;
            }
        }
    }
}