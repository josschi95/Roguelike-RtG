using UnityEngine;

namespace JS.ECS
{
    public class RenderSingle : RenderBase
    {
        public SpriteRenderer renderer;
        public Sprite sprite;

        public RenderSingle(Transform transform, Sprite sprite)
        {
            entity = transform.entity;
            this.transform = transform;
            this.sprite = sprite;
            RenderSystem.NewSingle(this);
        }

        public override void Release()
        {
            base.Release();
            transform = null;
            sprite = null;
            RenderSystem.RemoveSinge(this);
        }
    }
}