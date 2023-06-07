using JS.CharacterSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace JS.ECS
{
    /// <summary>
    /// Component to handle basic behavior for sentient creatures
    /// </summary>
    public class Brain : ComponentBase
    {
        public Brain() { }

        public bool IsSleeping = false; //Automatically skips the creature's turn if sleeping
        public bool IsHostile = false; //Is the object hositle by default?
        public bool IsCalm = false; //Will the object become hostile if attacked?
        public bool IsMobile = true; //Can the object move?
        public bool Wanders = true; //Does the object wander when not engaged?
        public Habitat Habitat = Habitat.Terrestrial; //Can the object go on land/water? 
        public string Faction; //What faction does the object belong to?
        public int FactionDisposition; //What is the faction's dispositio towards this object
        public bool HasOverride = false; //is there some other component controlling behavior?

        //Other CoQ properties
        //public int MaxWanderDist = 5;
        //public bool WandersRandomly = false;
        //public bool LivesOnWalls = false;
        //public int MinKillRadius;
        //public int MaxKillRadius;
        //public bool Hibernating = true;?
        //public bool PointBlankRange = false;

        //Other possible properties
        //public int/enum Rationality
        //public int/enum DecisionMaking

        Stack<Goal> Goals = new Stack<Goal>();
        
        private TimedActor _actor;
        public TimedActor Actor
        {
            get
            {
                if (_actor == null)
                {
                    _actor = entity.GetComponent<TimedActor>();
                }
                return _actor;
            }
        }

        private int MoveSpeed
        {
            get
            {
                if (entity.TryGetStat("MoveSpeed", out StatBase stat))
                {
                    return stat.Value;
                }
                return 1;
            }
        }

        public override void OnEvent(Event newEvent)
        {
            if (newEvent is TurnStart) OnTurnStart();
        }

        private void OnTurnStart()
        {
            if (HasOverride) return;

            if (IsSleeping) Actions.SkipAction(entity.GetComponent<TimedActor>());
            else if (Wanders && IsMobile)
            {
                if (UnityEngine.Random.value < 0.15) Actions.SkipAction(entity.GetComponent<TimedActor>());
                else if (LocomotionSystem.TryMoveLocal(entity.GetComponent<Physics>(), DirectionHelper.GetRandom(), out int cost))
                {
                    int netCost = UnityEngine.Mathf.RoundToInt(LocomotionSystem.movementDividend / (MoveSpeed - cost));
                    TimeSystem.SpendActionPoints(Actor, netCost);
                    TimeSystem.EndTurn(Actor);
                }
                else Actions.SkipAction(entity.GetComponent<TimedActor>());
            }
            else Actions.SkipAction(entity.GetComponent<TimedActor>());


        }

        private void TakeAction()
        {
            while(Goals.Peek().Finished()) Goals.Pop();
            Goals.Peek().TakeAction();
        }
    }
}

public class Goal
{
    public bool Finished()
    {
        return true;
    }
    
    public void TakeAction()
    {

    }
    public Goal OriginalIntent; //If TakeAction returns false (can't move, can't heal, can't kill, go back to this


    //Has the goal been accomplished?

}

//What are some possible goals? 
/* Murder someone, finished if that target's HP is <= 0
 *      Accomplish by Approach Target within range to attack, and then attacking
 * 
 * Approach Target
 *      Accomplished by moving towards target
 *      
 * Heal someone (could be self), finished if that target's HP is >= some set point
 *      Accomplished by acquiring a source of healing (food, magic, etc.) 
 *      this should only be possible if they have that source in the first place
 * 
 * Flee, finished if the source of their flight is no longer within FoV
 *      Accomplished by moving in opposite direction
 *      
 * 
 */

public enum Habitat
{
    Terrestrial,
    Aquatic,
    Amphibious,

}