using UnityEngine;

namespace JS.ECS
{
    public class BleedResistance : ComponentBase
    {
        public BleedResistance()
        {
            Priority = 2;
        }

        public BleedResistance(int amount)
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
            if (damage.Types.Contains((int)DamageTypes.Bleed))
            {
                int index = damage.Amounts.IndexOf((int)DamageTypes.Bleed);
                damage.Amounts[index] = Mathf.RoundToInt(damage.Amounts[index] * Resistance.GetModifier(Amount));
            }
        }
    }
}
