using JS.CharacterSystem;
using UnityEngine;

namespace JS.ECS
{
    public class CombatSystem : MonoBehaviour
    {
        private static CombatSystem instance;

        private enum HitResult
        {
            Miss,
            Hit,
            Critical
        }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;
        }

        public static bool TryMeleeAttack(Combat combatant, Vector2Int attackPos)
        {
            if (GridManager.WorldMapActive) return false;

            var validTargets = TransformSystem.GetEntitiesAt(combatant.entity.GetComponent<Physics>().Position, attackPos);
            if (validTargets == null || validTargets.Length == 0)
            {
                MessageSystem.NewMessage("Nothing to attack");
                Debug.Log(combatant.entity.GetComponent<Physics>().LocalPosition + ", " +  attackPos);
                return false; //There is nothing to attack here
            }

            var target = instance.GetHighestPrecedenceTarget(validTargets);
            target.entity.FireEvent(new AttackedBy(combatant.entity));
            //Debug.Log("Trying to attack. " + target.entity.Name);

            //Get a list of all valid Melee Attacks
            var E1 = new GetMeleeAttacks();
            combatant.entity.FireEvent(E1);
            if (E1.attacks.Count == 0)
            {
                MessageSystem.NewMessage("Nothing to attack " + target.entity.Name + " with!");
                return false;
            }

            for (int i = 0; i < E1.attacks.Count; i++)
            {
                var result = instance.GetMeleeAttackResult(combatant, E1.attacks[i].entity, target.entity, i);

                if (result == HitResult.Critical) instance.OnMeleeCrit(E1.attacks[i].entity, target.entity);
                else if (result == HitResult.Hit) instance.OnMeleeHit(combatant.entity, E1.attacks[i].entity, target.entity);
                else MessageSystem.NewMessage(combatant.entity.Name + " missed " + target.entity.Name);
            }

            return true;
        }

        private HitResult GetMeleeAttackResult(Combat combatant, Entity weapon, Entity target, int previousAttacks)
        {
            int roll = Dice.Roll(20); //Roll a d20 to determine initial accuracy
            var meleeWeapon = weapon.GetComponent<MeleeWeapon>();

            //!!!Ensure checking for critical hit happens before the roll is modified!!!
            bool criticalHit = roll >= GetCritRange(combatant, meleeWeapon);

            //Apply a -3 penalty for each consecutive melee attack made
            if (!combatant.hasMultiStrike) roll -= 3 * previousAttacks;
            else roll -= previousAttacks; // -1 if they have multi-strike

            if (meleeWeapon != null)
            {
                roll += meleeWeapon.Accuracy; //Add weapon's Accuracy, and attacker's weapon Proficiency
                if (combatant.entity.TryGetStat(meleeWeapon.Proficiency, out StatBase prof)) roll += prof.Value;
            }

            int DV = 0; //Get target's Dodge Value
            if (target.TryGetStat("DV", out StatBase stat)) DV = stat.Value;

            if (criticalHit)
            {
                if (roll >= DV) return HitResult.Critical;
                return HitResult.Hit;
            }
            if (roll >= DV) return HitResult.Hit;
            return HitResult.Miss;
        }

        private void OnMeleeHit(Entity attacker, Entity meleeWeapon, Entity target)
        {
            //Get Damage from the object
            var E1 = new DealingMeleeDamage();
            meleeWeapon.FireEvent(E1);

            //Pass damage to target
            var E2 = new TakeDamage(E1.Damage);
            target.FireEvent(E2);

            string log = attacker.Name + " hit " + target.Name + " for ";
            foreach(var pair in E1.Damage)
            {
                log += pair.Value + " " + pair.Key + " damage,";
            }
            log.TrimEnd(',');
            log += " with " + meleeWeapon.Name + ".";
            MessageSystem.NewMessage(log);
        }

        private void OnMeleeCrit(Entity meleeWeapon, Entity target)
        {

        }

        /// <summary>
        /// Returns the Critical Hit Range for the combatant using the given weapon.
        /// </summary>
        private int GetCritRange(Combat combatant, MeleeWeapon weapon)
        {
            int range = 20 - combatant.CritRangeBonus; //combatant's bonuse
            if (weapon != null) range -= weapon.CritRange; //weapon bonus
            range = Mathf.Clamp(range, 15, 20); //clamp to prevent broken combos
            return range;
        }

        public static bool TryRangedAttack(Combat combatant, Vector2Int attackPos)
        {
            Debug.LogWarning("Not yet implemented.");
            return false;
        }

        /// <summary>
        /// Returns highest value target from array based on combatant status and sentience
        /// </summary>
        private Physics GetHighestPrecedenceTarget(Physics[] targets)
        {
            //target combatants first
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].entity.GetComponent<Combat>() != null) return targets[i];
            }
            //target sentient targets next
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].entity.GetComponent<Brain>() != null) return targets[i];
            }
            return targets[0];
        }
    }
}