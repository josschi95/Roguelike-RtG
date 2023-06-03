namespace JS.ECS
{
    public class Description : ComponentBase
    {
        public string Value;

        public Description(string value = "An unremarkable object.")
        {
            Value = value;
        }

        public override void OnEvent(Event newEvent)
        {
            //
        }
    }
}

