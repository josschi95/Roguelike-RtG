namespace JS.ECS
{
    public class TimedActor : ComponentBase
    {
        public delegate void OnTurnChangeCallback();
        public OnTurnChangeCallback onTurnStart;
        public OnTurnChangeCallback onTurnEnd;

        private bool isTurn = false;
        public bool IsTurn
        {
            get => isTurn;
            set
            {
                isTurn = value;
                if (isTurn) onTurnStart?.Invoke();
                else onTurnEnd?.Invoke();
            }
        }

        public int ActionPoints = 0;
        public int Speed = 100;

        public TimedActor(Entity entity)
        {
            this.entity = entity;
            this.Speed = 100;
            TimeSystem.Register(this);
        }

        public override void Release()
        {
            entity = null;
            TimeSystem.Unregister(this);
        }
    }
}