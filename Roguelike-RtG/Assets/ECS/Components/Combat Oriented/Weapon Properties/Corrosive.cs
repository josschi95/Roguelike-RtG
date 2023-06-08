namespace JS.ECS
{
    /// <summary>
    /// Component which adds the Corrosive Property to an object and adds acid damage to attacks
    /// </summary>
    public class Corrosive : ComponentBase
    {
        public Corrosive()
        {
            Priority = 2;
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
            if (damage.Damage.ContainsKey("Acid"))
            {
                damage.Damage["Acid"] += roll;
            }
            else damage.Damage.Add("Acid", roll);
        }
    }
}