using UnityEngine;

namespace JS.ECS
{
    public class LocomotionSystem : SystemBase<Locomotion>
    {
        private static LocomotionSystem instance;

        private const float movementDivident = 100000;

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
            //Is position Obstructed?

            return true;
        }

        public static bool TryMoveEntity(Locomotion entity, Compass direction, out int cost)
        {
            cost = 0;
            if (!CanMoveToPosition(GetDirection(direction))) return false;

            //Will also need to take into account difficult terrain, movement modifiers, etc. 
            //Movement modifiers should affect Locomotion directly, and have no impact on this
            cost = Mathf.RoundToInt(movementDivident / entity.MovementSpeed);

            entity.Transform.localPosition += GetDirection(direction);
            entity.Transform.onTransformChanged?.Invoke();

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