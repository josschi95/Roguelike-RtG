namespace JS.ECS
{
    /// <summary>
    /// A Component to add to creatures to determine anatomical layout and capabilities
    /// </summary>
    public class Body : ComponentBase
    {
        public Anatomy Anatomy;
    }
}

public enum Anatomy
{
    Humanoid,
    TailedHumanoid,
    Quadruped,
    Centauroid,
    Avian,
    Ooze,
    Insectoid,
    Arachnid,
    Gastropod,
    Eyeball,
    Dragon,
    Fungus,
    Tree,

}