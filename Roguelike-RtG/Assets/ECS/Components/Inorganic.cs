namespace JS.ECS
{
    /// <summary>
    /// Component to add to Inorganic objects, making them immune to Bleed, Poison, and Psychic damage
    /// </summary>
    public class Inorganic : ComponentBase
    {
        public override void FireEvent(Event newEvent)
        {
            switch (newEvent)
            {
                case TakeDamage damage:
                    OnTakeDamage(damage);
                    break;
            }
        }

        private void OnTakeDamage(TakeDamage damage)
        {
            for (int i = 0; i < damage.Types.Count; i++)
            {
                if (damage.Types[i] == (int)DamageTypes.Bleed) damage.Amounts[i] = 0;
                if (damage.Types[i] == (int)DamageTypes.Poison) damage.Amounts[i] = 0;
                if (damage.Types[i] == (int)DamageTypes.Psychic) damage.Amounts[i] = 0;
            }
        }
    }
}