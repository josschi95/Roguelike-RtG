namespace JS.ECS
{
    public class Food : ComponentBase
    {
        public Food() { }

        public int Satiation = 0;
        public int Hydration = 0;

        public bool IsGross = false;
        public bool IllOnEat = false;

        public string Message; //Message on eating

        public override void OnEvent(Event newEvent)
        {
            
        }
    }
}