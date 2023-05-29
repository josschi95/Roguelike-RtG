using System.Collections;
using UnityEngine;

namespace JS.ECS
{
    public class RenderSystem : SystemBase<RenderBase>
    {
        private static RenderSystem instance;

        [SerializeField] private SpriteRenderer single;
        [SerializeField] private CompoundRenderer compound;

        [Space]

        [SerializeField] private Material flashMaterial;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;
        }

        public static void NewSingle(RenderSingle render)
        {
            Register(render);

            var go = Instantiate(instance.single);
            go.sprite = render.sprite;
            go.transform.position = render.transform.LocalPosition;
            render.renderer = go;

            render.transform.onTransformChanged += delegate
            {
                UpdatePosition(render);
            };
        }

        public static void NewCompound(RenderCompound render)
        {
            Register(render);
            var go = Instantiate(instance.compound);

            go.transform.position = render.transform.LocalPosition;
            render.renderer = go;

            for (int i = 0; i < render.sprites.Length; i++)
            {
                go.Renderers[i].sprite = render.sprites[i];
            }


            render.transform.onTransformChanged += delegate
            {
                UpdatePosition(render);
            };
        }

        //Update Position on change
        private static void UpdatePosition(RenderSingle single)
        {
            single.renderer.transform.position = single.transform.LocalPosition;
        }

        private static void UpdatePosition(RenderCompound compound)
        {
            compound.renderer.transform.position = compound.transform.LocalPosition;
        }

        //Flash Sprites on damage
        public static void FlashSprite(RenderBase render)
        {
            if (render is RenderSingle single)
                instance.StartCoroutine(instance.Flash(single));
            else if (render is RenderCompound compound)
                instance.StartCoroutine(instance.Flash(compound));
        }

        private IEnumerator Flash(RenderSingle render)
        {
            var mat = render.renderer.material;
            render.renderer.material = flashMaterial;
            yield return new WaitForSeconds(0.1f);

            render.renderer.material = mat;
        }

        private IEnumerator Flash(RenderCompound compound)
        {
            var mats = new Material[10];
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = compound.renderer.Renderers[i].material;
                compound.renderer.Renderers[i].material = flashMaterial;
            }
            yield return new WaitForSeconds(0.1f);

            for (int i = 0; i < mats.Length; i++)
            {
                compound.renderer.Renderers[i].material = mats[i];
            }
        }

        //Remove renderers
        public static void RemoveSinge(RenderSingle single)
        {
            single.transform.onTransformChanged -= delegate
            {
                UpdatePosition(single);
            };
            Destroy(single.renderer);
            Unregister(single);
        }

        public static void RemoveCompound(RenderCompound compound)
        {
            compound.transform.onTransformChanged -= delegate
            {
                UpdatePosition(compound);
            };
            Destroy(compound.renderer);
            Unregister(compound);
        }
    }
}