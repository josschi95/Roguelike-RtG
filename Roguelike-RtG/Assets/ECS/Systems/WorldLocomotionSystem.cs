using UnityEngine;
using JS.World.Map;

namespace JS.ECS
{
    public class WorldLocomotionSystem : SystemBase<WorldLocomotion>
    {
        private static WorldLocomotionSystem instance;

        [SerializeField] private WorldData worldMap;

        private Vector2Int regionCenter;
        private Vector2Int localCenter;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;

            regionCenter = new Vector2Int(Mathf.FloorToInt(WorldParameters.REGION_WIDTH * 0.5f),
                Mathf.FloorToInt(WorldParameters.REGION_HEIGHT * 0.5f));
            localCenter = new Vector2Int(Mathf.FloorToInt(WorldParameters.LOCAL_WIDTH * 0.5f),
                Mathf.FloorToInt(WorldParameters.LOCAL_HEIGHT * 0.5f));
        }

        /// <summary>
        /// Fast travel on the World Map from World Tile to World Tile. Returns additional costs given the Terrain.
        /// </summary>
        public static bool TryMoveWorld(Transform t, Compass direction, out int cost)
        {
            cost = 0;
            if (!GridManager.WorldMapActive) return false; //Can only fast travel when viewing the world map
            if (!instance.WithinWorldBounds((Vector2Int)t.WorldPosition + DirectionHelper.GetVector(direction)))
            {
                //Debug.Log("Not within world bounds");
                return false;
            }

            //the returned cost will have to be equal to the movement penalty for the declared space
            //by default this is 0, but can be modified by difficult terrain, terrain type, etc.

            //So this moves them a full world tile but keeps their regional position within a world tile the same
            //var newPos = t.RegionPosition + (Vector3Int)DirectionHelper.GetVector(direction) * instance.worldGenParams.RegionDimensions.x;
            var newPos = t.WorldPosition + (Vector3Int)DirectionHelper.GetVector(direction);

            TransformSystem.SetPosition(t, newPos, instance.regionCenter, instance.localCenter);
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

        /// <summary>
        /// Switches to Local Map view from World Map.
        /// </summary>
        public static bool SwitchToLocalMap(Transform transform)
        {
            //What would prevent this?
            return instance.SwitchFromWorldToLocalMap(transform);
        }

        private bool SwitchFromWorldToLocalMap(Transform transform)
        {
            //Don't know how, but make sure no other entity can swap map focus
            if (!EntityManager.TryGetComponent<CameraFocus>(transform.entity, out _)) return false;

            GridManager.OnEnterLocalMap(transform.WorldPosition, transform.RegionPosition);
            return true;
        }

        /// <summary>
        /// Switch to World Map view from Local Map.
        /// </summary>
        public static bool SwitchToWorldMap(Transform transform)
        {
            return instance.SwitchFromLocalToWorldMap(transform);
        }

        private bool SwitchFromLocalToWorldMap(Transform transform)
        {
            //Don't know how, but make sure no other entity can swap map focus
            if (!EntityManager.TryGetComponent<InputHandler>(transform.entity, out _)) return false;

            //Can only switch to World Map if on the surface
            if (transform.WorldPosition.z < 0) return false;
            
            //Will also need to check if the player is in combat, possibly other factors
            //Maybe terrain, etc.

            GridManager.OnSwitchToWorldView();

            return true;
        }
    }
}