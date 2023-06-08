namespace JS.ECS
{
    /// <summary>
    /// Component which adds the Flaming Property to an object and adds fire damage to attacks
    /// </summary>
    public class Flaming : ComponentBase
    {
        public Flaming(Physics physics)
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
            var result = Dice.Roll(6);
            if (damage.Damage.ContainsKey("Fire"))
            {
                damage.Damage["Fire"] += result;
            }
            else damage.Damage.Add("Fire", result);
        }
    }
}