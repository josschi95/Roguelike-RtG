using UnityEngine;

namespace JS.ECS
{
    public class RenderSingle : RenderBase
    {
        public Sprite sprite;

        public RenderSingle(Physics physics, Sprite sprite, bool ifDark = false, bool onWorld = false)
        {
            entity = physics.entity;
            _physics = physics;
            this.sprite = sprite;
            RenderIfDark = ifDark;
            RenderOnWorldMap = onWorld;
            RenderSystem.NewRender(this);
        }

        public override void Disassemble()
        {
            base.Disassemble();
            sprite = null;
        }
    }
}