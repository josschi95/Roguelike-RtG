using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace JS.ECS
{
    public class LocomotionSystem : SystemBase<Locomotion>
    {
        private static LocomotionSystem instance;

        private const float movementDivident = 100000;

        private Vector2Int _north = new Vector2Int(0, 1);
        private Vector2Int _south = new Vector2Int(0, -1);
        private Vector2Int _east = new Vector2Int(1, 0);
        private Vector2Int _west = new Vector2Int(-1, 0);
        
        private Vector2Int _northEast = new Vector2Int(0,1);
        private Vector2Int _northWest = new Vector2Int(0,1);
        private Vector2Int _southEast = new Vector2Int(0,1);
        private Vector2Int _southWest = new Vector2Int(0,1);

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

        public static int MoveEntity(Locomotion entity, Compass direction)
        {
            if (!CanMoveToPosition(GetDirection(direction))) return 0;

            //Will also need to take into account difficult terrain, movement modifiers, etc. 
            //Movement modifiers should affect Locomotion directly, and have no impact on this
            int cost = Mathf.RoundToInt(movementDivident / entity.MovementSpeed);

            entity.Transform.localPosition += GetDirection(direction);
            entity.Transform.onTransformChanged?.Invoke();
            return cost;
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