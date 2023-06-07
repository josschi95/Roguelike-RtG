namespace JS.ECS
{
    public class TimedActor : ComponentBase
    {
        public bool IsTurn { get; private set; } = false;
        public int ActionPoints = 0;

        public int Speed
        {
            get
            {
                if (entity.TryGetStat("Speed", out var stat))
                {
                    return stat.Value;
                }
                return 100;
            }
        }

        public TimedActor() { }

        public override void OnRegistered()
        {
            TimeSystem.Register(this);
        }

        public override void Disassemble()
        {
            entity = null;
            TimeSystem.Unregister(this);
        }

        public override void OnEvent(Event newEvent)
        {
            if (newEvent is TurnStart) IsTurn = true;
            else if (newEvent is TurnEnd) IsTurn = false;
        }
    }
}