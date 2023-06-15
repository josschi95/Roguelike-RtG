namespace JS.ECS
{
    public class EquipmentSlot : ComponentBase
    {
        public EquipmentSlot() { }

        public BodySlot Type;
        public Entity EquippedArmor;
    }
}