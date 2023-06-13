using JS.ECS.Tags;
using System.Collections.Generic;

namespace JS.ECS
{
    /// <summary>
    /// Component that allows an object to hold other objects
    /// </summary>
    public class Inventory : ComponentBase
    {
        private Physics _physics;

        public Physics Physics
        {
            get
            {
                if (_physics == null)
                {
                    _physics = EntityManager.GetComponent<Physics>(entity);
                }
                return _physics;
            }
        }

        public List<Entity> Contents;

        public Inventory()
        {
            Contents = new List<Entity>();
        }

        public bool AddObject(Physics newObject)
        {
            if (!CanAddItem(newObject)) return false;

            if (EntityManager.TryGetComponent<ObjectStack>(newObject.entity, out var stack))
            {
                //Check if can stack... but this needs to go inside an InventorySystem
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
            if (!EntityManager.GetTag<Item>(newObject.entity)) return false;

            return true;
        }

        public override void OnEvent(Event newEvent)
        {
            if (newEvent is TransformChanged) CheckItemsAtPosition();
        }

        private void CheckItemsAtPosition()
        {
            if (GridManager.WorldMapActive) return;
            //UnityEngine.Debug.Log("From: " + entity.Name + " : " + Physics.Position);
            var objects = TransformSystem.GetEntitiesAt(Physics.Position, Physics.LocalPosition);

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].IsTakeable)
                {
                    UnityEngine.Debug.Log(entity.Name + " found " + objects[i].entity.Name);
                }
            }
        }
    }
}

