using UnityEngine;

namespace JS.ECS
{
    public class AcidResistance : ComponentBase
    {
        public AcidResistance() { }

        public AcidResistance(int amount)
        {
            Priority = 2;
            Amount = amount;
        }

        public int Amount;

        public override void OnEvent(Event newEvent)
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