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
            //
        }
    }
}