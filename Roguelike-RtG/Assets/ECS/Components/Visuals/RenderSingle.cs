using UnityEngine;

namespace JS.ECS
{
    public class RenderSingle : RenderBase
    {
        public Sprite sprite;

        public RenderSingle(Transform transform, Sprite sprite)
        {
            entity = transform.entity;
            this.transform = transform;
            this.sprite = sprite;
            RenderSystem.NewRender(this);
        }

        public override void Disassemble()
        {
            base.Disassemble();
            sprite = null;
        }
    }
}