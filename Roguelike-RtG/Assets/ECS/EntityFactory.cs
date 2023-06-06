namespace JS.ECS
{
    public class EntityFactory
    {
        public static void LoadBlueprints()
        {

        }

        public static Entity CreateEntity(string blueprint)
        {
            return new Entity(blueprint);
        }
    }
}

