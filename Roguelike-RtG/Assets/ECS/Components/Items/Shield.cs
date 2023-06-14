namespace JS.ECS
{
    public class Shield : ComponentBase
    {
        public Shield() { }

        public int AV = 1;
        public int DV = 0;
        public int BlockChance = 25;

        public override void OnEvent(Event newEvent)
        {
            //
        }
    }
}