using UnityEngine;

namespace JS.ECS
{
    public class PoisonResistance : ComponentBase
    {
        public int Amount;

        public PoisonResistance(Physics physics, int amount)
        {
            Priority = 2;
            Amount = amount;
            physics.entity.AddComponent(this);
        }

        public override void OnEvent(Event newEvent)
        {
            if (newEvent is TakeDamage damage) ApplyResistance(damage);
        }

        private void ApplyResistance(TakeDamage damage)
        {
            if (damage.Types.Contains((int)DamageTypes.Poison))
            {
                int index = damage.Amounts.IndexOf((int)DamageTypes.Poison);
                damage.Amounts[index] = Mathf.RoundToInt(damage.Amounts[index] * Resistance.GetModifier(Amount));
            }
        }
    }
}
