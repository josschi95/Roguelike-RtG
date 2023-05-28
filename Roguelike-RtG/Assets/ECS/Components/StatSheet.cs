using JS.CharacterSystem;
using System.Collections.Generic;

namespace JS.ECS.Stats
{
    public class StatSheet : ComponentBase
    {
        public List<StatBase> Stats { get; private set; }

        public StatSheet() 
        { 
            Stats = new List<StatBase>();
        }
    }
}