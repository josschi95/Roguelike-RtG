using System.Collections.Generic;
using JS.ECS;

public class Event
{
    //string ID;
    //Dictionary<string, Object> Parameters;
}

public class MeleeAttackMade : Event
{

}

public class TargetedByMelee : Event
{

}

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