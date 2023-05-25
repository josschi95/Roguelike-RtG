using UnityEngine;

namespace JS.ECS
{
    public static class Action
    {
        public static bool TryMoveAction(TimedActor actor, Locomotion entity, Vector2Int direction)
        {
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
            if (!entity.IsTurn) return;
            TimeSystem.SpendActionPoints(entity);
            TimeSystem.EndTurn(entity);
        }
    }

}

public enum MoveResult
{
    Success, //No message
    Wall, //There's a wall there
    Enemy, //The <name> blocks your path
}