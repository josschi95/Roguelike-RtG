using UnityEngine;

namespace JS.ECS
{
    public static class Action
    {
        public static bool MoveAction(TimedActor actor, Locomotion entity, Vector2Int direction)
        {
            if (actor.HasActed) return false;
            int cost = LocomotionSystem.MoveEntity(entity, direction);
            if (cost > 0)
            {
                TimeSystem.SpendActionPoints(actor, cost);
                actor.HasActed = true;
            }
            return true;
        }

        public static void SkipAction(TimedActor entity)
        {
            if (entity.HasActed) return;
            TimeSystem.SpendActionPoints(entity);
            entity.HasActed = true;
        }
    }

}