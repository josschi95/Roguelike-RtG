namespace JS.ECS
{
    /// <summary>
    /// Component that grants experience upon objects destruction
    /// </summary>
    public class Experience : ComponentBase
    {
        public int Amount;

        public override void OnEvent(Event newEvent)
        {
            //
        }
    }
}