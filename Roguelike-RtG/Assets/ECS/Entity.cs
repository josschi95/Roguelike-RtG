using System;
using System.Collections.Generic;

namespace JS.ECS
{
    public class Entity
    {
        //this might later be changed to a string
        public Guid ID { get; private set; }
        private List<ComponentBase> components;

        public Entity()
        {
            ID = Guid.NewGuid();
            components = new List<ComponentBase>();
        }

        
        //Adds the referenced component to this entity and sets this as its entity
        public void AddComponent(ComponentBase component)
        {
            component.entity = this;
            for (int i = 0; i < components.Count; i++)
            {
                if (component.Priority > components[i].Priority)
                {
                    components.Insert(i, component);
                    return;
                }
            }
            components.Add(component);
        }

        //Removes the referenced component and tells it to Unregister itself
        public void RemoveComponent(ComponentBase component)
        {
            if (!components.Contains(component)) return;

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

        public void Destroy()
        {
            for (int i = components.Count - 1; i >= 0; i--)
            {
                RemoveComponent(components[i]);
            }
            //Will also need to unregister this likely
        }
        
        public void SendEvent(Event newEvent)
        {
            for (int i = 0; i < components.Count; i++)
            {
                components[i].OnEvent(newEvent);
            }
        }
    }
}