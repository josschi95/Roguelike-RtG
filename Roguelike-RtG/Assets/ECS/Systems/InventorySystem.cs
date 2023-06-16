using JS.ECS.Tags;
using UnityEngine;

namespace JS.ECS
{
    public class InventorySystem : MonoBehaviour
    {
        private static InventorySystem instance;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;
        }

        public static bool TryAddItem(Inventory inventory, Entity newItem)
        {
            if (!instance.CanTakeItem(newItem)) return false;

            instance.AddToInventory(inventory, newItem);
            return true;
        }

        private bool CanTakeItem(Entity newObject)
        {
            if (!EntityManager.TryGetComponent<Transform>(newObject, out var transform)) return false;
            if (!EntityManager.TryGetComponent<Physics>(newObject, out var phys)) return false;
            if (!phys.IsTakeable) return false;

            return true;
        }

        private void AddToInventory(Inventory inventory, Entity newItem)
        {
            //So here I need to check if I can stack it with an existing item, which is going to be a challenge
            if (CanStackItem(inventory, newItem)) return;

            inventory.Contents.Add(newItem);

            OnItemAddedToInventory(newItem);

            //Debug.Log("Adding " + newItem.Name + " to inventory of " + inventory.entity.Name);
        }

        private bool CanStackItem(Inventory inventory, Entity newItem)
        {
            if (!EntityManager.TryGetComponent<ObjectStack>(newItem, out var newStack)) return false;

            for (int i = 0; i < inventory.Contents.Count; i++)
            {
                if (inventory.Contents[i].Name != newItem.Name) continue;
                if (!EntityManager.TryGetComponent<ObjectStack>(inventory.Contents[i], out var oldStack)) continue;
                if (ItemsCanStack(oldStack, newStack))
                {
                    AddToStack(inventory, oldStack, newStack);
                    return true;
                }
            }
            return false;
        }

        private bool ItemsCanStack(ObjectStack oldStack, ObjectStack newStack)
        {
            if(!oldStack.entity.Name.Equals(newStack.entity.Name)) return false; //Differently named
            if (EntityManager.GetTag<NeverStack>(oldStack.entity) || 
                EntityManager.GetTag<NeverStack>(newStack.entity)) return false; //Never allowed to stack

            EntityManager.TryGetStat(oldStack.entity, "HP", out var hpA);
            EntityManager.TryGetStat(newStack.entity, "HP", out var hpB);

            if (hpA != null && hpB != null && hpA.Value == hpB.Value) return true; //The items have the same HP
            //Down the line I may need to also check for Runes, but maybe items of those types shouldn't stack at all
            return false;
        }

        private void AddToStack(Inventory inventory, ObjectStack oldStack, ObjectStack newStack)
        {
            //Increase inventory weight by the added count
            inventory.TotalWeight += newStack.Count * EntityManager.GetComponent<Physics>(newStack.entity).Weight;
            Debug.Log("Adding " + newStack.entity.Name + " to inventory of " + inventory.entity.Name);

            //Can merge the two stacks completely
            if (oldStack.Count + newStack.Count <= oldStack.MaxStack)
            {
                oldStack.Count += newStack.Count;
                EntityManager.Destroy(newStack.entity);
                return;
            }

            int diff = oldStack.MaxStack - oldStack.Count;
            oldStack.Count += diff;
            newStack.Count -= diff;

            inventory.Contents.Add(newStack.entity);
            OnItemAddedToInventory(newStack.entity);
        }

        private void OnItemAddedToInventory(Entity entity)
        {
            //I'm also going to need to tell the RenderSystem....

            if (EntityManager.TryGetComponent<Transform>(entity, out var transform))
                EntityManager.RemoveComponent(entity, transform);

            EntityManager.FireEvent(entity, new AddedToInventory());
        }
    }
}

