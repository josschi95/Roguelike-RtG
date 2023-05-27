using System.Collections.Generic;
using UnityEngine;

namespace JS.ECS
{
    public class PhysicsBehavior : ComponentBase
    {
        public int hitPoints;
        private List<ComponentBase> children;

        public PhysicsBehavior(Entity entity, int hitPoints)
        {
            this.entity = entity;
            this.hitPoints = hitPoints;
            children = new List<ComponentBase>();
        }

        public void AddChild(ComponentBase child)
        {
            for (int i = 0; i < children.Count; i++)
            {
                if (child.Priority > children[i].Priority)
                {
                    children.Insert(i, child);
                    return;
                }
                children.Add(child);
            }
        }

        public void SendEvent(Event newEvent)
        {
            switch (newEvent)
            {
                case MeleeAttackHit hit:
                    OnMeleeAttackHit(hit.target);
                    break;
                case DealingMeleeDamage dealing:
                    OnDealingMeleeDamage(dealing);
                    break;
                case TakeDamage dmg: 
                    OnTakeDamage(dmg); 
                    break;
            }
        }

        private void OnMeleeAttackHit(PhysicsBehavior target)
        {
            var E1 = new DealingMeleeDamage();

            this.SendEvent(E1);

            var E2 = new TakeDamage(E1.Amounts, E1.Types);

            target.SendEvent(E2);
        }

        private void OnDealingMeleeDamage(DealingMeleeDamage dmg)
        {
            for (int i = 0; i < children.Count; i++)
            {
                //children[i].SendEvent(dmg);
            }
        }

        private void OnTakeDamage(TakeDamage damage)
        {
            for (int i = 0; i < damage.Amounts.Count; i++)
            {
                hitPoints -= damage.Amounts[i];
            }
        }
    }

    public class WeaponBehaviour : ComponentBase
    {
        public int Amount;
        public int Type;
        
        public WeaponBehaviour(PhysicsBehavior physics)
        {
            entity = physics.entity;
            Priority = 1;
            physics.AddChild(this);
        }

        public void OnEvent(Event newEvent)
        {
            if (newEvent is DealingMeleeDamage damage)
            {
                damage.Amounts[0] = Amount;
                damage.Types[0] = Type;
            }
        }
    }

    public class Fiery : ComponentBase
    {
        public Fiery(PhysicsBehavior physics)
        {
            entity = physics.entity;
            Priority = 2;
            physics.AddChild(this);
        }

        public void OnEvent(Event newEvent)
        {
            if (newEvent is DealingMeleeDamage damage)
            {
                AddDamage(damage);
            }
        }

        private void AddDamage(DealingMeleeDamage damage)
        {
            var result = Dice.Roll(6);
            if (damage.Types.Contains((int)DamageTypes.Fire))
            {
                int index = damage.Amounts.IndexOf((int)DamageTypes.Fire);
                damage.Amounts[index] += result;
            }
            else
            {
                damage.Amounts.Add(result);
                damage.Types.Add((int)DamageTypes.Fire);
            }
        }
    }

    public class FireResist : ComponentBase
    {
        public int Amount;

        public FireResist(PhysicsBehavior physics, int amount)
        {
            entity = physics.entity;
            Priority = 2;
            Amount = amount;
            physics.AddChild(this);
        }

        public void OnEvent(Event newEvent)
        {
            if (newEvent is TakeDamage damage) ApplyResistance(damage);
        }

        private void ApplyResistance(TakeDamage damage)
        {
            if (damage.Types.Contains((int)DamageTypes.Fire))
            {
                int index = damage.Amounts.IndexOf((int)DamageTypes.Fire);
                damage.Amounts[index] = Mathf.RoundToInt(damage.Amounts[index] * Resistance.GetModifier(Amount));
            }
        }
    }
}

public enum DamageTypes
{
    Blunt,
    Pierce,
    Slash,

    Fire,
    Frost,
    Lightning,


}
