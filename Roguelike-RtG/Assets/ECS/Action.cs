using UnityEngine;

namespace JS.ECS
{
    public static class Action
    {
        public static bool TryMoveAction(TimedActor actor, Locomotion entity, Vector2Int direction)
        {
            Debug.Log("TryMoveAction");
            if (!actor.IsTurn) return false;
            int cost = LocomotionSystem.MoveEntity(entity, direction);
            if (cost > 0)
            {
                TimeSystem.SpendActionPoints(actor, cost);
            }
            TimeSystem.EndTurn(actor);
            return true;
        }

        public static void SkipAction(TimedActor entity)
        {
            Debug.Log("SkipAction");
            if (!entity.IsTurn) return;
            TimeSystem.SpendActionPoints(entity);
            TimeSystem.EndTurn(entity);
        }
    }

}