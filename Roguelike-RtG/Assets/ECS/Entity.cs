using System;
using System.Collections.Generic;

namespace JS.ECS
{
    public class Entity
    {
        //this might later be changed to a string
        public Guid ID { get; private set; }
        //private List<ComponentBase> components;

        public Entity()
        {
            ID = Guid.NewGuid();
            //components = new List<ComponentBase>();
        }
        /*
        //Adds the referenced component to this entity and sets this as its entity
        public void AddComponent(ComponentBase component)
        {
            components.Add(component);
            component.entity = this;
        }

        //Removes the referenced component and tells it to Unregister itself
        public void RemoveComponent(ComponentBase component)
        {
            if (!components.Contains(component)) return;

            components.Remove(component);
            component.Release();
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

        public void Destroy()
        {
            for (int i = components.Count - 1; i >= 0; i--)
            {
                RemoveComponent(components[i]);
            }
            //Will also need to unregister this likely
        }
        */
    }
}