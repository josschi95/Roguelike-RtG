using System.Collections.Generic;

namespace JS.ECS
{
    public class Event
    {
        //string ID; //string.Compare isn't that bad
        //Dictionary<string, Object> Parameters;
    }

    /// <summary>
    /// Event to inform an entity that its Transform has changed.
    /// </summary>
    public class TransformChanged : Event { }

    #region - Turn/Round Events -
    /// <summary>
    /// Tells an entity that a new round has started.
    /// </summary>
    public class NewRound: Event { }

    /// <summary>
    /// Tells an entity that their turn has started.
    /// </summary>
    public class TurnStart : Event { }

    /// <summary>
    /// Tells an entity that their turn has ended.
    /// </summary>
    public class TurnEnd : Event { }
    #endregion

    public class GetStat : Event
    {
        public string Name;
        public int Value;

        public GetStat(string name)
        {
            Name = name;
        }
    }

    #region - Combat Events -
    /// <summary>
    /// Event to inform a target that they have been attacked by another entity
    /// </summary>
    public class AttackedBy : Event
    {
        public Entity entity;
        public AttackedBy(Entity entity)
        {
            this.entity = entity;
        }
    }

    public class GetMeleeAttacks : Event
    {
        public List<MeleeWeapon> weapons;
        public GetMeleeAttacks()
        {
            weapons = new List<MeleeWeapon>();
        }
    }

    public class DeclareMissileAttack : Event
    {
        public Physics target;
        public DeclareMissileAttack(Physics target)
        {
            this.target = target;
        }
    }

    public class RangedAttackHit : Event
    {
        public Physics target;
        public RangedAttackHit(Physics target)
        {
            this.target = target;
        }
    }

    public class DealingMeleeDamage : Event
    {
        public Dictionary<string, int> Damage;
        public bool isCrit;

        public DealingMeleeDamage(bool isCrit = false)
        {
            Damage = new Dictionary<string, int>();
            this.isCrit = isCrit;
        }
    }

    public class TakeDamage : Event
    {
        public Dictionary<string, int> Damage;

        public TakeDamage(Dictionary<string, int> dict)
        {
            Damage = dict;
        }
    }

    public class Death : Event { }
    #endregion
}