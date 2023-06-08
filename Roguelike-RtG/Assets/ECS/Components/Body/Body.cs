using JS.ECS.Tags;
using System.Collections.Generic;

namespace JS.ECS
{
    /// <summary>
    /// A component that represents anatomical layout of a crature and holds reference to all of its body part entities 
    /// </summary>
    public class Body : ComponentBase
    {
        public List<Entity> NewBodyParts;




        public List<BodyPart> BodyParts;
        //public List<EquipmentSlot> EquipmentSlots; //these should be components
        public Entity defaultBehaviour;

        public Body()
        {

            BodyParts = new List<BodyPart> ();
            //EquipmentSlots = new List<EquipmentSlot> ();
        }

        public override void OnEvent(Event newEvent)
        {
            //
            for (int i = 0; i < BodyParts.Count; i++)
            {
                BodyParts[i].OnEvent(newEvent);
            }
        }

        public void AttachPart(BodyPart part)
        {
            BodyParts.Add(part);
        }

        //So all equippable items inherit from Armor
        //Armor inherits from Item
        //Weapon should also inherit from Item
        //limbs supposedly inherit from Corpse but I think that's severed limbs
    }

    /*
     * Ok so Equipment slots are going to be a Component
     * The question is.... is the body a separate entity
     * or do I just add that component
     * 
     * Each body part is its own entity, which the Body Component holds reference to
     * 
     * so do I have a single generic equipment slot which cna hold 
     */
}

public enum Laterality
{
    None, 
    Left,
    Right,
}

public enum EquipmentType
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