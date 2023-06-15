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

        public List<Entity> Contents = new List<Entity>();
        public float TotalWeight;
    }
}

