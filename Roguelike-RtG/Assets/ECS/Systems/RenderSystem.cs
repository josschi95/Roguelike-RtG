using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JS.ECS
{
    public class RenderSystem : SystemBase<RenderBase>
    {
        private static RenderSystem instance;

        //Later these should be changed to pools
        [SerializeField] private SingleRenderer single;
        [SerializeField] private CompoundRenderer compound;

        [Space]

        [SerializeField] private Material flashMaterial;
        
        private Dictionary<RenderBase, SingleRenderer> activeSingleRenders;
        private Dictionary<RenderBase, CompoundRenderer> activeCompoundRenders;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;
            activeSingleRenders = new Dictionary<RenderBase, SingleRenderer>();
            activeCompoundRenders = new Dictionary<RenderBase, CompoundRenderer>();
        }

        /// <summary>
        /// Stops rendering objects in previous scene and begins rendering objects in newly loaded scene
        /// </summary>
        public static void OnNewSceneLoaded()
        {
            for (int i = 0; i < components.Count; i++)
            {
                instance.UpdateRenderPosition(components[i]);
            }
        }

        /// <summary>
        /// Returns true if the entity is on the currently active map.
        /// </summary>
        private bool EntityOnCurrentMap(RenderBase render)
        {
            if (GridManager.WorldMapActive) return render.RenderOnWorldMap;
            return render.Physics.Position == GridManager.ActiveGrid.Position;
        }

        /// <summary>
        /// Registers the render and begins rendering if on currently active map.
        /// </summary>
        public static void NewRender(RenderBase render)
        {
            Register(render);
            UpdatePosition(render);
        }

        /// <summary>
        /// Updates the position of the object's render when its transform changes.
        /// </summary>
        /// <param name="render"></param>
        public static void UpdatePosition(RenderBase render)
        {
            instance.UpdateRenderPosition(render);
        }

        private void UpdateRenderPosition(RenderBase render)
        {
            if (!EntityOnCurrentMap(render))
            {
                StopRendering(render);
                return;
            }

            if (render is RenderSingle single)
            {
                if (!activeSingleRenders.ContainsKey(single) || activeSingleRenders[single] == null) StartRendering(single);

                if (GridManager.WorldMapActive) activeSingleRenders[single].transform.position = single.Physics.WorldMapPosition;
                else activeSingleRenders[single].transform.position = (Vector3Int)single.Physics.LocalPosition;
            }
            else if (render is RenderCompound compound)
            {
                if (!activeCompoundRenders.ContainsKey(compound) || activeCompoundRenders[compound] == null) StartRendering(compound);

                if (GridManager.WorldMapActive) activeCompoundRenders[compound].transform.position = compound.Physics.WorldMapPosition;
                else activeCompoundRenders[compound].transform.position = (Vector3Int)compound.Physics.LocalPosition;
            }
        }

        /// <summary>
        /// Momentarily flashes the renderer white.
        /// </summary>
        public static void FlashSprite(RenderBase render)
        {
            if (render is RenderSingle single)
                instance.StartCoroutine(instance.Flash(single));
            else if (render is RenderCompound compound)
                instance.StartCoroutine(instance.Flash(compound));
        }

        private IEnumerator Flash(RenderSingle render)
        {
            var mat = instance.activeSingleRenders[render].Renderer.material;
            instance.activeSingleRenders[render].Renderer.material = flashMaterial;
            yield return new WaitForSeconds(0.1f);

            instance.activeSingleRenders[render].Renderer.material = mat;
        }
        private IEnumerator Flash(RenderCompound compound)
        {
            var mats = new Material[10];
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = instance.activeCompoundRenders[compound].Renderers[i].material;
                instance.activeCompoundRenders[compound].Renderers[i].material = flashMaterial;
            }
            yield return new WaitForSeconds(0.1f);

            for (int i = 0; i < mats.Length; i++)
            {
                instance.activeCompoundRenders[compound].Renderers[i].material = mats[i];
            }
        }

        /// <summary>
        /// Begins rendering the object when it is within the current scene.
        /// </summary>
        private void StartRendering(RenderBase render)
        {
            if (render is RenderSingle single)
            {
                var go = Instantiate(instance.single);
                go.Renderer.sprite = single.sprite;
                go.transform.position = (Vector3Int)single.Physics.LocalPosition;
                instance.activeSingleRenders[single] = go;
            }
            else if (render is RenderCompound compound)
            {
                var go = Instantiate(instance.compound);
                go.transform.position = (Vector3Int)compound.Physics.LocalPosition;
                for (int i = 0; i < compound.sprites.Length; i++)
                {
                    go.Renderers[i].sprite = compound.sprites[i];
                }
                instance.activeCompoundRenders[compound] = go;
            }
        }
        
        /// <summary>
        /// Stops rendering the object when it is no longer in the current scene.
        /// </summary>
        private void StopRendering(RenderBase render)
        {
            if (render is RenderSingle single && activeSingleRenders.ContainsKey(single))
            {
                if (activeSingleRenders[single] != null)
                    Destroy(activeSingleRenders[single].gameObject);

                activeSingleRenders.Remove(single);
            }
            else if (render is RenderCompound compound && activeCompoundRenders.ContainsKey(compound))
            {
                if (activeCompoundRenders[compound] != null)
                    Destroy(activeCompoundRenders[compound].gameObject);

                activeCompoundRenders.Remove(compound);
            }
        }

        /// <summary>
        /// Removes component from list and stops rendering.
        /// </summary>
        public static void RemoveRender(RenderBase render)
        {
            instance.RemoveRenderComponent(render);
        }

        private void RemoveRenderComponent(RenderBase render)
        {
            StopRendering(render);
            Unregister(render);
        }
    }
}