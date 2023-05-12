namespace JS.ECS
{
    public class TimeSystem : SystemBase<TimedActor>
    {
        public delegate void OnTimeProgressCallback();
        public OnTimeProgressCallback onTick;
        public OnTimeProgressCallback onNewRound;

        private TimedActor sentinel;
        public const int pointsToAct = 1000;
        private bool runTurnCounter = true;

        public TimeSystem()
        {
            var entity = new Entity();
            var sentinel = new TimedActor();
            entity.AddComponent(sentinel);
            this.sentinel = sentinel;

            TurnTransition();
        }

        private void OnNewRound()
        {
            onNewRound?.Invoke();
            Action.SkipAction(sentinel);
        }

        public void Pause()
        {
            runTurnCounter = false;
        }

        public void Resume()
        {
            runTurnCounter = true;
            TurnTransition();
        }

        private void TurnTransition()
        {
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
        }

        private void StartTurn(TimedActor actor)
        {
            actor.HasActed = false;
            actor.onTurnChange?.Invoke(true);

            if (actor == sentinel) OnNewRound();

            while (!actor.HasActed)
            {
                //Wait
            }

            EndTurn(actor);
        }

        private void EndTurn(TimedActor actor)
        {
            actor.onTurnChange?.Invoke(false);
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