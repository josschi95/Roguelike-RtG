namespace JS.ECS
{
    /// <summary>
    /// Component which adds the Frost Property to an object and adds frost damage to attacks
    /// </summary>
    public class Frost : ComponentBase
    {
        public Frost()
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
            if (damage.Damage.ContainsKey("Frost"))
            {
                damage.Damage["Frost"] += roll;
            }
            else damage.Damage.Add("Frost", roll);
        }
    }
}