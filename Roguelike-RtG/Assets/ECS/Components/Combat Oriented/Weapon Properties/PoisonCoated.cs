namespace JS.ECS
{
    /// <summary>
    /// Component which adds the PoisonCoated Property to an object and adds poison damage to attacks.
    /// Note that this is distinct from the Poisoned property which inflicts damage to the object it is applied to.
    /// </summary>
    public class PoisonCoated : ComponentBase
    {
        public string Amount;
        public PoisonCoated(string amount = "1d6")
        {
            Amount = amount;
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
            var roll = Dice.Roll(Amount);
            if (damage.Damage.ContainsKey("Poison"))
            {
                damage.Damage["Poison"] += roll;
            }
            else damage.Damage.Add("Poison", roll);
        }
    }
}

