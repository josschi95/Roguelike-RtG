using System.Collections.Generic;
using JS.ECS;

namespace JS.ECS
{
    public class Event
    {
        //string ID; //string.Compare isn't that bad
        //Dictionary<string, Object> Parameters;
    }

    public class TransformChanged : Event { }

    #region - Turn/Round Events -
    /// <summary>
    /// Tells an entity that a new round has started.
    /// </summary>
    public class NewRound: Event
    {
    }

    /// <summary>
    /// Tells an entity that their turn has started.
    /// </summary>
    public class TurnStart : Event 
    {
    }

    /// <summary>
    /// Tells an entity that their turn has ended.
    /// </summary>
    public class TurnEnd : Event
    {
    }
    #endregion

    /// <summary>
    /// Tells an entity that their transform has changed.
    /// </summary>


    #region - Combat Events -
    public class DeclareMeleeAttack : Event
    {
        public Physics target;
        public DeclareMeleeAttack(Physics target)
        {
            this.target = target;
        }
    }

    public class TargetedByMelee : Event { }

    public class MeleeAttackHit : Event
    {
        public Physics target;
        public MeleeAttackHit(Physics target)
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
        public List<int> Amounts;
        public List<int> Types;

        public DealingMeleeDamage(int amount = 1, int type = 0)
        {
            Amounts = new List<int>() { amount };
            Types = new List<int>() { type };
        }
    }

    public class TakeDamage : Event
    {
        public List<int> Amounts;
        public List<int> Types;
        public TakeDamage(List<int> amount, List<int> type)
        {
            Amounts = amount;
            Types = type;
        }
    }
    #endregion
}