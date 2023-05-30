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

        public int ActionPoints;
        public int Speed;

        public TimedActor(int speed = 100)
        {
            Speed = speed;
            TimeSystem.Register(this);
        }

        public override void Disassemble()
        {
            entity = null;
            onTurnStart = null;
            onTurnEnd = null;
            TimeSystem.Unregister(this);
        }

        public override void FireEvent(Event newEvent)
        {
            //
        }
    }
}