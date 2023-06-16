using System.Collections.Generic;
using UnityEngine;
using JS.CharacterSystem;
using JS.ECS.Tags;

namespace JS.ECS
{
    public class EntityManager : MonoBehaviour
    {
        private static EntityManager instance;

        private class EntityPackage
        {
            public List<ComponentBase> components;
            public List<StatBase> stats;
            public List<TagBase> tags;

            public EntityPackage()
            {
                components = new List<ComponentBase>();
                stats = new List<StatBase>();
                tags = new List<TagBase>();
            }
        }

        private Dictionary<Entity, EntityPackage> entities;

        private Entity playerEntity;
        public static Entity Player
        {
            get
            {
                if (instance.playerEntity == null)
                {
                    foreach(var pair in instance.entities)
                    {
                        if (instance.GetEntityTag<PlayerTag>(pair.Key))
                        {
                            instance.playerEntity = pair.Key;
                            break;
                        }
                    }
                }
                return instance.playerEntity;
            }
        }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;
            entities = new Dictionary<Entity, EntityPackage>();
        }

        public static Entity NewEntity(string name = "Entity")
        {
            var newEntity = new Entity(name);
            instance.entities.Add(newEntity, new EntityPackage());
            return newEntity;
        }

        #region - Components -
        /// <summary>
        /// Adds new component to entity and registers it.
        /// </summary>
        public static ComponentBase AddComponent(Entity entity, ComponentBase component)
        {
            return instance.AddNewComponent(entity, component);
        }

        private ComponentBase AddNewComponent(Entity entity, ComponentBase component)
        {
            if (!entities.ContainsKey(entity)) return null;

            component.entity = entity;
            bool added = false;
            for (int i = 0; i < entities[entity].components.Count; i++)
            {
                if (component.Priority < entities[entity].components[i].Priority)
                {
                    entities[entity].components.Insert(i, component);
                    added = true;
                    break;
                }
            }
            if (!added) entities[entity].components.Add(component);

            component.OnRegistered();
            return component;
        }

        /// <summary>
        /// Removes component from entity and unregisters it.
        /// </summary>
        public static void RemoveComponent(Entity entity, ComponentBase component)
        {
            instance.RemoveOldComponent(entity, component);
        }

        private void RemoveOldComponent(Entity entity, ComponentBase component)
        {
            if (component.entity != entity) throw new UnityException("Component does not belong to entity!");
            if (!entities.ContainsKey(entity)) return;
            //Debug.Log("Removing " + component.GetType().Name);
            entities[entity].components.Remove(component);
            component.entity = null;
            component.Disassemble();
        }

        /// <summary>
        /// Returns the requested component if it is registered to the entity.
        /// </summary>
        public static T GetComponent<T>(Entity entity) where T : ComponentBase
        {
            return instance.GetEntityComponent<T>(entity);
        }

        private T GetEntityComponent<T>(Entity entity) where T : ComponentBase
        {
            if (!entities.ContainsKey(entity)) return null;

            foreach(ComponentBase component in entities[entity].components)
            {
                if (component.GetType().Equals(typeof(T))) return (T)component;
            }
            return null;
        }

        /// <summary>
        /// Returns true with the requested component if it is registered to the entity.
        /// </summary>
        public static bool TryGetComponent<T>(Entity entity, out T component) where T : ComponentBase
        {
            return instance.TryGetEntityComponent<T>(entity, out component);
        }

        private bool TryGetEntityComponent<T>(Entity entity, out T comp) where T : ComponentBase
        {
            comp = null;
            if (!entities.ContainsKey(entity)) return false;

            foreach (ComponentBase component in entities[entity].components)
            {
                if (component.GetType().Equals(typeof(T)))
                {
                    comp = (T)component;
                    return true;
                }
            }
            return false;
        }

        public static bool TryFindComponent(Entity entity, ComponentBase component, out ComponentBase comp)
        {
            return instance.TryFindExistingComponent(entity, component, out comp);
        }

        private bool TryFindExistingComponent(Entity entity, ComponentBase component, out ComponentBase comp)
        {
            comp = null;
            if (!entities.ContainsKey(entity)) return false;

            foreach (ComponentBase componentBase in entities[entity].components)
            {
                if (componentBase.GetType().Equals(component.GetType()))
                {
                    comp = componentBase;
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region - Stats -
        public static void AddStat(Entity entity, StatBase stat)
        {
            instance.AddNewStat(entity, stat);
        }

        private void AddNewStat(Entity entity, StatBase stat)
        {
            if (!entities.ContainsKey(entity)) return;

            entities[entity].stats.Add(stat);
        }

        public static void RemoveStat(Entity entity, StatBase stat)
        {
            instance.RemoveOldStat(entity, stat);
        }

        private void RemoveOldStat(Entity entity, StatBase stat)
        {
            if (!entities.ContainsKey(entity)) return;

            entities[entity].stats.Remove(stat);
        }

        public static bool TryGetStat(Entity entity, string statName, out StatBase stat)
        {
            return instance.TryGetEntityStat(entity, statName, out stat);
        }

        private bool TryGetEntityStat(Entity entity, string name, out StatBase stat)
        {
            stat = null;
            if (!entities.ContainsKey(entity)) return false;

            foreach(StatBase statBase in entities[entity].stats)
            {
                if (statBase.Name.Equals(name) || statBase.ShortName.Equals(name))
                {
                    stat = statBase;
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region - Tags -
        public static void AddTag(Entity entity, TagBase tag)
        {
            instance.AddNewTag(entity, tag);
        }

        private void AddNewTag(Entity entiy, TagBase tag)
        {
            if (!entities.ContainsKey(entiy)) return;

            entities[entiy].tags.Add(tag);
        }

        public static void RemoveTag(Entity entity, TagBase tag)
        {
            instance.RemoveOldTag(entity, tag);
        }

        private void RemoveOldTag(Entity entiy, TagBase tag)
        {
            if (!entities.ContainsKey(entiy)) return;

            entities[entiy].tags.Remove(tag);
        }

        public static bool GetTag<T>(Entity entity) where T : TagBase
        {
            return instance.GetEntityTag<T>(entity);
        }

        private bool GetEntityTag<T>(Entity entiy) where T : TagBase
        {
            if (!entities.ContainsKey(entiy)) return false;

            foreach(TagBase tag in entities[entiy].tags)
            {
                if (tag.GetType().Equals(typeof(T)))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TryGetTag<T>(Entity entity, out string value) where T : TagBase
        {
            return instance.TryGetEntityTag<T>(entity, out value);
        }

        private bool TryGetEntityTag<T>(Entity entity, out string value) where T : TagBase
        {
            value = null;
            if (!entities.ContainsKey(entity)) return false;

            foreach (TagBase tag in entities[entity].tags)
            {
                if (tag.GetType().Equals(typeof(T)))
                {
                    value = tag.Value;
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region - Events -
        public static void FireEvent(Entity entity, Event newEvent)
        {
            instance.FireNewEvent(entity, newEvent);
        }

        private void FireNewEvent(Entity entity, Event newEvent)
        {
            if (instance.entities.ContainsKey(entity))
            {
                for (int i = 0; i < entities[entity].components.Count; i++)
                {
                    //Debug.Log("Firing " + newEvent.GetType().Name + " to " + entities[entity].components[i].GetType().Name);    
                    entities[entity].components[i].OnEvent(newEvent);
                }
            }
        }
        #endregion

        #region - Entity Removal -
        public static void Destroy(Entity entity)
        {
            instance.DestroyEntity(entity);
        }

        private void DestroyEntity(Entity entity)
        {
            Debug.Log("Destroying " + entity.Name);
            var pair = entities[entity];

            for (int i = pair.components.Count - 1; i >= 0; i--)
            {
                RemoveOldComponent(entity, pair.components[i]);
            }
            pair.components.Clear();
            pair.components = null;

            for (int i = pair.stats.Count - 1; i >= 0; i--)
            {
                RemoveOldStat(entity, pair.stats[i]);
            }
            pair.stats.Clear();
            pair.stats = null;

            for (int i = pair.tags.Count - 1; i >= 0; i--)
            {
                RemoveOldTag(entity, pair.tags[i]);
            }
            pair.tags.Clear();
            pair.tags = null;

            //This needs to be last so that all other methods can reference it in the dictionary
            entities.Remove(entity);
        }
        #endregion
    }
}