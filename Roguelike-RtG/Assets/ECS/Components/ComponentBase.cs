namespace JS.ECS
{
    /// <summary>
    /// A base class for all Components which will be attached to an Entity
    /// </summary>
    public class ComponentBase
    {
        public Entity entity;
        public int Priority = int.MaxValue;

        /*public virtual void Update()
        {
            //Meant to be overwritten
        }*/

        public virtual void FireEvent(Event newEvent)
        {
            //Meant to be overwritten
            //Each Component will take in the Event ID and decide if/how to handle it
            //Should this be changed to a bool? return true if it was modified?
        }

        public virtual void Disassemble()
        {
            entity = null;
            //System.Unregister(this);
        }
    }
}