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
        /// Try to move one tile in the given direction on the world map.
        /// </summary>
        public static void TryMoveWorld(Transform t, Compass direction, bool isForced = false)
        {
            if (!WorldLocomotionSystem.TryMoveWorld(t, direction, out int cost)) return;
            if (isForced) return; //Forced movement does not spend AP

            if (!EntityManager.TryGetComponent<TimedActor>(t.entity, out var actor)) return;

            var E1 = new GetStat("WALK");
            EntityManager.FireEvent(t.entity, E1);
            E1.Value = Mathf.Clamp(E1.Value, 1, 200);
            int netCost = Mathf.RoundToInt(LocomotionSystem.movementDividend / (E1.Value - cost));

            TimeSystem.SpendActionPoints(actor, netCost);
            TimeSystem.EndTurn(actor);
        }

        /// <summary>
        /// Try to move one tile in given direction.
        /// </summary>
        public static void TryMoveLocal(Transform obj, Compass direction, bool isForced = false)
        {
            if (!LocomotionSystem.TryMoveLocal(obj, direction, out int cost)) return;
            if (isForced) return; //Forced movement does not spend AP

            if (!EntityManager.TryGetComponent<TimedActor>(obj.entity, out var actor)) return;

            var E1 = new GetStat("WALK");
            EntityManager.FireEvent(obj.entity, E1);
            E1.Value = Mathf.Clamp(E1.Value, 1, 200);
            int netCost = Mathf.RoundToInt(LocomotionSystem.movementDividend / (E1.Value - cost));

            TimeSystem.SpendActionPoints(actor, netCost);
            TimeSystem.EndTurn(actor);
        }

        public static void TryMeleeAttack(Combat combatant, Vector2Int position)
        {
            if (!EntityManager.TryGetComponent<TimedActor>(combatant.entity, out var actor)) return;
            if (!actor.IsTurn) return; //not your turn. May change this for special cases

            if (!CombatSystem.TryMeleeAttack(combatant, position)) return;

            TimeSystem.SpendActionPoints(actor, BaseActionCost);
            TimeSystem.EndTurn(actor);
        }

        public static void TryTakeItem(Entity entity, Physics itemToTake)
        {
            EntityManager.TryGetComponent<Inventory>(entity, out var inventory);
            if (itemToTake == null || !itemToTake.IsReal || !itemToTake.IsTakeable) return;

            if (!EntityManager.TryGetComponent<TimedActor>(entity, out var actor)) return;
            if (!actor.IsTurn) return; //not your turn. May change this for special cases

            inventory.AddObject(itemToTake);

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