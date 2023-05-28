using UnityEngine;

namespace JS.ECS
{
    public class MysticResistance : ComponentBase
    {
        public int Amount;

        public MysticResistance(Physics physics, int amount)
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
            if (damage.Types.Contains((int)DamageTypes.Mystic))
            {
                int index = damage.Amounts.IndexOf((int)DamageTypes.Mystic);
                damage.Amounts[index] = Mathf.RoundToInt(damage.Amounts[index] * Resistance.GetModifier(Amount));
            }
        }
    }
}
