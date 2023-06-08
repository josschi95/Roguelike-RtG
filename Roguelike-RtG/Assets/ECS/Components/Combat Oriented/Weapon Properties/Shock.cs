namespace JS.ECS
{
    /// <summary>
    /// Component which adds the Shock Property to an object and adds lightning damage to attacks
    /// </summary>
    public class Shock : ComponentBase
    {
        public Shock(Physics physics)
        {
            Priority = 2;
            physics.entity.AddComponent(this);
        }

        public override void OnEvent(Event newEvent)
        {
            if (newEvent is DealingMeleeDamage damage)
            {
                AddDamage(damage);
            }
        }

        private void AddDamage(DealingMeleeDamage damage)
        {
            var roll = Dice.Roll(6);
            if (damage.Damage.ContainsKey("Lightning"))
            {
                damage.Damage["Lightning"] += roll;
            }
            else damage.Damage.Add("Lightning", roll);
        }
    }
}