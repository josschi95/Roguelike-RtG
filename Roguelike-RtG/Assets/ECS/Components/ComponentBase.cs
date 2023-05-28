namespace JS.ECS
{
    /// <summary>
    /// A base class for all Components which will be attached to an Entity
    /// </summary>
    public class ComponentBase
    {
        public Entity entity;
        public int Priority = int.MaxValue;

        public virtual void Update()
        {
            //Meant to be overwritten
        }

        public virtual void OnEvent(Event newEvent)
        {
            //Meant to be overwritten
        }

        public virtual void Disassemble()
        {
            entity = null;
            //System.Unregister(this);
        }
    }
}