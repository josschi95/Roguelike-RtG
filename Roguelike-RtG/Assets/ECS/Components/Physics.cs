using System;

namespace JS.ECS
{
    /// <summary>
    /// Component for all physical objects which can be interacted with
    /// </summary>
    public class Physics : ComponentBase
    {
        private Transform _transform;
        public Transform Transform
        {
            get
            {
                if (_transform == null)
                {
                    _transform = entity.GetComponent<Transform>();
                }
                return _transform;
            }
        }
        

        public bool IsTakeable = true; //Can the object be picked up
        public bool IsSolid = false; //Does the object block gas from spreading
        public float Weight = 1.0f; //The weight of the object
        public ObjectCategory Category = ObjectCategory.Miscellaneous; //The type of object

        //CoQ also has values for temp, but... yeah.

        public Physics(bool takeable = true, bool solid = false, float weight = 1.0f, ObjectCategory category = ObjectCategory.Miscellaneous)
        {
            IsTakeable = takeable;
            IsSolid = solid;
            Weight = weight;
            Category = category;
        }

        public override void FireEvent(Event newEvent)
        {
            switch (newEvent)
            {
                case MeleeAttackHit hit:
                    OnMeleeAttackHit(hit.target);
                    break;
            }
        }

        private void OnMeleeAttackHit(Physics target)
        {
            var E1 = new DealingMeleeDamage();

            entity.FireEvent(E1);

            var E2 = new TakeDamage(E1.Amounts, E1.Types);

            target.entity.FireEvent(E2);
        }
    }

    public class WeaponBehaviour : ComponentBase
    {
        public int Amount;
        public int Type;
        
        public WeaponBehaviour(Physics physics)
        {
            Priority = 1;
            physics.entity.AddComponent(this);
        }

        public override void FireEvent(Event newEvent)
        {
            if (newEvent is DealingMeleeDamage damage)
            {
                damage.Amounts[0] = Amount;
                damage.Types[0] = Type;
            }
        }
    }
}

public enum ObjectCategory
{
    Miscellaneous,
    Creature,
    Item,
    Wall,
}

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

    Positive,
    Negative,

    Sonic,
    Psychic,
    Mystic,
}
