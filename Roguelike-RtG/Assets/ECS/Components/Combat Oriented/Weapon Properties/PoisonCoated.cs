namespace JS.ECS
{
    /// <summary>
    /// Component which adds the PoisonCoated Property to an object and adds poison damage to attacks.
    /// Note that this is distinct from the Poisoned property which inflicts damage to the object it is applied to.
    /// </summary>
    public class PoisonCoated : ComponentBase
    {
        public string Amount;
        public PoisonCoated(Physics physics, string amount = "1d6")
        {
            Amount = amount;
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
            var result = Dice.Roll(Amount);
            if (damage.Types.Contains((int)DamageTypes.Poison))
            {
                int index = damage.Amounts.IndexOf((int)DamageTypes.Poison);
                damage.Amounts[index] += result;
            }
            else
            {
                damage.Amounts.Add(result);
                damage.Types.Add((int)DamageTypes.Poison);
            }
        }
    }
}

