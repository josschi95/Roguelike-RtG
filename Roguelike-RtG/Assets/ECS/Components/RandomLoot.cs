namespace JS.ECS
{
    /// <summary>
    /// Component that triggers a chance to drop random loot on death.
    /// </summary>
    public class RandomLoot : ComponentBase
    {
        public RandomLoot() { }

        public override void OnEvent(Event newEvent)
        {
            if (newEvent is Death) LootSystem.OnRandomLoot(entity);
        }
    }
}