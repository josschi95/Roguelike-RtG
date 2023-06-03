using UnityEngine;
using JS.CharacterSystem;

namespace JS.ECS
{
    /// <summary>
    /// Component to hold all resistances for damage types
    /// </summary>
    public class DamageResistances : ComponentBase
    {
        public StatBase[] Resistances { get; private set; }

        public DamageResistances(StatBase[] resistances)
        {
            Priority = 2;
            Resistances = resistances;
        }

        public override void OnEvent(Event newEvent)
        {
            if (newEvent is TakeDamage damage) ApplyResistances(damage);
        }

        private void ApplyResistances(TakeDamage damage)
        {
            for (int i = 0; i < damage.Types.Count; i++)
            {
                int index = damage.Types[i];
                damage.Amounts[index] += Mathf.RoundToInt(damage.Amounts[index] * Resistance.GetModifier(Resistances[index].Value));
            }
        }
    }
}