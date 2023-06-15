using JS.WorldMap;
using UnityEngine;

namespace JS.ECS
{
    public class WorldLocomotionSystem : SystemBase<WorldLocomotion>
    {
        private static WorldLocomotionSystem instance;

        [SerializeField] private WorldData worldMap;
        [SerializeField] private WorldGenerationParameters worldGenParams;

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

            regionCenter = new Vector2Int(Mathf.FloorToInt(worldGenParams.RegionDimensions.x * 0.5f),
                Mathf.FloorToInt(worldGenParams.RegionDimensions.y * 0.5f));
            localCenter = new Vector2Int(Mathf.FloorToInt(worldGenParams.LocalDimensions.x * 0.5f),
                Mathf.FloorToInt(worldGenParams.LocalDimensions.y * 0.5f));
        }

        /// <summary>
        /// Fast travel on the World Map from World Tile to World Tile. Returns additional costs given the Terrain.
        /// </summary>
        public static bool TryMoveWorld(Transform t, Compass direction, out int cost)
        {
            cost = 0;
            if (!GridManager.WorldMapActive) return false; //Can only fast travel when viewing the world map
            if (!instance.WithinWorldBounds((Vector2Int)t.Position + DirectionHelper.GetVector(direction))) return false;

            //the returned cost will have to be equal to the movement penalty for the declared space
            //by default this is 0, but can be modified by difficult terrain, terrain type, etc.

            //So this moves them a full world tile but keeps their regional position within a world tile the same
            var newPos = t.Position + (Vector3Int)DirectionHelper.GetVector(direction) * instance.worldGenParams.RegionDimensions.x;
            TransformSystem.SetPosition(t, newPos);
            //obj.LocalPosition = instance.localCenter;

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
        public static bool SwitchToLocalMap(Transform player)
        {
            //What would prevent this?
            return instance.SwitchFromWorldToLocalMap(player);
        }

        private bool SwitchFromWorldToLocalMap(Transform player)
        {
            //Don't know how, but make sure no other entity can swap map focus
            if (player.entity != EntityManager.Player) return false;

            GridManager.OnEnterLocalMap(player.Position);
            player.LocalPosition = localCenter;
            return true;
        }

        /// <summary>
        /// Switch to World Map view from Local Map.
        /// </summary>
        public static bool SwitchToWorldMap(Transform player)
        {
            return instance.SwitchFromLocalToWorldMap(player);
        }

        private bool SwitchFromLocalToWorldMap(Transform player)
        {
            //Don't know how, but make sure no other entity can swap map focus
            if (player.entity != EntityManager.Player) return false;

            //Can only switch to World Map if on the surface
            if (player.Position.z < 0) return false;
            
            //Will also need to check if the player is in combat, possibly other factors
            //Maybe terrain, etc.

            GridManager.OnSwitchToWorldView();

            return true;
        }
    }
}