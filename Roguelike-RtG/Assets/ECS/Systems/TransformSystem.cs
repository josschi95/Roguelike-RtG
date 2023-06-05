using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JS.ECS
{
    public class TransformSystem : SystemBase<Transform>
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

        /// <summary>
        /// Returns an array of entities at the given local position.
        /// </summary>
        public static Entity[] GetLocalEntitiesAt(Transform t, Vector2Int localPosition)
        {
            var entities = new List<Entity>();
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].WorldPosition != t.WorldPosition) continue;
                if (components[i].RegionPosition != t.RegionPosition) continue;
                if (components[i].LocalPosition == localPosition) entities.Add(components[i].entity);
            }
            return entities.ToArray();
        }
    }
}

