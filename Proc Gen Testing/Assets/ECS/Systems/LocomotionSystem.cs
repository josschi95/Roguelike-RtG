using UnityEngine;

namespace JS.ECS
{
    public class LocomotionSystem : SystemBase<Locomotion>
    {
        private const float movementDivident = 100000;

        public static bool CanMoveToPosition(Vector2 position)
        {
            //Is position valid?
            //Is position Obstructed?

            return true;
        }

        public static int MoveEntity(Locomotion entity, Vector2 position)
        {
            if (!CanMoveToPosition(position)) return 0;

            //Will also need to take into account difficult terrain, movement modifiers, etc. 
            //Movement modifiers should affect Locomotion directly, and have no impact on this
            int cost = Mathf.RoundToInt(movementDivident / entity.MovementSpeed);

            entity.Transform.position = position;
            return cost;
        }
    }
}