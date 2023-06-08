namespace JS.ECS
{
    /// <summary>
    /// Reduces all incoming damage to the entity to 0.
    /// </summary>
    public class NoDamage : ComponentBase
    {
        public NoDamage() 
        {
            Priority = 0; //top priority
        }

        public override void OnEvent(Event newEvent)
        {
            if (newEvent is TakeDamage dmg)
            {
                dmg.Damage.Clear();
            }
        }
    }
}