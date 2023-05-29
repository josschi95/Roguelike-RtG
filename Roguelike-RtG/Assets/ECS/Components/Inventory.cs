using System.Collections.Generic;

namespace JS.ECS
{
    /// <summary>
    /// Component that allows an object to hold other objects
    /// </summary>
    public class Inventory : ComponentBase
    {
        public List<Physics> Contents;

        public Inventory()
        {
            Contents = new List<Physics>();
        }

        public bool AddObject(Physics newObject)
        {
            if (newObject.IsTakeable) return false;


            return true;
        }

        public override void FireEvent(Event newEvent)
        {
            //
        }
    }
}

