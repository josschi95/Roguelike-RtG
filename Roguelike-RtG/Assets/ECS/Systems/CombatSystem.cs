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

            var validTargets = TransformSystem.GetEntitiesAt(combatant.Physics.Position, attackPos);
            if (validTargets == null || validTargets.Length == 0)
            {
                MessageSystem.NewMessage("Nothing to attack");
                //Debug.Log(combatant.Physics.LocalPosition + ", " +  attackPos);
                return false; //There is nothing to attack here
            }

            var target = instance.GetHighestPrecedenceTarget(validTargets);
            EntityManager.FireEvent(target.entity, new AttackedBy(combatant.entity));
            //Debug.Log("Trying to attack. " + target.entity.Name);

            //Get a list of all valid Melee Attacks
            var E1 = new GetMeleeAttacks();
            EntityManager.FireEvent(combatant.entity, E1);
            if (E1.attacks.Count == 0)
            {
                MessageSystem.NewMessage("Nothing to attack " + target.entity.Name + " with!");
                return false;
            }

            for (int i = 0; i < E1.attacks.Count; i++)
            {
                //Exit early if the target has already been defeated
                if (EntityManager.TryGetStat(target.entity, "HP", out var HP) && HP.CurrentValue <= 0) break;

                var result = instance.GetMeleeAttackResult(combatant, E1.attacks[i].entity, target.entity, i);

                if (result == HitResult.Critical) instance.OnMeleeHit(combatant.entity, E1.attacks[i].entity, target.entity, true);
                else if (result == HitResult.Hit) instance.OnMeleeHit(combatant.entity, E1.attacks[i].entity, target.entity);
                else MessageSystem.NewMessage(combatant.entity.Name + " missed " + target.entity.Name);
            }

            return true;
        }

        private HitResult GetMeleeAttackResult(Combat combatant, Entity weapon, Entity target, int previousAttacks)
        {
            int roll = Dice.Roll(20); //Roll a d20 to determine initial accuracy
            var meleeWeapon = EntityManager.GetComponent<MeleeWeapon>(weapon);

            //!!!Ensure checking for critical hit happens before the roll is modified!!!
            bool criticalHit = roll >= GetCritRange(combatant, meleeWeapon);

            //Apply a -3 penalty for each consecutive melee attack made
            if (!combatant.hasMultiStrike) roll -= 3 * previousAttacks;
            else roll -= previousAttacks; // -1 if they have multi-strike

            if (meleeWeapon != null) //literally anything stemming from PhysicalObject has this component...
            {
                roll += meleeWeapon.Accuracy; //Add weapon's Accuracy, and attacker's weapon Proficiency
                if (EntityManager.TryGetStat(combatant.entity, meleeWeapon.Proficiency, out StatBase prof)) roll += prof.Value;
            }

            int DV = 0; //Get target's Dodge Value
            if (EntityManager.TryGetStat(target, "DV", out StatBase stat)) DV = stat.Value;

            if (criticalHit)
            {
                if (roll >= DV) return HitResult.Critical;
                return HitResult.Hit;
            }
            if (roll >= DV) return HitResult.Hit;
            return HitResult.Miss;
        }

        private void OnMeleeHit(Entity attacker, Entity meleeWeapon, Entity target, bool isCrit = false)
        {
            //Get Damage from the object
            var E1 = new DealingMeleeDamage(isCrit);
            EntityManager.FireEvent(meleeWeapon, E1);

            //Pass damage to target
            EntityManager.FireEvent(target, new TakeDamage(E1.Damage));
            string log = "";
            if (isCrit) log = "Critical Hit! ";
            log += attacker.Name + " hit " + target.Name + " for ";
            foreach(var pair in E1.Damage)
            {
                log += pair.Value + " " + pair.Key + " damage,";
            }
            log.TrimEnd(',');
            log += " with " + meleeWeapon.Name + ".";
            MessageSystem.NewMessage(log);
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
                if (EntityManager.GetComponent<Combat>(targets[i].entity) != null) return targets[i];
            }
            //target sentient targets next
            for (int i = 0; i < targets.Length; i++)
            {
                if (EntityManager.GetComponent<Brain>(targets[i].entity) != null) return targets[i];
            }
            return targets[0];
        }
    }
}