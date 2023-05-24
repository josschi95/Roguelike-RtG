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
            Debug.Log("TimeSystem Init");
            var entity = new Entity();
            var sentinel = new TimedActor(entity);
            sentinel.Speed = 100;
            this.sentinel = sentinel;
            this.sentinel.onTurnStart += OnNewRound;

            while(components.Count <= 1) yield return null;

            TurnTransition();
        }

        private void OnNewRound()
        {
            Debug.Log("OnNewRound");
            onNewRound?.Invoke();
            Action.SkipAction(sentinel);
        }

        public void Pause() => instance.PauseTime();
        public void Resume() => instance.ResumeTime();

        private void PauseTime() => runTurnCounter = false;
        private void ResumeTime()
        {
            runTurnCounter = true;
            TurnTransition();
        }

        private void TurnTransition()
        {
            Debug.Log("TurnTransition");
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
            actor.ActionPoints -= points;
        }

        private void RegainActionPoints(TimedActor actor)
        {
            actor.ActionPoints += actor.Speed;
        }
    }
}