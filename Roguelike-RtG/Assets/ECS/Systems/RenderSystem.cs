using UnityEngine;

namespace JS.ECS
{
    public class RenderSystem : SystemBase<Render>
    {
        [SerializeField] private static SpriteRenderer single;
        [SerializeField] private SpriteRenderer compound;

        public static void NewRender(Render render)
        {
            if (render.single)
            {
                var go = Instantiate(single);
            }
        }
    }
}