namespace JS.ECS
{
    /// <summary>
    /// Reduces all incoming damage to the entity to 0.
    /// </summary>
    public class NoDamage : ComponentBase
    {
        public NoDamage() { }

        public override void OnEvent(Event newEvent)
        {
            if (newEvent is TakeDamage dmg)
            {
                for (int i = 0; i < dmg.Amounts.Count; i++)
                {
                    dmg.Amounts[i] = 0;
                }
            }
        }
    }
}