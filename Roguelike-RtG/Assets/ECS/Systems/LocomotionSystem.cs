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

        private Physics tracer;

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
            entity.AddComponent(new Physics());
            tracer = entity.GetComponent<Physics>();
            tracer.IsTakeable = false;
            tracer.IsSolid = false;
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
            var entitiesAtPosition = TransformSystem.GetEntitiesAt(tracer.Position, tracer.LocalPosition);
            for (int i = 0; i < entitiesAtPosition.Length; i++)
            {
                if (entitiesAtPosition[i].entity.GetTag<BlocksNode>()) return false; //position is blocked by wall
            }
            //the returned cost will have to be equal to the movement penalty for the declared space
            //by default this is 0, but can be modified by difficult terrain, terrain type, etc.

            //Change the Transform of the object to match the tracer's valid position
            obj.LocalPosition = tracer.LocalPosition;

            if (obj.Position != tracer.Position)
            {
                //Debug.Log("Region Position Changing");
                obj.Position = tracer.Position;

                //Player crossed into a new map, change scenes
                if (obj.entity.GetTag<PlayerTag>())
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

            //Check for presence of stairs/ladder

            obj.Position += Vector3Int.forward;
            
            return false;
        }

        public static bool TryMoveDown(Physics obj)
        {
            Debug.LogWarning("Not Yet Implemented.");
            //Check for presence of stairs/holes

            obj.Position += Vector3Int.back;

            return false;
        }

        /// <summary>
        /// Places a tracer at the projected position and returns true if it is valid.
        /// </summary>
        private bool ProjectedPositionIsValid(Physics physics, Compass direction)
        {
            tracer.Position = physics.Position;
            tracer.LocalPosition = physics.LocalPosition;

            tracer.LocalPosition += DirectionHelper.GetVector(direction);
            if (WithinLocalMap()) return true;

            WrapLocal(tracer); //Wrap local position to reflect change in map

            tracer.Position += (Vector3Int)DirectionHelper.GetVector(direction);
            if (WithinWorldMap()) return true;
            return false;
        }

        private bool WithinLocalMap()
        {
            if (tracer.LocalPosition.x < 0) return false;
            if (tracer.LocalPosition.y < 0) return false;
            if (tracer.LocalPosition.x > worldMapParams.LocalDimensions.x - 1) return false;
            if (tracer.LocalPosition.y > worldMapParams.LocalDimensions.y - 1) return false;
            return true;
        }

        private void WrapLocal(Physics physics)
        {
            //Wrap X
            if (physics.LocalPosition.x < 0)
                physics.LocalPosition = new Vector2Int(worldMapParams.LocalDimensions.x - 1, physics.LocalPosition.y);
            else if (physics.LocalPosition.x > worldMapParams.LocalDimensions.x - 1)
                physics.LocalPosition = new Vector2Int(0, physics.LocalPosition.y);
            //Wrap Y
            if (physics.LocalPosition.y < 0)
                physics.LocalPosition = new Vector2Int(physics.LocalPosition.x, worldMapParams.LocalDimensions.y - 1);
            else if (physics.LocalPosition.y > worldMapParams.LocalDimensions.y - 1)
                physics.LocalPosition = new Vector2Int(physics.LocalPosition.x, 0);
        }

        private bool WithinWorldMap()
        {
            if (tracer.Position.x < 0) return false;
            if (tracer.Position.y < 0) return false;
            if (tracer.Position.x > worldMap.Width - 1) return false;
            if (tracer.Position.y > worldMap.Height - 1) return false;
            return true;
        }
    }
}