namespace JS.ECS
{
    public class ObjectStack : ComponentBase
    {
        public int MaxStack { get; private set; } = 1;
        public int Count = 1;
    }
}