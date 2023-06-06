using UnityEngine;

namespace JS.ECS
{
    public class CombatSystem : MonoBehaviour
    {
        private static CombatSystem instance;

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
            var validTargets = TransformSystem.GetEntitiesAt(combatant.entity.GetComponent<Physics>().Position, attackPos);
            if (validTargets == null || validTargets.Length == 0) return false; //There is nothing to attack here

            var target = instance.GetHighestPrecedenceTarget(validTargets);

            Debug.Log("Trying to attack " + target.entity.Name);
            combatant.entity.FireEvent(new DeclareMeleeAttack(target));

            return true;
        }

        public static bool TryRangedAttack(Combat combatant, Vector2Int attackPos)
        {
            Debug.LogWarning("Not yet implemented.");
            return false;
        }

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