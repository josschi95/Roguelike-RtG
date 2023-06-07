namespace JS.ECS
{
    /// <summary>
    /// Component for all objects that can be traded
    /// </summary>
    public class Commerce : ComponentBase
    {
        public Commerce() { }

        public Commerce(float value = 0.01f)
        {
            Value = value;
        }

        public float Value;

        public override void OnEvent(Event newEvent)
        {
            //
        }
    }
}