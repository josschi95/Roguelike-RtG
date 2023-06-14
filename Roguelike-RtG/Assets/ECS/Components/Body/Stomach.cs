namespace JS.ECS
{
    public class Stomach : ComponentBase
    {
        public int MaxSatiation = 1000;
        public int MaxHydration = 1000;

        public int Satiation = 1000;
        public int Hydration = 1000;

        public HungerState Hunger;
        public ThirstState Thirst;

        public Stomach()
        {
            StomachSystem.Register(this);
        }

        public override void OnRegistered()
        {
            base.OnRegistered();
        }

        public override void OnEvent(Event newEvent)
        {
            //
        }

        public override void Disassemble()
        {
            StomachSystem.Unregister(this);
            base.Disassemble();
        }
    }
}