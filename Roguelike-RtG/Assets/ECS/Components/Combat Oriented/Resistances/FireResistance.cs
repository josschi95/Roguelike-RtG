using UnityEngine;

namespace JS.ECS
{
    public class FireResistance : ComponentBase
    {
        public int Amount;

        public FireResistance(int amount)
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
            if (damage.Types.Contains((int)DamageTypes.Fire))
            {
                int index = damage.Amounts.IndexOf((int)DamageTypes.Fire);
                damage.Amounts[index] = Mathf.RoundToInt(damage.Amounts[index] * Resistance.GetModifier(Amount));
            }
        }
    }
}
