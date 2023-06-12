using JS.CharacterSystem;
using UnityEngine;

namespace JS.ECS
{
    public static class Actions
    {
        public const int BaseActionCost = 1000;

        /// <summary>
        /// Do nothing. End turn.
        /// </summary>
        public static void SkipAction(TimedActor actor)
        {
            if (!actor.IsTurn) return;
            //UnityEngine.Debug.Log("SkipAction " + actor.entity.Name);
            TimeSystem.SpendActionPoints(actor);
            TimeSystem.EndTurn(actor);
        }

        /// <summary>
        /// Try to move one tile in given direction.
        /// </summary>
        public static void TryMoveLocal(Physics obj, Compass direction, bool isForced = false)
        {
            if (!LocomotionSystem.TryMoveLocal(obj, direction, out int cost)) return;
            if (isForced) return; //Forced movement does not spend AP

            var actor = obj.entity.GetComponent<TimedActor>();
            if (actor == null) return;

            var E1 = new GetStat("MoveSpeed");
            obj.entity.FireEvent(E1);
            E1.Value = Mathf.Clamp(E1.Value, 1, 200);
            int netCost = Mathf.RoundToInt(LocomotionSystem.movementDividend / (E1.Value - cost));

            TimeSystem.SpendActionPoints(actor, netCost);
            TimeSystem.EndTurn(actor);
        }

        //is it in range?
        //does the attack hit? Accuracy vs. Dodge
        //(Done) Get the resulting damage from the attack
        //(Done) Pass the resulting damage to the target

        //so the attacker attempts a melee attack at the target
        //assume that the attacker was only able to do this because the target was in range (checked before this)

        //so the attacker makes an accuracy check, and this would be dependent on both the state of the attacker and what they're attacking with
        //meaning that an event needs to be sent to both the creature and the weapon
        //but that may not always be the case because it might be an unarmed attack, or it might be a trap
        //

        public static void TryMeleeAttack(Combat combatant, Vector2Int position)
        {
            var actor = combatant.entity.GetComponent<TimedActor>();
            if (actor != null && !actor.IsTurn) return; //not your turn. May change this for special cases

            if (!CombatSystem.TryMeleeAttack(combatant, position)) return;

            TimeSystem.SpendActionPoints(actor, BaseActionCost);
            TimeSystem.EndTurn(actor);
        }
    }
}

public enum MoveResult
{
    Success, //No message
    Wall, //There's a wall there
    Enemy, //The <name> blocks your path
}