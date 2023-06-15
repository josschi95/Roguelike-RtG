using UnityEngine;

namespace JS.ECS
{
    /// <summary>
    /// Component for all physical objects which can be interacted with
    /// </summary>
    public class Physics : ComponentBase
    {
        public Physics() { Priority = int.MaxValue; }

        public bool IsTakeable = true; //Can the object be picked up
        public bool IsSolid = false; //Does the object block gas from spreading and line of sight
        public bool IsCorporeal = true;
        public bool IsReal = true; //False for some magical effects, visual effects, etc.
        public float Weight = 1.0f; //The weight of the object
        public string Category = "Miscellaneous";
        //public PhysicsCategory Category = PhysicsCategory.Miscellaneous; //The type of object

        public override void OnEvent(Event newEvent)
        {
            switch (newEvent)
            {
                case GetStat stat:
                    OnGetStat(stat);
                    break;
                case TakeDamage damage:
                    OnTakeDamage(damage);
                    break;
                case Death death:
                    OnDeath();
                    break;
            }
        }

        private void OnGetStat(GetStat stat)
        {
            if (EntityManager.TryGetStat(entity, stat.Name, out var value))
            {
                stat.Value += value.Value;
            }
        }

        private void OnTakeDamage(TakeDamage damage)
        {
            EntityManager.TryGetStat(entity, "HP", out var stat);
            if (stat.Value <= 0) return; //already dead

            foreach (var key in damage.Damage.Keys)
            {
                if (stat.CurrentValue <= 0) return;

                var E1 = new GetStat(key + "Resistance");
                EntityManager.FireEvent(entity, E1);

                int net = Mathf.RoundToInt(damage.Damage[key] * Resistance.GetModifier(E1.Value));
                stat.CurrentValue -= net;
            }

            if (stat.CurrentValue <= 0) EntityManager.FireEvent(entity, new Death());
        }

        private void OnDeath()
        {
            MessageSystem.NewMessage(entity.Name + " has been killed");
            CorpseManager.OnCreatureDeath(entity);
            EntityManager.Destroy(entity);
        }
    }
}

/*public enum PhysicsCategory
{
    Miscellaneous,
    Creature,
    Item,
    Wall,
    MeleeWeapon,
    MissileWeapon,
    Projectile,
    Armor,
    Shield,
    NaturalWeapon,
    NaturalMissileWeapon,
    NaturalArmor,
}*/

public enum DamageTypes
{
    Blunt,
    Pierce,
    Slash,
    Bleed,

    Fire,
    Frost,
    Lightning,

    Poison,
    Acid,

    Sonic,
    Psychic,
    Mystic,

    Positive,
    Negative,
}
