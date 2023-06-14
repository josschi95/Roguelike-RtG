using JS.CharacterSystem;
using System;
using System.Collections.Generic;

namespace JS.ECS
{
    /// <summary>
    /// Component to handle basic behavior for sentient creatures
    /// </summary>
    public class Brain : ComponentBase
    {
        public Brain() { }

        public bool IsAlive = true;
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
                    _actor = EntityManager.GetComponent<TimedActor>(entity);
                }
                return _actor;
            }
        }

        private Physics _physics;

        public Physics Physics
        {
            get
            {
                if (_physics == null)
                {
                    _physics = EntityManager.GetComponent<Physics>(entity);
                }
                return _physics;
            }
        }

        public override void OnEvent(Event newEvent)
        {
            if (newEvent is TurnStart) OnTurnStart();
            else if (newEvent is Death) IsAlive = false;
        }

        private void OnTurnStart()
        {
            if (HasOverride) return;
            if (!IsAlive || IsSleeping) Actions.SkipAction(Actor);
            else if (Wanders && IsMobile)
            {
                if (UnityEngine.Random.value < 0.15) Actions.SkipAction(Actor);
                else if (TryWander()) TimeSystem.EndTurn(Actor);
                else Actions.SkipAction(Actor);
            }
            else Actions.SkipAction(Actor);


        }

        private bool TryWander()
        {
            if (LocomotionSystem.TryMoveLocal(Physics, DirectionHelper.GetRandom(), out int cost))
            {
                var E1 = new GetStat("WALK");
                EntityManager.FireEvent(entity, E1);

                if (E1.Value < 1) E1.Value = 1;
                int netCost = UnityEngine.Mathf.RoundToInt(LocomotionSystem.movementDividend / (E1.Value - cost));
                TimeSystem.SpendActionPoints(Actor, netCost);
                return true;
            }
            return false;
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