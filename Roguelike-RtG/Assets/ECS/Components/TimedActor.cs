namespace JS.ECS
{
    public class TimedActor : ComponentBase
    {
        public delegate void OnTurnChangeCallback(bool isTurn);
        public OnTurnChangeCallback onTurnChange;

        private bool hasActed = false;
        public bool HasActed
        {
            get => hasActed;
            set
            {
                hasActed = value;
                onTurnChange?.Invoke(!hasActed);
            }
        }

        public int ActionPoints = 0;
        public int Speed = 100;

        public TimedActor(Entity entity)
        {
            this.entity = entity;
            TimeSystem.Register(this);
        }

        public override void Release()
        {
            entity = null;
            TimeSystem.Unregister(this);
        }
    }
}