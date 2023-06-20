using JS.ECS;

/// <summary>
/// Class to hold reference to armor and the body part it is attached to.
/// </summary>
public class ArmorSlot
{
    public EquipmentSlot Slot;
    public BodyPart Attachedto;
    public Entity Armor;

    public ArmorSlot(EquipmentSlot bodySlot, BodyPart attachedto)
    {
        Slot = bodySlot;
        Attachedto = attachedto;
    }

    public void OnEvent(Event newEvent)
    {

    }
}