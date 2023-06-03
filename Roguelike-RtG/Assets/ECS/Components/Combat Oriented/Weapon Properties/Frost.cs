namespace JS.ECS
{
    /// <summary>
    /// Component which adds the Frost Property to an object and adds frost damage to attacks
    /// </summary>
    public class Frost : ComponentBase
    {
        public Frost(Physics physics)
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
            if (damage.Types.Contains((int)DamageTypes.Frost))
            {
                int index = damage.Amounts.IndexOf((int)DamageTypes.Frost);
                damage.Amounts[index] += result;
            }
            else
            {
                damage.Amounts.Add(result);
                damage.Types.Add((int)DamageTypes.Frost);
            }
        }
    }
}