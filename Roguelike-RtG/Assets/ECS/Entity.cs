using System;
using System.Collections.Generic;
using JS.CharacterSystem;
using JS.ECS.Tags;

namespace JS.ECS
{
    public class Entity
    {
        public Guid ID { get; private set; }
        public string Name;
        public List<ComponentBase> components;
        private List<StatBase> _stats;
        private List<TagBase> _tags;

        public Entity(string name = "Entity")
        {
            ID = Guid.NewGuid();
            components = new List<ComponentBase>();
            _stats = new List<StatBase>();
            _tags = new List<TagBase>();
            Name = name;
        }

        public void FireEvent(Event newEvent)
        {
            for (int i = 0; i < components.Count; i++)
            {
                components[i].OnEvent(newEvent);
            }
        }

        public void Destroy()
        {
            for (int i = components.Count - 1; i >= 0; i--)
            {
                RemoveComponent(components[i]);
            }
            for (int i = 0; i < _stats.Count; i++)
            {
                RemoveStat(_stats[i]);
            }
            for (int i = 0; i < _tags.Count; i++)
            {
                RemoveTag(_tags[i]);
            }
            //Will also need to unregister this likely
        }

        #region - Components -
        //Adds the referenced component to this entity and sets this as its entity
        public void AddComponent(ComponentBase component)
        {
            component.entity = this;
            for (int i = 0; i < components.Count; i++)
            {
                if (component.Priority < components[i].Priority)
                {
                    components.Insert(i, component);
                    return;
                }
            }
            components.Add(component);
            component.OnRegistered();
        }

        public bool HasComponent(ComponentBase component, out ComponentBase comp)
        {
            comp = null;
            for (int i = 0; i < components.Count; i++)
            {
                if (component.GetType().Equals(components[i].GetType()))
                    return true;
            }
            return false;
        }

        //Removes the referenced component and tells it to Unregister itself
        public void RemoveComponent(ComponentBase component)
        {
            if (!components.Contains(component)) return;
            component.entity = null;
            components.Remove(component);
            component.Disassemble();
        }

        //Returns the requested component type
        public T GetComponent<T>() where T : ComponentBase
        {
            foreach (ComponentBase component in components)
            {
                if (component.GetType().Equals(typeof(T)))
                {
                    return (T)component;
                }
            }
            return null;
        }

        //Checks if the requested component exists and returns it with a value of true if true
        public bool TryGetComponent<T>(out ComponentBase component) where T : ComponentBase
        {
            component = null;
            foreach(ComponentBase comp in components)
            {
                if (comp.GetType().Equals(typeof(T)))
                {
                    component = (T)comp;
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region - Stats -
        public void AddStat(StatBase newStat)
        {
            _stats.Add(newStat);
        }

        public void RemoveStat(StatBase stat)
        {
            _stats.Remove(stat);
        }

        public bool TryGetStat(string name, out StatBase stat)
        {
            stat = null;
            foreach (StatBase s in _stats)
            {
                if (s.Name.Equals(name) || s.ShortName.Equals(name)) 
                { 
                    stat = (StatBase)s; 
                    return true; 
                }
            }
            return false;
        }
        #endregion

        #region - Tags -
        public void AddTag(TagBase tag)
        {
            if (_tags.Contains(tag)) return;
            _tags.Add(tag);
        }

        public void RemoveTag(TagBase tag)
        {
            _tags.Remove(tag);
        }

        public bool GetTag<T>() where T : TagBase
        {
            foreach (TagBase tag in _tags)
            {
                if (tag.GetType().Equals(typeof(T)))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}