using JS.ECS.Tags;
using JS.WorldMap;
using UnityEngine;

namespace JS.ECS
{
    public class LocomotionSystem : SystemBase<LocalLocomotion>
    {
        private static LocomotionSystem instance;

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
            return instance.TryMove(obj, direction, out cost);
        }

        private bool TryMove(Physics obj, Compass direction, out int cost)
        {
            cost = 0;
            if (!ProjectedPositionIsValid(obj.Transform, direction)) return false;

            var entitiesAtPosition = TransformSystem.GetLocalEntitiesAt(tracer, tracer.LocalPosition);
            if (entitiesAtPosition.Length > 0)
            {
                for (int i = 0; i < entitiesAtPosition.Length; i++)
                {
                    if (entitiesAtPosition[i].GetTag<BlocksNode>()) return false; //position is blocked by wall
                }
            }
            //the returned cost will have to be equal to the movement penalty for the declared space
            //by default this is 0, but can be modified by difficult terrain, terrain type, etc.

            //Change the Transform of the object to match the tracer's valid position
            obj.Transform.LocalPosition = tracer.LocalPosition;

            if (obj.Transform.RegionPosition != tracer.RegionPosition)
            {
                //Debug.Log("Region Position Changing");
                obj.Transform.RegionPosition = tracer.RegionPosition;

                if (obj.Transform.WorldPosition != tracer.WorldPosition)
                    obj.Transform.WorldPosition = tracer.WorldPosition;

                //Player crossed into a new map, change scenes
                if (obj.entity.GetTag<PlayerTag>())
                {
                    //Debug.Log("Player Switching Map");
                    GridManager.OnEnterLocalMap(obj.Transform.WorldPosition, obj.Transform.RegionPosition, obj.Transform.Depth);
                }
                //else Debug.Log("Player Tag not found");
            }
            return true;
        }

        public static bool TryMoveUp(Physics obj)
        {
            Debug.LogWarning("Not Yet Implemented.");
            if (obj.Transform.Depth > 0) return false;

            //Check for presence of stairs/ladder

            obj.Transform.Depth++;
            
            return false;
        }

        public static bool TryMoveDown(Physics obj)
        {
            Debug.LogWarning("Not Yet Implemented.");
            //Check for presence of stairs/holes

            obj.Transform.Depth--;

            return false;
        }

        /// <summary>
        /// Places a tracer at the projected position and returns true if it is valid.
        /// </summary>
        private bool ProjectedPositionIsValid(Transform transform, Compass direction)
        {
            tracer.LocalPosition = transform.LocalPosition;
            tracer.RegionPosition = transform.RegionPosition;
            tracer.WorldPosition = transform.WorldPosition;

            tracer.LocalPosition += DirectionHelper.GetVector(direction);
            if (WithinLocalMap()) return true;

            WrapLocal(tracer); //Wrap local position to reflect change in region map

            //Moved outside local map, Wrap position, move to next region
            tracer.RegionPosition += DirectionHelper.GetVector(direction);
            if (WithinRegionMap()) return true;

            WrapRegion(tracer); //Wrap region position to reflect chane in world map

            tracer.WorldPosition += DirectionHelper.GetVector(direction);
            if (WithinWorldMap()) return true;
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

        private bool WithinLocalMap()
        {
            if (tracer.LocalPosition.x < 0) return false;
            if (tracer.LocalPosition.y < 0) return false;
            if (tracer.LocalPosition.x > worldMapParams.LocalDimensions.x - 1) return false;
            if (tracer.LocalPosition.y > worldMapParams.LocalDimensions.y - 1) return false;
            return true;
        }

        private bool WithinRegionMap()
        {
            if (tracer.RegionPosition.x < 0) return false;
            if (tracer.RegionPosition.y < 0) return false;
            if (tracer.RegionPosition.x > worldMapParams.RegionDimensions.x - 1) return false;
            if (tracer.RegionPosition.y > worldMapParams.RegionDimensions.y - 1) return false;
            return true;
        }

        private bool WithinWorldMap()
        {
            if (tracer.WorldPosition.x < 0) return false;
            if (tracer.WorldPosition.y < 0) return false;
            if (tracer.WorldPosition.x > worldMap.Width - 1) return false;
            if (tracer.WorldPosition.y > worldMap.Height - 1) return false;
            return true;
        }

        /*****************************
         * Not Currently Being Used
         ****************************/
        private bool WithinLocalMap(Transform transform, Compass direction)
        {
            var projectedPos = transform.LocalPosition + DirectionHelper.GetVector(direction);
            if (projectedPos.x < 0) return false;
            if (projectedPos.y < 0) return false;
            if (projectedPos.x > worldMapParams.LocalDimensions.x - 1) return false;
            if (projectedPos.y > worldMapParams.LocalDimensions.y - 1) return false;
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

        private bool WithinWorldMap(Transform transform, Compass direction)
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