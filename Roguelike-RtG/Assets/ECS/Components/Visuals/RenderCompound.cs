using UnityEngine;

namespace JS.ECS
{
    public class RenderCompound : RenderBase
    {
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

            RenderSystem.NewRender(this);
        }

        public override void OnEvent(Event newEvent)
        {
            base.OnEvent(newEvent);
        }

        public override void Disassemble()
        {
            base.Disassemble();
            sprites = null;
        }
    }
}