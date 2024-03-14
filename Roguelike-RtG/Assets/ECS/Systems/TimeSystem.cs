using System.Collections;
using UnityEngine;
using JS.Architecture.EventSystem;

namespace JS.ECS
{
    public class TimeSystem : SystemBase<TimedActor>
    {
        public const int pointsToAct = 1000;
        public const float AnimationTime = 0.2f;

        private static TimeSystem instance;


        public delegate void OnTimeProgressCallback();
        public OnTimeProgressCallback onTick;
        public OnTimeProgressCallback onNewRound;

        private bool runTurnCounter = true;
        private TimedActor sentinel;

        private NewRound newRoundEvent;
        private TurnStart turnStartEvent;
        private TurnEnd turnEndEvent;

        [SerializeField] private GameEvent gameTickEvent;

        private AnimationMode _animationMode;
        public static AnimationMode AnimMode => instance._animationMode;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this); 
                return;
            }
            instance = this;

            newRoundEvent = new NewRound();
            turnStartEvent = new TurnStart();
            turnEndEvent = new TurnEnd();
        }

        private IEnumerator Start()
        {
            CreateSentinelt();

            while (components.Count <= 1) yield return null;

            TurnTransition();
        }

        private void CreateSentinelt()
        {
            var entity = EntityManager.NewEntity("Sentinel");
            sentinel = EntityManager.AddComponent(entity, new TimedActor()) as TimedActor;
        }

        private void OnNewRound()
        {
            for (int i = 0; i < components.Count; i++)
            {
                EntityManager.FireEvent(components[i].entity, newRoundEvent);
            }

            //Debug.Log("OnNewRound");
            onNewRound?.Invoke();
            if (_animationMode == AnimationMode.Instant) StartCoroutine(RoundDelay());
            else Actions.SkipAction(sentinel);
        }

        //Adds a slight delay when rounds are instant to avoid stack overflows
        private IEnumerator RoundDelay()
        {
            yield return new WaitForSeconds(0.1f);
            Actions.SkipAction(sentinel);
        }

        public static void Pause() => instance.PauseTime();
        public static void Resume() => instance.ResumeTime();

        private void PauseTime() => runTurnCounter = false;
        private void ResumeTime()
        {
            runTurnCounter = true;
            TurnTransition();
        }

        private void TurnTransition()
        {
            //Debug.Log("TurnTransition");
            if (!runTurnCounter) return;
            GetNextTurn();
        }

        private void GetNextTurn()
        {
            while (GetNextActor() == null)
            {
                Tick();
            }
            StartTurn(GetNextActor());
        }

        private TimedActor GetNextActor()
        {
            TimedActor nextActor = null;

            for (int i = 0; i < components.Count; i++)
            {
                //Debug.Log(components[i].entity.Name + " AP: " + components[i].ActionPoints);
                if (EntityIsFaster(components[i], nextActor))
                {
                    nextActor = components[i];
                }
            }
            
            return nextActor;
        }

        private bool EntityIsFaster(TimedActor actor, TimedActor comparedActor)
        {
            if (actor.ActionPoints < pointsToAct) return false;
            if (comparedActor == null) return true;

            if (actor.ActionPoints < comparedActor.ActionPoints) return false;
            if (actor.ActionPoints > comparedActor.ActionPoints) return true;
            if (actor.Speed > comparedActor.Speed) return true;

            return false; //tie goes to lower in index
        }

        private void Tick()
        {
            for (int i = 0; i < components.Count; i++)
            {
                RegainActionPoints(components[i]);
            }
            onTick?.Invoke();
            gameTickEvent?.Invoke();
        }

        private void StartTurn(TimedActor actor)
        {
            //Debug.Log("Start Turn: " + actor.entity.Name);
            //Debug.Log(components.Count);

            //This should only be true for the player actor
            if (_animationMode == AnimationMode.Quick && actor.TurnDelay == true)
            {
                //Have a short delay prior to the player acting, for all other animations to play simultaneously

                //Note that this currently will not work because the way that I'm checking to see if I should smooth an anim 
                //is by seeing if TurnDelay is set to true.... 
                StartCoroutine(QuickAnimDelay(actor));
            }
            else
            {
                EntityManager.FireEvent(actor.entity, turnStartEvent);
                if (actor == sentinel) OnNewRound();
            }

        }

        private IEnumerator QuickAnimDelay(TimedActor actor)
        {
            yield return new WaitForSeconds(AnimationTime);
            EntityManager.FireEvent(actor.entity, turnStartEvent);
        }

        public static void EndTurn(TimedActor actor)
        {
            instance.EndActorTurn(actor);
        }

        private void EndActorTurn(TimedActor actor)
        {
            EntityManager.FireEvent(actor.entity, turnEndEvent);
            if (actor.TurnDelay) StartCoroutine(TurnDelay());
            else TurnTransition();
        }

        private IEnumerator TurnDelay()
        {
            yield return new WaitForSeconds(AnimationTime);
            TurnTransition();
        }

        public static void SpendActionPoints(TimedActor actor, int points = pointsToAct)
        {
            //Debug.Log(actor.entity.Name + " Spending " + points + " AP");
            actor.ActionPoints -= points;
        }

        private void RegainActionPoints(TimedActor actor)
        {
            //Debug.Log(actor.entity.Name + " regaining AP: " + actor.Speed);
            actor.ActionPoints += actor.Speed;
        }

        //Called from GameSettings
        public static void OnAnimationChange(AnimationMode animationMode)
        {
            instance.OnAnimationChanged(animationMode);
        }

        private void OnAnimationChanged(AnimationMode animationMode)
        {
            if (_animationMode == animationMode) return;
            _animationMode = animationMode;

            switch (_animationMode)
            {
                case AnimationMode.Instant:
                    foreach (var component in components) component.TurnDelay = false;
                    return; //Exit early so I'm not setting player back to true
                case AnimationMode.Quick:
                    foreach (var component in components) component.TurnDelay = false;
                    break;
                case AnimationMode.Staggered:
                    //This one might be tricky. I'll have to loop through basically all entities and see if they're in active combat
                    //Specifically with the player, this might be some sort of CombatSystem thing, we'll see.
                    Debug.LogWarning("Not fully implemented switching back");
                    break;
            }

            if (EntityManager.Player != null)
            {
                EntityManager.GetComponent<TimedActor>(EntityManager.Player).TurnDelay = true;
            }
        }
    }
}