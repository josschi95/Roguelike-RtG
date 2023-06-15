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

        private bool CanTakeItem(Physics newObject)
        {
            if (!newObject.IsTakeable) return false;
            if (!EntityManager.TryGetComponent<Transform>(newObject.entity, out var transform)) return false;

            return true;
        }
    }
}

