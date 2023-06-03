using UnityEngine;

namespace JS.ECS
{
    public class NegativeEnergyResistance : ComponentBase
    {
        public int Amount;

        public NegativeEnergyResistance(Physics physics, int amount)
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
            if (damage.Types.Contains((int)DamageTypes.Negative))
            {
                int index = damage.Amounts.IndexOf((int)DamageTypes.Negative);
                damage.Amounts[index] = Mathf.RoundToInt(damage.Amounts[index] * Resistance.GetModifier(Amount));
            }
        }
    }
}