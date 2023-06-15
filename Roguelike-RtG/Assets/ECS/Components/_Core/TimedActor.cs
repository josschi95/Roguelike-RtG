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
                if (EntityManager.TryGetStat(entity, "Speed", out var stat))
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
            switch (newEvent)
            {
                case TurnStart:
                    IsTurn = true; 
                    break;
                case TurnEnd: 
                    IsTurn = false; 
                    break;
                case Death: 
                    TimeSystem.Unregister(this); 
                    break;
            }
        }
    }
}