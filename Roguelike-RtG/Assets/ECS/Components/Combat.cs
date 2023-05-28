namespace JS.ECS
{
    /// <summary>
    /// Component that allows an object to declare attacks against others
    /// </summary>
    public class Combat : ComponentBase
    {
        public Combat(Entity entity)
        {
            entity.AddComponent(this);
        }


    }
}