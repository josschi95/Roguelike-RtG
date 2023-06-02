using JS.EventSystem;
using JS.WorldMap;
using UnityEngine;

namespace JS.ECS
{
    public class WorldLocomotionSystem : SystemBase<WorldLocomotion>
    {
        private static WorldLocomotionSystem instance;

        [SerializeField] private WorldData worldMap;
        [SerializeField] private NodeReference nodeReference;
        [SerializeField] private GameEvent worldToLocalEvent;
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;
        }

        public static bool TryMoveWorld(Physics obj, Compass direction, out int cost)
        {
            cost = 0;
            if (!instance.WithinWorldBounds(obj.Transform.WorldPosition + DirectionHelper.GetVector(direction))) return false;

            //the returned cost will have to be equal to the movement penalty for the declared space
            //by default this is 0, but can be modified by difficult terrain, terrain type, etc.

            obj.Transform.WorldPosition += DirectionHelper.GetVector(direction);
            return true;
        }

        public static bool SwitchToLocalMap(Physics obj)
        {
            //What would prevent this? I can probably think of some that will stop swithcing to world from local, but...

            instance.nodeReference.x = obj.Transform.WorldPosition.x;
            instance.nodeReference.y = obj.Transform.WorldPosition.y;
            //GridSystem.OnEnterNewMap(obj.Transform.WorldPosition, Vector2Int.one, 0);
            instance.worldToLocalEvent?.Invoke();
            return true;
        }

        private bool WithinWorldBounds(Vector2Int projectedPosition)
        {
            if (projectedPosition.x < 0) return false;
            if (projectedPosition.y < 0) return false;
            if (projectedPosition.x > worldMap.Width - 1) return false;
            if (projectedPosition.y > worldMap.Height - 1) return false;
            return true;
        }
    }
}