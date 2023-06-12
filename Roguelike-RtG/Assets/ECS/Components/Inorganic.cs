namespace JS.ECS
{
    /// <summary>
    /// Component to add to Inorganic objects, making them immune to Bleed, Poison, Psychic and Positive/Negative damage
    /// </summary>
    public class Inorganic : ComponentBase
    {
        //Set Priority to higher value to ensure OnTakeDamage reduces organic-based damage
        public Inorganic() 
        {
            Priority = 2;
        }

        public override void OnEvent(Event newEvent)
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
            foreach(var pair in damage.Damage)
            {
                if (pair.Key == "Bleed") damage.Damage.Remove(pair.Key);
                else if (pair.Key == "Poison") damage.Damage.Remove(pair.Key);
                else if (pair.Key == "Psychic") damage.Damage.Remove(pair.Key);
                else if (pair.Key == "Positive") damage.Damage.Remove(pair.Key);
                else if (pair.Key == "Negative") damage.Damage.Remove(pair.Key);
            }
        }
    }
}