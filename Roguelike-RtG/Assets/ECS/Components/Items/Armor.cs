namespace JS.ECS
{
    public class Armor : ComponentBase
    {
        public Armor() { }

        public Armor(int armorValue, int dodgeValue, BodySlot slot)
        {
            ArmorValue = armorValue;
            DodgeValue = dodgeValue;
            Slot = slot;
        }

        public int ArmorValue;
        public int DodgeValue;
        public BodySlot Slot;

        public override void OnEvent(Event newEvent)
        {
            if (newEvent is GetStat stat) OnGetStat(stat);
        }

        private void OnGetStat(GetStat stat)
        {
            if (stat.Name.Equals("ArmorValue") || stat.Name.Equals("AV")) stat.Value += ArmorValue;
            else if (stat.Name.Equals("DodgeValue") || stat.Name.Equals("DV")) stat.Value += DodgeValue;
        }
    }
}