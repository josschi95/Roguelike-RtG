namespace JS.ECS
{
    public class TimedActor : ComponentBase
    {
        public delegate void OnTurnChangeCallback(bool isTurn);
        public OnTurnChangeCallback onTurnChange;

        public bool HasActed = false;
        public int ActionPoints = 0;
        public int Speed = 100;

        public TimedActor() => TimeSystem.Register(this);

        public override void Release()
        {
            entity = null;
            TimeSystem.Unregister(this);
        }
    }
}