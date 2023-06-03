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
            var result = Dice.Roll("1d6");
            if (damage.Types.Contains((int)DamageTypes.Lightning))
            {
                int index = damage.Amounts.IndexOf((int)DamageTypes.Lightning);
                damage.Amounts[index] += result;
            }
            else
            {
                damage.Amounts.Add(result);
                damage.Types.Add((int)DamageTypes.Lightning);
            }
        }
    }
}