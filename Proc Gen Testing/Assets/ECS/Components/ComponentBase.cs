namespace JS.ECS
{
    /// <summary>
    /// A base class for all Components which will be attached to an Entity
    /// </summary>
    public class ComponentBase
    {
        public Entity entity;

        public virtual void Update()
        {
            //Meant to be overwritten
        }

        public virtual void Release()
        {
            entity = null;
            //System.Unregister(this);
        }
    }
}