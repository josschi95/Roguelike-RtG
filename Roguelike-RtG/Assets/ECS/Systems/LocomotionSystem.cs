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

        public static bool TryMoveLocal(Physics obj, Compass direction, out int cost)
        {
            return instance.TryMove(obj, direction, out cost);
        }

        private bool TryMove(Physics obj, Compass direction, out int cost)
        {
            cost = 0;
            if (!ProjectedPositionIsValid(obj, direction)) return false;

            //Highly considering storing all entities within GridManager's GameGrid class, sort of like a Scene folder
            var entitiesAtPosition = TransformSystem.GetEntitiesAt(worldTracer, localTracer);
            for (int i = 0; i < entitiesAtPosition.Length; i++)
            {
                if (EntityManager.GetTag<BlocksNode>(entitiesAtPosition[i].entity)) return false; //position is blocked by wall
            }
            //the returned cost will have to be equal to the movement penalty for the declared space
            //by default this is 0, but can be modified by difficult terrain, terrain type, etc.

            //Change the Transform of the object to match the tracer's valid position
            //obj.LocalPosition = tracer.LocalPosition;
            obj.LocalPosition = localTracer;
            if (obj.Position != worldTracer)
            {
                //Debug.Log("Region Position Changing");
                obj.Position = worldTracer;

                //Player crossed into a new map, change scenes
                if (obj.entity == EntityManager.Player)
                {
                    //Debug.Log("Player Switching Map");
                    GridManager.OnEnterLocalMap(obj.Position);
                }
                //else Debug.Log("Player Tag not found");
            }
            return true;
        }

        public static bool TryMoveUp(Physics obj)
        {
            Debug.LogWarning("Not Yet Implemented.");
            if (obj.Position.z >= 0) return false;

            if (obj.CurrentNode.stairsUp)
            {
                obj.Position += Vector3Int.forward;
                return true;
            }
            MessageSystem.NewMessage("There are no stairs leading up");
            return false;            
        }

        public static bool TryMoveDown(Physics obj)
        {
            Debug.LogWarning("Not Yet Implemented.");

            if (obj.CurrentNode.stairsDown)
            {
                obj.Position += Vector3Int.back;
                return true;
            }
            MessageSystem.NewMessage("There are no stairs leading down");
            return false;
        }

        /// <summary>
        /// Places a tracer at the projected position and returns true if it is valid.
        /// </summary>
        private bool ProjectedPositionIsValid(Physics physics, Compass direction)
        {
            worldTracer = physics.Position;
            localTracer = physics.LocalPosition;

            localTracer += DirectionHelper.GetVector(direction);
            if (WithinLocalMap()) return true;

            WrapLocal(); //Wrap local position to reflect change in map

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