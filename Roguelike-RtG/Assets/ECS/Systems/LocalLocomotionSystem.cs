using JS.WorldMap;
using UnityEngine;

namespace JS.ECS
{
    public class LocalLocomotionSystem : SystemBase<LocalLocomotion>
    {
        private static LocalLocomotionSystem instance;

        public const float movementDividend = 100000;

        [SerializeField] private WorldGenerationParameters worldMapParams;
        [SerializeField] private WorldData worldMap;

        private Transform tracer;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;
            GetTracer();
        }

        private void GetTracer()
        {
            var entity = new Entity("Tracer");
            entity.AddComponent(new Transform());
            tracer = entity.GetComponent<Transform>();
        }

        public static bool TryMoveLocal(Physics obj, Compass direction, out int cost)
        {
            cost = 0;
            instance.GetProjectedPosition(obj.Transform, direction);

            if (!instance.CanMoveToPosition(obj.Transform, direction)) return false;

            //the returned cost will have to be equal to the movement penalty for the declared space
            //by default this is 0, but can be modified by difficult terrain, terrain type, etc.

            obj.Transform.LocalPosition += DirectionHelper.GetVector(direction);
            return true;
        }

        public static bool TryMoveUp(Physics obj)
        {
            if (obj.Transform.Depth > 0) return false;

            obj.Transform.Depth++;
            
            return true;
        }



        private bool CanMoveToPosition(Transform transform, Compass direction)
        {
            if (!WithinWorldBounds(transform, direction)) return false;


            //Is position valid?
            //Is position Obstructed/Blocked?

            return true;
        }

        private bool GetProjectedPosition(Transform transform, Compass direction)
        {
            tracer.LocalPosition = transform.LocalPosition;
            tracer.RegionPosition = transform.RegionPosition;
            tracer.WorldPosition = transform.WorldPosition;

            if (WithinLocalMap(tracer, direction))
            {
                //Can move here, no change in local map
                tracer.LocalPosition += DirectionHelper.GetVector(direction);
                return true;
            }

            //Moved outside local map, Wrap position, move to next region
            WrapLocal(tracer);

            if (WithinRegionMap(tracer, direction))
            {
                //No change in region
                tracer.RegionPosition += DirectionHelper.GetVector(direction);
                return true;
            }

            WrapRegion(tracer);

            if (WithinWorldBounds(tracer, direction))
            {
                tracer.WorldPosition += DirectionHelper.GetVector(direction);
                return true;
            }
            return false;
        }

        private void WrapLocal(Transform transform)
        {
            //Wrap X
            if (transform.LocalPosition.x < 0)
                transform.LocalPosition = new Vector2Int(worldMapParams.LocalDimensions.x - 1, transform.LocalPosition.y);
            else if (transform.LocalPosition.x > worldMapParams.LocalDimensions.x - 1)
                transform.LocalPosition = new Vector2Int(0, transform.LocalPosition.y);
            //Wrap Y
            if (transform.LocalPosition.y < 0)
                transform.LocalPosition = new Vector2Int(transform.LocalPosition.x, worldMapParams.LocalDimensions.y - 1);
            else if (transform.LocalPosition.y > worldMapParams.LocalDimensions.y - 1)
                transform.LocalPosition = new Vector2Int(transform.LocalPosition.x, 0);
        }

        private void WrapRegion(Transform transform)
        {
            //Wrap X
            if (transform.RegionPosition.x < 0)
                transform.RegionPosition = new Vector2Int(worldMapParams.RegionDimensions.x - 1, transform.RegionPosition.y);
            else if (transform.RegionPosition.x > worldMapParams.RegionDimensions.x - 1)
                transform.RegionPosition = new Vector2Int(0, transform.RegionPosition.y);
            //Wrap Y
            if (transform.RegionPosition.y < 0)
                transform.RegionPosition = new Vector2Int(transform.RegionPosition.x, worldMapParams.RegionDimensions.y - 1);
            else if (transform.RegionPosition.y > worldMapParams.RegionDimensions.y - 1)
                transform.RegionPosition = new Vector2Int(transform.RegionPosition.x, 0);
        }

        private bool WithinLocalMap(Transform transform, Compass direction)
        {
            var projectedPos = transform.LocalPosition + DirectionHelper.GetVector(direction);
            if (projectedPos.x < 0) return false;
            if (projectedPos.y < 0) return false;
            if (projectedPos.x > worldMapParams.LocalDimensions.x - 1) return false;
            if (projectedPos.y >  worldMapParams.LocalDimensions.y - 1) return false;
            return true;
        }

        private bool WithinRegionMap(Transform transform, Compass direction)
        {
            var projectedPos = transform.RegionPosition + DirectionHelper.GetVector(direction);
            if (projectedPos.x < 0) return false;
            if (projectedPos.y < 0) return false;
            if (projectedPos.x > worldMapParams.RegionDimensions.x - 1) return false;
            if (projectedPos.y > worldMapParams.RegionDimensions.y - 1) return false;
            return true;
        }

        private bool WithinWorldBounds(Transform transform, Compass direction)
        {
            var projectedPos = transform.WorldPosition + DirectionHelper.GetVector(direction);
            if (projectedPos.x < 0) return false;
            if (projectedPos.y < 0) return false;
            if (projectedPos.x > worldMap.Width - 1) return false;
            if (projectedPos.y > worldMap.Height - 1) return false;
            return true;
        }
    }
}