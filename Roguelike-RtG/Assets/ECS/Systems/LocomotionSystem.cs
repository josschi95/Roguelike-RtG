using JS.WorldMap;
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

        [SerializeField] private WorldGenerationParameters worldMapParams;
        [SerializeField] private WorldData worldMap;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;
        }

        public static bool CanMoveToPosition(Transform transform, Compass direction)
        {
            if (!instance.WithinWorldBounds(transform, direction)) return false;

            //Is position valid?
            //Is position Obstructed/Blocked?

            return true;
        }

        public static bool TryMoveObject(Physics obj, Compass direction, out int cost)
        {
            cost = 0;
            if (!CanMoveToPosition(obj.Transform, direction)) return false;

            //the returned cost will have to be equal to the movement penalty for the declared space
            //by default this is 0, but can be modified by difficult terrain, terrain type, etc.

            switch (obj.Transform.MapLevel)
            {
                case MapLevel.Local:
                    obj.Transform.LocalPosition += GetDirection(direction);
                    break;
                case MapLevel.World:
                    obj.Transform.WorldPosition += GetDirection(direction);
                    break;
            }
            return true;
        }

        private bool WithinWorldBounds(Transform transform, Compass direction)
        {
            if (transform.MapLevel == MapLevel.World)
            {
                var projectedPos = transform.WorldPosition + GetDirection(direction);
                if (projectedPos.x < 0) return false;
                if (projectedPos.y < 0) return false;
                if (projectedPos.x > worldMap.Width - 1) return false;
                if (projectedPos.y > worldMap.Height - 1) return false;
            }
            else
            {
                //first check if they're on the edge of the map
            }

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