namespace JS.ECS
{
    public class CraftingItem : ComponentBase
    {
        public CraftingItem() { }

        public CraftingSkill Skill;
        public int Difficulty = 10;
        public int RoundsToCraft = 1;

        public override void OnEvent(Event newEvent)
        {
            
        }
    }
}

public enum CraftingSkill
{
    Alchemy,
    Smithing,
    Engineering,
}