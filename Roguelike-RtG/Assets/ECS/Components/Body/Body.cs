using System.Collections.Generic;
using JS.ECS;

namespace JS.ECS
{
    /// <summary>
    /// A component that represents anatomical layout of a crature and holds reference to all of its body part entities 
    /// </summary>
    public class Body : ComponentBase
    {
        public Body() { }

        public string Anatomy = "Humanoid";
        
        public List<BodyPart> BodyParts { get; set; }
        public BodyPart PrimaryLimb;
        public override void OnRegistered() => BodyFactory.CreateAnatomy(this);

        public override void OnEvent(Event newEvent)
        {
            for (int i = 0; i < BodyParts.Count; i++)
            {
                BodyParts[i].OnEvent(newEvent);
            }
        }
    }
}

public enum Laterality
{
    None, 
    Left,
    Right,
}

public enum BodyPartType
{
    Head,
    Body,
    Arm,
    Hand,
    //Leg,
    Feet,

    Tail,
    Wings,
}

public enum EquipmentSlot
{
    Head,   //Helms, Hoods, Hats, Caps, Crowns, Masks
    Eyes,   //Glasses, Blindfolds, Goggles, 
    Neck,   //Amulets, Necklaces, Medallions
    Body,   //Armor, Robes, 
    Back,   //Capes, Cloaks, Mantles, ?Wings
    Arm,    //Bracers, Bucklers, Manacles, Shackles, Bracelets (if wrists)
    Hand,   //Gauntlets, Gloves
    Ring,   //Rings
    Belt,   //Belts, Girdles, Bandoliers
    Feet,   //Boots, Sandles, Shoes

    Tail,   //No equipment
    Wings,  //No equipment
}

public class ArmorSlot
{
    public EquipmentSlot BodySlot;
    public Entity Armor;

    public ArmorSlot(EquipmentSlot bodySlot)
    {
        BodySlot = bodySlot;
    }
}