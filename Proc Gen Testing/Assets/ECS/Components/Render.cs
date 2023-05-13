using UnityEngine;

namespace JS.ECS
{
    public class Render : ComponentBase
    {
        public Transform transform;
        public SpriteRenderer renderer;

        public Render(Transform transform, Sprite sprite)
        {
            this.transform = transform;
            var go = new GameObject("sprite");
            go.transform.position = transform.position;

            renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
        }
    }
}