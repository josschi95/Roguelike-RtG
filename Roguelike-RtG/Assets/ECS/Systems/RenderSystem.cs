using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
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
        private static RenderSystem instance;

        //These should be made into pools later on
        [SerializeField] private SpriteRenderer staticSprite;
        [SerializeField] private Animator animatedSprite;

        [Space]

        [SerializeField] private Material flashMaterial;
        
        private Dictionary<Render, SpriteRenderer> activeSpriteRenderers;

        //When set to true, all sliding animations end and objects are set to their end positions
        private bool skipAnimations = false; 

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;

            activeSpriteRenderers = new Dictionary<Render, SpriteRenderer>();
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

            if (!activeSpriteRenderers.ContainsKey(render) || activeSpriteRenderers[render] == null) StartRendering(render);


            if(GridManager.WorldMapActive)
            {
                //Moved Right
                if(render.Transform.WorldPosition.x > activeSpriteRenderers[render].transform.position.x)
                {
                    activeSpriteRenderers[render].flipX = true; //all sprites face left by default
                }
                //Moved Left
                else if (render.Transform.WorldPosition.x < activeSpriteRenderers[render].transform.position.x)
                {
                    activeSpriteRenderers[render].flipX = false;
                }

                if (smooth) StartCoroutine(SlideWorld(render));
                else activeSpriteRenderers[render].transform.position = render.Transform.WorldPosition;
            }
            else
            {
                //Moved Right
                if (render.Transform.LocalPosition.x > activeSpriteRenderers[render].transform.position.x)
                {
                    activeSpriteRenderers[render].flipX = true;
                }
                //Moved Left
                else if (render.Transform.LocalPosition.x < activeSpriteRenderers[render].transform.position.x)
                {
                    activeSpriteRenderers[render].flipX = false;
                }

                if (smooth) StartCoroutine(SlideLocal(render));
                else activeSpriteRenderers[render].transform.position = (Vector3Int)render.Transform.LocalPosition;
            }


            /*if (render.IsAnimated)
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
            }*/
        }

        /// <summary>
        /// Begins rendering the object when it is within the current scene.
        /// </summary>
        private void StartRendering(Render render)
        {
            if (render.IsAnimated)
            {
                var go = Instantiate(instance.animatedSprite);

                instance.activeSpriteRenderers[render] = go.GetComponent<SpriteRenderer>();

                go.transform.position = (Vector3Int)render.Transform.LocalPosition;
                go.runtimeAnimatorController = GetAnimator(render.Tile);
            }
            else
            {
                var go = Instantiate(instance.staticSprite);

                instance.activeSpriteRenderers[render] = go;
                go.transform.position = (Vector3Int)render.Transform.LocalPosition;

                go.sprite = GetSprite(render.Tile);
                go.sortingLayerName = render.Layer;
                go.sortingOrder = render.Order;
            }
        }
        
        /// <summary>
        /// Stops rendering the object when it is no longer in the current scene.
        /// </summary>
        private void StopRendering(Render render)
        {
            if (activeSpriteRenderers.ContainsKey(render))
            {
                if (activeSpriteRenderers[render] != null)
                    Destroy(activeSpriteRenderers[render].gameObject);

                activeSpriteRenderers.Remove(render);
            }

            /*if (render.IsAnimated && activeCompoundRenders.ContainsKey(render))
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
            }*/
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
            var sprites = Resources.LoadAll<Sprite>($"Sprites/{path[0]}");
            foreach (var sprite in sprites)
            {
                if (!sprite.name.Equals(path[1])) continue;
                return sprite;
            }
            return null;
        }

        private AnimatorController GetAnimator(string name)
        {
            return Resources.Load<AnimatorController>($"Sprites/Animations/{name}");
        }

        public static GameObject GetGameObject(Entity entity)
        {
            if (entity == null) return null;

            foreach (var pair in instance.activeSpriteRenderers)
            {
                if (pair.Key.entity == entity) return pair.Value.gameObject;
            }

            return null;
        }
        #region - Animations -
        private IEnumerator SlideLocal(Render render)
        {
            if (!activeSpriteRenderers.ContainsKey(render)) yield break;

            UnityEngine.Transform transform = activeSpriteRenderers[render].transform;

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
            if (!activeSpriteRenderers.ContainsKey(render)) yield break;

            UnityEngine.Transform transform = activeSpriteRenderers[render].transform;

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
            if (!activeSpriteRenderers.ContainsKey(render)) yield break;

            UnityEngine.Transform transform = activeSpriteRenderers[render].transform;

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
        }

        private void Flash(Render render)
        {
            StartCoroutine(FlashSingle(activeSpriteRenderers[render]));
        }

        private IEnumerator FlashSingle(SpriteRenderer sprite)
        {
            var mat = sprite.material;
            sprite.material = flashMaterial;
            yield return new WaitForSeconds(0.1f);

            sprite.material = mat;
        }
        #endregion
    }
}