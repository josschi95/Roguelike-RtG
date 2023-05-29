namespace JS.ECS
{
    /// <summary>
    /// Component to handle basic behavior for sentient creatures
    /// </summary>
    public class Brain : ComponentBase
    {
        public bool IsHostile = false; //Is the object hositle by default?
        public bool IsCalm = false; //Will the object become hostile if attacked?
        public bool IsMobile = true; //Can the object move?
        public bool Wanders = true; //Does the object wander when not engaged?
        public Habitat Habitat = Habitat.Terrestrial; //Can the object go on land/water? 
        public string Faction; //What faction does the object belong to?
        public int FactionDisposition; //What is the faction's dispositio towards this object

        //Other CoQ properties
        //public int MaxWanderDist = 5;
        //public bool WandersRandomly = false;
        //public bool LivesOnWalls = false;
        //public int MinKillRadius;
        //public int MaxKillRadius;
        //public bool Hibernating = true;?
        //public bool PointBlankRange = false;

        //Other possible properties
        //public int/enum Rationality
        //public int/enum DecisionMaking

        public override void FireEvent(Event newEvent)
        {
            //
        }
    }
}

public enum Habitat
{
    Terrestrial,
    Aquatic,
    Amphibious,

}