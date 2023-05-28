using UnityEngine;

namespace JS.ECS
{
    public class PhysicsBehavior : ComponentBase
    {
        public int hitPoints;

        public PhysicsBehavior(Entity entity, int hitPoints)
        {
            this.entity = entity;
            this.hitPoints = hitPoints;
        }

        public override void OnEvent(Event newEvent)
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

        private void OnMeleeAttackDeclared(MeleeAttackMade attack)
        {

        }

        private void OnMeleeAttackHit(PhysicsBehavior target)
        {
            var E1 = new DealingMeleeDamage();

            entity.SendEvent(E1);

            var E2 = new TakeDamage(E1.Amounts, E1.Types);

            target.entity.SendEvent(E2);
        }

        private void OnDealingMeleeDamage(DealingMeleeDamage dmg)
        {

        }

        private void OnTakeDamage(TakeDamage damage)
        {
            for (int i = 0; i < damage.Amounts.Count; i++)
            {
                hitPoints -= damage.Amounts[i];
            }
        }
    }

    public class CombatBehavior
    {
        public void TryMeleeAttack()
        {
            
        }
    }

    public class WeaponBehaviour : ComponentBase
    {
        public int Amount;
        public int Type;
        
        public WeaponBehaviour(PhysicsBehavior physics)
        {
            Priority = 1;
            physics.entity.AddComponent(this);
        }

        public override void OnEvent(Event newEvent)
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
            Priority = 2;
            physics.entity.AddComponent(this);
        }

        public override void OnEvent(Event newEvent)
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
            Priority = 2;
            Amount = amount;
            physics.entity.AddComponent(this);
        }

        public override void OnEvent(Event newEvent)
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
