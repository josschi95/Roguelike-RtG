using UnityEngine;

namespace JS.ECS
{
    public class Render : ComponentBase
    {
        public Transform transform;
        public Sprite sprite;
        public bool single;

        public Render(Transform transform, Sprite sprite)
        {
            this.transform = transform;
            this.sprite = sprite;
            RenderSystem.Register(this);
            RenderSystem.NewRender(this);
        }
    }
}