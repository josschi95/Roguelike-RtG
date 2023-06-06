using JS.ECS.Tags;
using System.Collections.Generic;

namespace JS.ECS
{
    /// <summary>
    /// A Component to add to creatures to determine anatomical layout and capabilities. Holds reference to all extremities and 
    /// </summary>
    public class Body : ComponentBase
    {
        public Anatomy Anatomy;

        public List<BodyPart> BodyParts;
        public List<EquipmentSlot> EquipmentSlots;
        public Body(Anatomy anatomy)
        {
            Anatomy = anatomy;
            BodyParts = new List<BodyPart> ();
            EquipmentSlots = new List<EquipmentSlot> ();
        }

        public override void OnEvent(Event newEvent)
        {
            //
            for (int i = 0; i < BodyParts.Count; i++)
            {
                BodyParts[i].FireEvent(newEvent);
            }

        }

        public void AttachPart(BodyPart part)
        {
            BodyParts.Add(part);
            if (part.ArmorSlots == null) return;

            for (int i = 0; i < part.ArmorSlots.Length; i++)
            {
                EquipmentSlots.Add(new EquipmentSlot(part.ArmorSlots[i], part));
            }
        }

        //So all equippable items inherit from Armor
        //Armor inherits from Item
        //Weapon should also inherit from Item
        //limbs supposedly inherit from Corpse but I think that's severed limbs
    }

    public class EquipmentSlot
    {
        public ArmorSlots Slot;
        public BodyPart BodyPart;
        public Armor Item;

        public EquipmentSlot(ArmorSlots slot, BodyPart bodyPart)
        {
            Slot = slot;
            BodyPart = bodyPart;
        }
    }
}

public enum Anatomy
{
    Humanoid,           //1 head, 2 arms and legs
    TailedHumanoid,     //1 head, 2 arms and legs, 1 tail
    Quadruped,          //1 head, 4 legs
    Centauroid,         //1 head, 2 arms, 4 legs
    Avian,              //1 head, 2 legs (wings are back slot)
    Ooze,               //literally nothing
    Insectoid,          //this can be so many things I have no idea yet
    Arachnid,           //1 head, 8 legs
    Gastropod,          //1 head, 1 tail
    ArmedGastropod,     //1 head, 2 arms, 1 tail
    Eyeball,            //1 head? is that even separate?
    Dragon,             //1 head, 4 legs... wings go on back... so how is this different from Quadruped? I guess it would add by default?
}

/// <summary>
/// Note that body parts do not grant armor slots, they don't know about armor, but they are checked when trying to equip... but then how do I handle it when trying to see if I can equip?
/// 
/// </summary>
public enum BodyPartTypes
{
    Head,       //Appendage, usually on Thorax grants head equip slot, neck equip slot, eyes equip slot
    Trunk,     //Upper trunk, integral, grants body equip slot and shoulder equip slot
    Hips,    //Lower trunk, integral, grants belt equip slot? or Thorax
    Arm,        //Appendage, usually on Thorax, grants Arm equip slot
    Hand,       //Appendage, usually on Arm, grants Hand equip slot
    Leg,        //Appendage, usually on Abdomen
    Foot,       //Appendage, usually on Leg, grants Feet equip slot

    Tail,       //Appendage, usually on Abdomen
    Wings,      //Appendage, usually on Thorax
}

public enum Laterality
{
    None, 
    Left,
    Right,
}

public enum ArmorSlots
{
    Head,       //Helms, Hoods, Hats, Caps, Crowns, Masks
    Eyes,       //Glasses, Blindfolds, Goggles, 
    Neck,       //Amulets, Necklaces, Medallions
    Body,       //Armor, Robes, 
    Shoulders,  //Capes, Cloaks, Mantles, ?Wings
    Arms,       //Bracers, Bucklers, Manacles, Shackles, Bracelets (if wrists)
    Hands,      //Gauntlets, Gloves
    Ring,       //Rings
    Belt,       //Belts, Girdles, Bandoliers
    Feet,       //Boots, Sandles, Shoes
}

public enum WeaponSlots
{
    Hand,               //Unarmed, Pugilist Weapons, Blades, Axes, Blunt, Polearms, 
    MissileWeapon,      //Bows, Crossbows, Javelins, Throwing Axes/Knives/Clubs
}