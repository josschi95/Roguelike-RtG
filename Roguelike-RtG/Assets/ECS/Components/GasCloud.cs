namespace JS.ECS
{
    public class GasCloud : ComponentBase
    {
        public int gasID;
        public int volume;
        public Transform transform;

        public GasCloud(int gasID, int volume, Transform transform)
        {
            this.gasID = gasID;
            this.volume = volume;
            this.transform = transform;
            GasSystem.Register(this);
        }

        public override void Disassemble()
        {
            base.Disassemble();
            GasSystem.Unregister(this);
        }
    }
}