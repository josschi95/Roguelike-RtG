using UnityEngine;

namespace JS.ECS
{
    public class AcidResistance : ComponentBase
    {
        public int Amount;

        public AcidResistance(Physics physics, int amount)
        {
            Priority = 2;
            Amount = amount;
            physics.entity.AddComponent(this);
        }

        public override void FireEvent(Event newEvent)
        {
            if (newEvent is TakeDamage damage) ApplyResistance(damage);
        }

        private void ApplyResistance(TakeDamage damage)
        {
            if (damage.Types.Contains((int)DamageTypes.Acid))
            {
                int index = damage.Amounts.IndexOf((int)DamageTypes.Acid);
                damage.Amounts[index] = Mathf.RoundToInt(damage.Amounts[index] * Resistance.GetModifier(Amount));
            }
        }
    }
}