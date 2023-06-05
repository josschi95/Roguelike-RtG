using JS.ECS.Tags;
using System.Collections.Generic;

namespace JS.ECS
{
    /// <summary>
    /// Component that allows an object to hold other objects
    /// </summary>
    public class Inventory : ComponentBase
    {
        public List<Entity> Contents;

        public Inventory()
        {
            Contents = new List<Entity>();
        }

        public bool AddObject(Physics newObject)
        {
            if (!CanAddItem(newObject)) return false;

            if (newObject.entity.TryGetComponent<ObjectStack>(out var stack))
            {
                //Can 
            }
            else
            {
                Contents.Add(newObject.entity);
            }
            
            return true;
        }

        private bool CanAddItem(Physics newObject)
        {
            if (newObject.IsTakeable) return false;
            if (!newObject.entity.GetTag<Item>()) return false;

            return true;
        }

        public override void OnEvent(Event newEvent)
        {
            if (newEvent is TransformChanged) CheckItemsAtPosition();
        }

        private void CheckItemsAtPosition()
        {
            var t = entity.GetComponent<Transform>();
            if (t.Depth == 1) return;

            var objects = TransformSystem.GetLocalEntitiesAt(t, t.LocalPosition);

            for (int i = 0; i < objects.Length; i++)
            {
                var phys = objects[i].GetComponent<Physics>();
                if (phys != null && phys.IsTakeable)
                {
                    UnityEngine.Debug.Log("Found " + objects[i].Name);
                }
            }
        }
    }
}

