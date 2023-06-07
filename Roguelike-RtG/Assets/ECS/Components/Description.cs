namespace JS.ECS
{
    public class Description : ComponentBase
    {
        public Description() { }

        public Description(string value = "An unremarkable object.")
        {
            Value = value;
        }

        public string Value;
        
        public override void OnEvent(Event newEvent)
        {
            //
        }
    }
}

