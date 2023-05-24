using System.Collections.Generic;
using UnityEngine;

namespace JS.ECS
{
    public abstract class SystemBase<T> : MonoBehaviour where T : ComponentBase
    {
        protected static List<T> components = new List<T>();

        public static void Register(T component)
        {
            components.Add(component);
        }

        public static void Update()
        {
            foreach (var component in components)
            {
                component.Update();
            }
        }

        public static void Unregister(T component)
        {
            components.Remove(component);
        }
    }
}