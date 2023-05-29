namespace JS.ECS
{
    /// <summary>
    /// Component for all objects that can be traded
    /// </summary>
    public class Commerce : ComponentBase
    {
        public float Value;

        public Commerce(float value = 0.01f)
        {
            Value = value;
        }

        public override void FireEvent(Event newEvent)
        {
            //
        }
    }
}