namespace JS.ECS
{
    public class Description : ComponentBase
    {
        public Description() { }

        public string Value = "An unremarkable object.";
        
        public override void OnEvent(Event newEvent)
        {
            //
        }
    }
}

