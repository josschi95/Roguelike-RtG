using UnityEngine;

namespace JS.ECS
{
    public class BluntResistance : ComponentBase
    {
        public int Amount;

        public BluntResistance(int amount)
        {
            Priority = 2;
            Amount = amount;
        }

        public override void OnEvent(Event newEvent)
        {
            if (newEvent is TakeDamage damage) ApplyResistance(damage);
        }

        private void ApplyResistance(TakeDamage damage)
        {
            if (damage.Types.Contains((int)DamageTypes.Blunt))
            {
                int index = damage.Amounts.IndexOf((int)DamageTypes.Blunt);
                damage.Amounts[index] = Mathf.RoundToInt(damage.Amounts[index] * Resistance.GetModifier(Amount));
            }
        }
    }
}
