using UnityEngine;

namespace JS.ECS
{
    public class Transform : ComponentBase
    {
        public Vector2 position = Vector2.zero;
        public int layerDepth = 0;
        public float rotation = 0;

        public Transform(Entity entity)
        {
            this.entity = entity;
        }
    }
}