namespace JS.ECS
{
    public class Armor : ComponentBase
    {
        public Armor() { }

        public int AV = 1;
        public int DV = 0;
        public EquipmentSlot Slot;

        public override void OnEvent(Event newEvent)
        {
            if (newEvent is GetStat stat) OnGetStat(stat);
        }

        private void OnGetStat(GetStat stat)
        {
            if (stat.Name.Equals("ArmorValue") || stat.Name.Equals("AV")) stat.Value += AV;
            else if (stat.Name.Equals("DodgeValue") || stat.Name.Equals("DV")) stat.Value += DV;
        }
    }
}