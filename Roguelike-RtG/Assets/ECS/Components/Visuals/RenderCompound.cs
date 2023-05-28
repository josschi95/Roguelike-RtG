using UnityEngine;

namespace JS.ECS
{
    public class RenderCompound : RenderBase
    {
        public CompoundRenderer renderer;
        public Sprite[] sprites;

        public RenderCompound(Transform transform, Sprite[] sprites)
        {
            entity = transform.entity;
            this.transform = transform;

            this.sprites = new Sprite[10];
            for (int i = 0; i < sprites.Length; i++)
            {
                this.sprites[i] = sprites[i];
            }

            RenderSystem.NewCompound(this);
        }

        public override void Disassemble()
        {
            base.Disassemble();
            transform = null;
            sprites = null;
            RenderSystem.RemoveCompound(this);
        }
    }
}