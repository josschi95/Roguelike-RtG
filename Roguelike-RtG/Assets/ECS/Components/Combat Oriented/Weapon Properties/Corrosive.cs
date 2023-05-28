namespace JS.ECS
{
    /// <summary>
    /// Component which adds the Corrosive Property to an object and adds acid damage to attacks
    /// </summary>
    public class Corrosive : ComponentBase
    {
        public Corrosive(Physics physics)
        {
            Priority = 2;
            physics.entity.AddComponent(this);
        }

        public override void FireEvent(Event newEvent)
        {
            if (newEvent is DealingMeleeDamage damage)
            {
                AddDamage(damage);
            }
        }

        private void AddDamage(DealingMeleeDamage damage)
        {
            var result = Dice.Roll("1d6");
            if (damage.Types.Contains((int)DamageTypes.Acid))
            {
                int index = damage.Amounts.IndexOf((int)DamageTypes.Acid);
                damage.Amounts[index] += result;
            }
            else
            {
                damage.Amounts.Add(result);
                damage.Types.Add((int)DamageTypes.Acid);
            }
        }
    }
}