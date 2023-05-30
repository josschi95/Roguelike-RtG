namespace JS.ECS
{
    public class Armor : ComponentBase
    {
        public int ArmorValue;
        public int DodgeValue;
        public ArmorSlots Slot;

        public override void FireEvent(Event newEvent)
        {
            //
        }
    }
}