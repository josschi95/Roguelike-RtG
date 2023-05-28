using UnityEngine;

namespace JS.ECS
{
    public static class PerformAction
    {
        public static bool TryMoveAction(TimedActor actor, Locomotion entity, Compass direction)
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

        public static void TryMeleeAttack(PhysicsBehavior attacker, PhysicsBehavior target, int accuracy)
        {
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


        }

        public static void TryAttack(TimedActor actor)
        {

        }

        public static void SkipAction(TimedActor entity)
        {
            if (!entity.IsTurn) return;
            TimeSystem.SpendActionPoints(entity);
            TimeSystem.EndTurn(entity);
        }
    }

    public abstract class ActionBase
    {

    }

    public class MoveAction : ActionBase
    {

    }

    public class MeleeAttackAction : ActionBase
    {
        public int accuracy;
    }
}

public enum MoveResult
{
    Success, //No message
    Wall, //There's a wall there
    Enemy, //The <name> blocks your path
}