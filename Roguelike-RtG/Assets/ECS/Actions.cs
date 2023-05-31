namespace JS.ECS
{
    public static class Actions
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
        public static void TryAttack(TimedActor actor, Compass direction)
        {
            if (!actor.IsTurn) return;
            //Debug.Log("TryAttack " + _actor.entity.Name);
            //Is there an adjacent creature? What is the attacker's range? is there a target within range?

            //If there is, make an attack


            TimeSystem.SpendActionPoints(actor);
            TimeSystem.EndTurn(actor);
        }

        public static void SkipAction(TimedActor actor)
        {
            if (!actor.IsTurn) return;
            //UnityEngine.Debug.Log("SkipAction " + actor.entity.Name);
            TimeSystem.SpendActionPoints(actor);
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