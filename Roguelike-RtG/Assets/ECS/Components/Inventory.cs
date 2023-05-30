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

        public override void FireEvent(Event newEvent)
        {
            //
        }
    }
}

