using System.Collections.Generic;
using JS.ECS;

public class Event
{

}

public class MeleeAttackHit : Event
{
    public PhysicsBehavior target;
    public MeleeAttackHit(PhysicsBehavior target)
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

public class ApplyDamage : Event
{
    public int Amount;
    public ApplyDamage(int amount)
    {
        Amount = amount;
    }
}