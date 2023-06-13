using System;

namespace JS.ECS
{
    public class Entity
    {
        public Guid ID { get; private set; }
        public string Name;

        public Entity(string name = "Entity")
        {
            ID = Guid.NewGuid();
            Name = name;
        }
    }
}