namespace JS.ECS
{
    /// <summary>
    /// Component that allows an object to declare attacks against others
    /// </summary>
    public class Combat : ComponentBase
    {
        public bool hasMultiStrike; //removes major penalty to making multiple attacks

        public override void OnEvent(Event newEvent)
        {
            //
        }
    }
}