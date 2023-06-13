using System.Collections.Generic;
using UnityEngine;

namespace JS.ECS
{
    public class TransformSystem : MonoBehaviour
    {
        public static TransformSystem instance;


        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;
        }

        public static Physics[] GetEntitiesAt(Vector3Int world,  Vector2Int local)
        {
            var entities = new List<Physics>();
            var grid = GridManager.GetGrid(world);
            if (grid == null) return entities.ToArray();

            var list = grid.Grid.GetGridObject(local.x, local.y).Entities;

            for (int i = 0; i < list.Count; i++)
            {
                if (EntityManager.TryGetComponent<Physics>(list[i], out var phys))
                {
                    if (!phys.IsReal) continue;
                    if (phys.LocalPosition == local) entities.Add(phys);
                }
            }

            return entities.ToArray();
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
    }
}

