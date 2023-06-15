namespace JS.ECS
{
    /// <summary>
    /// Component to attach to creatures, has chance to drop their corpse on death.
    /// </summary>
    public class Corpse : ComponentBase
    {
        public Corpse() { }

        public int CorpseChance = 100;
        public string CorpseBlueprint = "GenericCorpse";
    }
}