using System.Collections;
using UnityEngine;
using JS.EventSystem;

namespace JS.ECS
{
    public class TimeSystem : SystemBase<TimedActor>
    {
        private static TimeSystem instance;


        public delegate void OnTimeProgressCallback();
        public OnTimeProgressCallback onTick;
        public OnTimeProgressCallback onNewRound;

        private TimedActor sentinel;
        public const int pointsToAct = 1000;
        private bool runTurnCounter = true;

        [SerializeField] private GameEvent gameTickEvent;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this); 
                return;
            }
            instance = this;
        }

        private IEnumerator Start()
        {
            var entity = new Entity("Sentinel");

            var actor = new TimedActor();
            entity.AddComponent(actor);

            sentinel = actor;
            sentinel.onTurnStart += OnNewRound;

            while(components.Count <= 1) yield return null;

            TurnTransition();
        }

        private void OnNewRound()
        {
            Debug.Log("OnNewRound");
            onNewRound?.Invoke();
            StartCoroutine(RoundDelay());
        }

        private IEnumerator RoundDelay()
        {
            yield return new WaitForSeconds(0.2f);
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
            Debug.Log(actor.entity.Name);
            Debug.Log(components.Count);
            actor.IsTurn = true;
        }

        public static void EndTurn(TimedActor actor)
        {
            instance.EndActorTurn(actor);
        }

        private void EndActorTurn(TimedActor actor)
        {
            actor.IsTurn = false;
            TurnTransition();
        }

        public static void SpendActionPoints(TimedActor actor, int points = pointsToAct)
        {
            //Debug.Log(actor.entity.Name + " Spending " + points + " AP");
            actor.ActionPoints -= points;
        }

        private void RegainActionPoints(TimedActor actor)
        {
            actor.ActionPoints += actor.Speed;
        }
    }
}