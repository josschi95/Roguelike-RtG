using UnityEngine;

namespace JS.ECS
{
    public class Transform : ComponentBase
    {
        public delegate void OnTransformChanged();
        public OnTransformChanged onTransformChanged;

        public Vector2 worldPosition;
        public Vector2 regionPosition;
        public Vector2 localPosition;

        public Transform(Entity entity)
        {
            this.entity = entity;
        }

        public override void Disassemble()
        {
            base.Disassemble();
            onTransformChanged = null;
        }
    }
}