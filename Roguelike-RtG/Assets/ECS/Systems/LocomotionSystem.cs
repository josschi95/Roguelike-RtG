using JS.ECS.Tags;
using JS.WorldMap;
using UnityEngine;

namespace JS.ECS
{
    public class LocomotionSystem : MonoBehaviour
    {
        private static LocomotionSystem instance;

        public const float movementDividend = 100000;

        [SerializeField] private WorldGenerationParameters worldMapParams;
        [SerializeField] private WorldData worldMap;

        private Vector3Int worldTracer = Vector3Int.zero;
        private Vector2Int regionTracer = Vector2Int.zero;
        private Vector2Int localTracer = Vector2Int.zero;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;
        }

        public static bool TryMoveLocal(Transform obj, Compass direction, out int cost)
        {
            return instance.TryMove(obj, direction, out cost);
        }

        private bool TryMove(Transform obj, Compass direction, out int cost)
        {
            cost = 0;
            if (!ProjectedPositionIsValid(obj, direction))
            {
                Debug.Log("Position not valid");
                return false;
            }

            //Highly considering storing all entities within GridManager's GameGrid class, sort of like a Scene folder
            var entitiesAtPosition = TransformSystem.GetEntitiesAt(worldTracer, regionTracer, localTracer);
            for (int i = 0; i < entitiesAtPosition.Count; i++)
            {
                if (EntityManager.GetTag<BlocksNode>(entitiesAtPosition[i].entity)) return false; //position is blocked by wall
                else if (EntityManager.TryGetComponent<Brain>(entitiesAtPosition[i].entity, out _))
                {
                    MessageSystem.NewMessage("The " + entitiesAtPosition[i].entity.Name + " blocks your path.");
                    return false; //there is a creature there
                }
            }
            //the returned cost will have to be equal to the movement penalty for the declared space
            //by default this is 0, but can be modified by difficult terrain, terrain type, etc.
            
            if (obj.WorldPosition != worldTracer) TransformSystem.SetPosition(obj, worldTracer, regionTracer, localTracer);
            else if (obj.RegionPosition != regionTracer) TransformSystem.SetPosition(obj, worldTracer, regionTracer, localTracer);
            else TransformSystem.SetLocal(obj, localTracer);
            return true;
        }

        public static bool TryMoveUp(Transform obj)
        {
            Debug.LogWarning("Not Yet Implemented.");
            if (obj.WorldPosition.z >= 0) return false;

            if (obj.CurrentNode.stairsUp)
            {
                obj.WorldPosition += Vector3Int.forward;
                return true;
            }
            MessageSystem.NewMessage("There are no stairs leading up");
            return false;            
        }

        public static bool TryMoveDown(Transform obj)
        {
            Debug.LogWarning("Not Yet Implemented.");

            if (obj.CurrentNode.stairsDown)
            {
                obj.WorldPosition += Vector3Int.back;
                return true;
            }
            MessageSystem.NewMessage("There are no stairs leading down");
            return false;
        }

        /// <summary>
        /// Places a tracer at the projected position and returns true if it is valid.
        /// </summary>
        private bool ProjectedPositionIsValid(Transform physics, Compass direction)
        {
            worldTracer = physics.WorldPosition;
            regionTracer = physics.RegionPosition;
            localTracer = physics.LocalPosition;

            localTracer += DirectionHelper.GetVector(direction);
            if (!WithinLocalMap()) WrapLocal(); //Wrap local position to reflect change in map
            else return true;

            regionTracer += DirectionHelper.GetVector(direction);
            if (!WithinRegionMap()) WrapRegion();
            else return true;

            worldTracer += (Vector3Int)DirectionHelper.GetVector(direction);
            if (WithinWorldMap()) return true;
            return false;
        }

        private bool WithinLocalMap()
        {
            if (localTracer.x < 0) return false;
            if (localTracer.y < 0) return false;
            if (localTracer.x > worldMapParams.LocalDimensions.x - 1) return false;
            if (localTracer.y > worldMapParams.LocalDimensions.y - 1) return false;
            return true;
        }

        private void WrapLocal()
        {
            //Wrap X
            if (localTracer.x < 0) localTracer.x = worldMapParams.LocalDimensions.x - 1;
            else if (localTracer.x > worldMapParams.LocalDimensions.x - 1) localTracer.x = 0;
            //Wrap Y
            if (localTracer.y < 0) localTracer.y = worldMapParams.LocalDimensions.y - 1;
            else if (localTracer.y > worldMapParams.LocalDimensions.y - 1) localTracer.y = 0;
        }

        private bool WithinRegionMap()
        {
            if (regionTracer.x < 0) return false;
            if (regionTracer.y < 0) return false;
            if (regionTracer.x > worldMapParams.RegionDimensions.x - 1) return false;
            if (regionTracer.y > worldMapParams.RegionDimensions.y - 1) return false;
            return true;
        }

        private void WrapRegion()
        {
            //Wrap X
            if (regionTracer.x < 0) regionTracer.x = worldMapParams.RegionDimensions.x - 1;
            else if (regionTracer.x > worldMapParams.RegionDimensions.x - 1) regionTracer.x = 0;
            //Wrap Y
            if (regionTracer.y < 0) regionTracer.y = worldMapParams.RegionDimensions.y - 1;
            else if (regionTracer.y > worldMapParams.RegionDimensions.y - 1) regionTracer.y = 0;
        }

        private bool WithinWorldMap()
        {
            if (worldTracer.x < 0) return false;
            if (worldTracer.y < 0) return false;
            if (worldTracer.x > worldMap.Width - 1) return false;
            if (worldTracer.y > worldMap.Height - 1) return false;
            return true;
        }
    }
}