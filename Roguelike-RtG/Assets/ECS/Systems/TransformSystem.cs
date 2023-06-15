using System.Collections.Generic;
using UnityEngine;

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
            instance.transforms.Remove(t);
        }

        public static void SetPosition(Transform t, Vector3Int world, Vector2Int local)
        {
            t.Position = world;
            SetLocal(t, local);
        }

        public static void SetPosition(Transform t, Vector3Int pos)
        {
            t.Position = pos;
            EntityManager.FireEvent(t.entity, transformChanged);
        }

        public static void SetLocal(Transform t, Vector2Int pos)
        {
            if (t.CurrentNode == null) GetCurrentNode(t);
            if (t.CurrentNode == null) return;

            t.CurrentNode.Entities.Remove(t.entity);
            t.LocalPosition = pos;
            GetCurrentNode(t);
            t.CurrentNode.Entities.Add(t.entity);

            EntityManager.FireEvent(t.entity, transformChanged);
        }

        private static void GetCurrentNode(Transform t)
        {
            var grid = GridManager.GetGrid(t.Position);
            if (grid == null) return;
            t.CurrentNode = grid.Grid.GetGridObject(t.LocalPosition.x, t.LocalPosition.y);
        }

        public static Vector2Int GetWorldMapPos(Transform t)
        {
            int x = Mathf.FloorToInt(t.Position.x / 3);
            int y = Mathf.FloorToInt(t.Position.y / 3);
            return new Vector2Int(x, y);
        }

        public static List<Physics> GetEntitiesAt(Vector3Int world,  Vector2Int local)
        {
            var entities = new List<Physics>();
            var grid = GridManager.GetGrid(world);
            if (grid == null) return entities;

            var list = grid.Grid.GetGridObject(local.x, local.y).Entities;

            foreach (var entity in list )
            {
                if (EntityManager.TryGetComponent<Physics>(entity, out var phys))
                {
                    if (!phys.IsReal) continue;
                    entities.Add(phys);
                }
            }

            return entities;
        }

        public static Physics[] GetLocalEntitiesInLine(Vector3Int world, Vector2Int localStart, Compass direction, int range)
        {
            var entities = new List<Physics>();
            var grid = GridManager.GetGrid(world);
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

        public static Physics[] GetTakeablesAt(Vector3Int world, Vector2Int local)
        {
            var entities = new List<Physics>();
            var phys = GetEntitiesAt(world, local);
            for (int i = 0; i < phys.Count; i++)
            {
                if (phys[i].IsTakeable) entities.Add(phys[i]);
            }

            return entities.ToArray();
        }
    }
}