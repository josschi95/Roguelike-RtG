namespace JS.ECS
{
    public class TimedActor : ComponentBase
    {
        public bool IsTurn { get; private set; }
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
            TimeSystem.Unregister(this);
        }

        public override void FireEvent(Event newEvent)
        {
            if (newEvent is TurnStart) IsTurn = true;
            else if (newEvent is TurnEnd) IsTurn = false;
        }
    }
}