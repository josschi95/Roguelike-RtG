using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace JS.ECS
{
    public class TransformSystem : MonoBehaviour
    {
        public static TransformSystem instance;

        private List<Transform> transforms;
        private static TransformChanged transformChanged;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;
            transforms = new List<Transform>();
            transformChanged = new TransformChanged();
        }

        public static void Register(Transform t)
        {
            instance.transforms.Add(t);
        }

        public static void Unregister(Transform t)
        {
            t.CurrentNode?.Entities.Remove(t.entity);
            instance.transforms.Remove(t);
        }

        public static void SetPosition(Transform t, Vector3Int world, Vector2Int region, Vector2Int local)
        {
            t.WorldPosition = world;
            t.RegionPosition = region;
            SetLocal(t, local);
        }

        /// <summary>
        /// Sets all of t's positions to to's positions
        /// </summary>
        public static void SetPosition(Transform t, Transform to)
        {
            t.WorldPosition = to.WorldPosition;
            t.RegionPosition = to.RegionPosition;
            SetLocal(t, to.LocalPosition);
        }

        public static void SetWorldPosition(Transform t, Vector3Int pos)
        {
            t.WorldPosition = pos;
            EntityManager.FireEvent(t.entity, transformChanged);
        }

        public static void SetRegionPosition(Transform t, Vector2Int pos)
        {
            t.RegionPosition = pos;
            EntityManager.FireEvent(t.entity, transformChanged);
        }

        public static void SetLocal(Transform transform, Vector2Int pos)
        {
            transform.LocalPosition = pos;

            if (transform.CurrentNode == null) GetCurrentNode(transform);
            if (transform.CurrentNode == null)
            {
                EntityManager.FireEvent(transform.entity, transformChanged);
                return;
            }

            transform.CurrentNode.Entities.Remove(transform.entity);
            
            GetCurrentNode(transform);

            if (!transform.CurrentNode.Entities.Contains(transform.entity))
                transform.CurrentNode.Entities.Add(transform.entity);

            EntityManager.FireEvent(transform.entity, transformChanged);
        }

        private static void GetCurrentNode(Transform t)
        {
            var grid = GridManager.GetGrid(t.WorldPosition, t.RegionPosition);
            if (grid == null) return;
            t.CurrentNode = grid.Grid.GetGridObject(t.LocalPosition.x, t.LocalPosition.y);
        }

        public static List<Physics> GetEntitiesAt(Transform t, Vector2Int local)
        {
            return GetEntitiesAt(t.WorldPosition, t.RegionPosition, local);
        }

        public static List<Physics> GetEntitiesAt(Vector3Int world, Vector2Int region, Vector2Int local)
        {
            var entities = new List<Physics>();
            var grid = GridManager.GetGrid(world, region);
            if (grid == null) return entities;

            foreach (var entity in grid.Grid.GetGridObject(local.x, local.y).Entities)
            {
                if (EntityManager.TryGetComponent<Physics>(entity, out var phys))
                {
                    if (!phys.IsReal) continue;
                    entities.Add(phys);
                }
            }

            return entities;
        }

        public static Physics[] GetLocalEntitiesInLine(Vector3Int world, Vector2Int region, Vector2Int localStart, Compass direction, int range)
        {
            var entities = new List<Physics>();
            var grid = GridManager.GetGrid(world, region);
            if (grid == null) return entities.ToArray();

            var nodes = new GridNode[range];
            for (int i = 0; i <  range; i++)
            {
                var pos = localStart + (DirectionHelper.GetVector(direction) * i);
                nodes[i] = grid.Grid.GetGridObject(pos.x, pos.y);
            }

            for (int i = 0; i < nodes.Length; i++)
            {
                for (int j = 0; j < nodes[i].Entities.Count; j++)
                {
                    if (EntityManager.TryGetComponent<Physics>(nodes[i].Entities[j], out var phys))
                    {
                        if (phys.IsReal) entities.Add(phys);
                    }
                }
            }

            return entities.ToArray();
        }

        public static List<Physics> GetTakeablesAt(Transform t)
        {
            var entities = new List<Physics>();
            var phys = GetEntitiesAt(t, t.LocalPosition);
            for (int i = 0; i < phys.Count; i++)
            {
                if (phys[i].IsTakeable) entities.Add(phys[i]);
            }

            return entities;
        }
    }
}