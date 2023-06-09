using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/************ Sorting Layers ************
 * 0: Default (Floor)                   *
 * 1: Floor Decal (e.g. liquids)        *
 * 2: Wall                              *
 * 3: Wall Decal (e.g torches)          *
 * 4: Object (e.g. items)               *
 * 5. Creature (& items on back at -1)  *
 * 6. Creature Decal (e.g. armor)       *
 * 7. Overlay (e.g. gas, VFX)           *
 * 8. UI                                *
 ****************************************/
namespace JS.ECS
{
    public class RenderSystem : SystemBase<Render>
    {
        //So another option rather than having single and compound renderers is to just render each thing separateley
        //this would of course also mean tracking them all separately, and I'm not sure how that would work for 
        //the bob/idle sways, as well as the other anims when I add those in... I could still parent them... w/ reference to parent

        private static RenderSystem instance;

        //Later these should be changed to pools
        [SerializeField] private SingleRenderer single;
        [SerializeField] private CompoundRenderer compound;

        [Space]

        [SerializeField] private Material flashMaterial;
        
        private Dictionary<Render, SingleRenderer> activeSingleRenders;
        private Dictionary<Render, CompoundRenderer> activeCompoundRenders;

        private bool skipAnimations = false; //When set to true, all animations end and objects are set to their end positions

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;
            activeSingleRenders = new Dictionary<Render, SingleRenderer>();
            activeCompoundRenders = new Dictionary<Render, CompoundRenderer>();
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
        private bool EntityOnCurrentMap(Render render)
        {
            if (GridManager.WorldMapActive) return render.RenderOnWorldMap;
            if (render.Transform.WorldPosition != GridManager.ActiveGrid.World) return false;
            return render.Transform.RegionPosition == GridManager.ActiveGrid.Region;
        }

        /// <summary>
        /// Registers the render and begins rendering if on currently active map.
        /// </summary>
        public static void NewRender(Render render)
        {
            Register(render);
            if (render.Layer == "Floor") render.Layer = "Default";
            UpdatePosition(render);
        }

        /// <summary>
        /// Updates the position of the object's render when its transform changes.
        /// </summary>
        /// <param name="render"></param>
        public static void UpdatePosition(Render render, bool smooth = false)
        {
            instance.UpdateRenderPosition(render, smooth);
        }

        private void UpdateRenderPosition(Render render, bool smooth = false)
        {
            if (!EntityOnCurrentMap(render))
            {
                StopRendering(render);
                return;
            }

            if (render.IsComposite)
            {
                if (!activeCompoundRenders.ContainsKey(render) || activeCompoundRenders[render] == null) StartRendering(render);

                if (GridManager.WorldMapActive)
                {
                    if (smooth) StartCoroutine(SlideWorld(render));
                    else activeCompoundRenders[render].transform.position = render.Transform.WorldPosition;
                }
                else
                {
                    if (smooth) StartCoroutine(SlideLocal(render));
                    else activeCompoundRenders[render].transform.position = (Vector3Int)render.Transform.LocalPosition;
                }
            }
            else
            {
                if (!activeSingleRenders.ContainsKey(render) || activeSingleRenders[render] == null) StartRendering(render);

                if (GridManager.WorldMapActive)
                {
                    if (smooth) StartCoroutine(SlideWorld(render));
                    else activeSingleRenders[render].transform.position = render.Transform.WorldPosition;
                }
                else
                {
                    if (smooth) StartCoroutine(SlideLocal(render));
                    else activeSingleRenders[render].transform.position = (Vector3Int)render.Transform.LocalPosition;
                }
            }
        }

        /// <summary>
        /// Begins rendering the object when it is within the current scene.
        /// </summary>
        private void StartRendering(Render render)
        {
            if (render.IsComposite)
            {
                var go = Instantiate(instance.compound);
                instance.activeCompoundRenders[render] = go;
                go.transform.position = (Vector3Int)render.Transform.LocalPosition;

                go.Base.sprite = GetSprite(render.Tile);

                /*for (int i = 0; i < compound.sprites.Length; i++)
                {
                    go.Renderers[i].sprite = compound.sprites[i];
                }*/
            }
            else
            {
                var go = Instantiate(instance.single);
                instance.activeSingleRenders[render] = go;
                go.transform.position = (Vector3Int)render.Transform.LocalPosition;

                go.Renderer.sprite = GetSprite(render.Tile);
                go.Renderer.sortingLayerName = render.Layer;
                go.Renderer.sortingOrder = render.Order;
            }
        }
        
        /// <summary>
        /// Stops rendering the object when it is no longer in the current scene.
        /// </summary>
        private void StopRendering(Render render)
        {
            if (render.IsComposite && activeCompoundRenders.ContainsKey(render))
            {
                if (activeCompoundRenders[render] != null)
                    Destroy(activeCompoundRenders[render].gameObject);

                activeCompoundRenders.Remove(render);
            }
            else if (activeSingleRenders.ContainsKey(render))
            {
                if (activeSingleRenders[render] != null)
                    Destroy(activeSingleRenders[render].gameObject);

                activeSingleRenders.Remove(render);
            }
        }

        /// <summary>
        /// Removes component from list and stops rendering.
        /// </summary>
        public static void RemoveRender(Render render)
        {
            instance.RemoveRenderComponent(render);
        }

        private void RemoveRenderComponent(Render render)
        {
            StopRendering(render);
            Unregister(render);
        }

        private Sprite GetSprite(string name)
        {
            var path = name.Split(",");
            var sprites = Resources.LoadAll<Sprite>("Sprites/" + path[0]);
            foreach (var sprite in sprites)
            {
                if (!sprite.name.Equals(path[1])) continue;
                return sprite;
            }
            return null;
        }

        public static GameObject GetGameObject(Entity entity)
        {
            if (entity == null) return null;

            foreach(var pair in instance.activeCompoundRenders)
            {
                if (pair.Key.entity == entity) return pair.Value.gameObject;
            }
            foreach (var pair in instance.activeSingleRenders)
            {
                if (pair.Key.entity == entity) return pair.Value.gameObject;
            }

            return null;
        }
        #region - Animations -
        private IEnumerator SlideLocal(Render render)
        {
            UnityEngine.Transform transform;

            if (activeSingleRenders.ContainsKey(render)) 
                transform = activeSingleRenders[render].transform;
            else if (activeCompoundRenders.ContainsKey(render)) 
                transform = activeCompoundRenders[render].transform;
            else yield break;

            var start = transform.position;
            var end = (Vector3Int)render.Transform.LocalPosition;
            float t = 0f;
            while (t < TimeSystem.AnimationTime && !skipAnimations)
            {
                transform.position = Vector3.Lerp(start, end, t / TimeSystem.AnimationTime);
                t += Time.deltaTime;
                yield return null;
            }
            transform.position = end;
        }

        private IEnumerator SlideWorld(Render render)
        {
            UnityEngine.Transform transform;

            if (activeSingleRenders.ContainsKey(render))
                transform = activeSingleRenders[render].transform;
            else if (activeCompoundRenders.ContainsKey(render))
                transform = activeCompoundRenders[render].transform;
            else yield break;

            var start = transform.position;
            var end = render.Transform.WorldPosition;
            float t = 0f;
            while (t < TimeSystem.AnimationTime && !skipAnimations)
            {
                transform.position = Vector3.Lerp(start, end, t / TimeSystem.AnimationTime);
                t += Time.deltaTime;
                yield return null;
            }
            transform.position = end;
        }

        private IEnumerator Jab(Render render, Vector3 direction)
        {
            UnityEngine.Transform transform;

            if (activeSingleRenders.ContainsKey(render))
                transform = activeSingleRenders[render].transform;
            else if (activeCompoundRenders.ContainsKey(render))
                transform = activeCompoundRenders[render].transform;
            else yield break;

            var start = transform.position;
            var mid = start + (direction * 0.5f);
            float t = 0f;

            while (t < TimeSystem.AnimationTime && !skipAnimations)
            {
                if (t <  TimeSystem.AnimationTime / 2)
                {
                    transform.position = Vector3.Lerp(start, mid, t / TimeSystem.AnimationTime);
                }
                else
                {
                    transform.position = Vector3.Lerp(mid, start, t / TimeSystem.AnimationTime);
                }

                t += Time.deltaTime;
                yield return null;
            }
            transform.position = start;
        }

        public static void SkipAnimations()
        {
            instance.skipAnimations = true;
            instance.Invoke("ResetAnimations", 0.01f);
        }

        private void ResetAnimations()
        {
            skipAnimations = false;
        }
        #endregion

        #region - Sprite Flash -
        /// <summary>
        /// Momentarily flashes the renderer white.
        /// </summary>
        public static void FlashSprite(Render render)
        {
            instance.Flash(render);

            /*if (render is RenderSingle single)
                instance.StartCoroutine(instance.Flash(single));
            else if (render is RenderCompound compound)
                instance.StartCoroutine(instance.Flash(compound));*/
        }

        private void Flash(Render render)
        {
            if (render.IsComposite) StartCoroutine(FlashComposite(activeCompoundRenders[render]));
            else StartCoroutine(FlashSingle(activeSingleRenders[render]));
        }

        private IEnumerator FlashSingle(SingleRenderer single)
        {
            var mat = single.Renderer.material;
            single.Renderer.material = flashMaterial;
            yield return new WaitForSeconds(0.1f);

            single.Renderer.material = mat;
        }

        private IEnumerator FlashComposite(CompoundRenderer compound)
        {
            var mats = new Material[10];
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = compound.Renderers[i].material;
                compound.Renderers[i].material = flashMaterial;
            }
            yield return new WaitForSeconds(0.1f);

            for (int i = 0; i < mats.Length; i++)
            {
                compound.Renderers[i].material = mats[i];
            }
        }

        /*private IEnumerator Flash(RenderSingle render)
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
        }*/
        #endregion
    }
}