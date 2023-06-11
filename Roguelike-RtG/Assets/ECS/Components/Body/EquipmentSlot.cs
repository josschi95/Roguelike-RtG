namespace JS.ECS
{
    public class EquipmentSlot : ComponentBase
    {
        public EquipmentSlot() { }

        public BodySlot Type;
        public Entity EquippedArmor;


        public override void OnEvent(Event newEvent)
        {
            
        }

        public override void Disassemble()
        {
            base.Disassemble();
        }
    }
}