using JS.ECS.Tags;
using System.Collections.Generic;

namespace JS.ECS
{
    /// <summary>
    /// Component that allows an object to hold other objects
    /// </summary>
    public class Inventory : ComponentBase
    {
        private Transform _transform;
        public Transform Transform
        {
            get
            {
                if (_transform == null)
                {
                    _transform = EntityManager.GetComponent<Transform>(entity);
                }
                return _transform;
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
            var objects = TransformSystem.GetTakeablesAt(Transform.Position, Transform.LocalPosition);
        }
    }
}

