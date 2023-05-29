using System.Collections.Generic;

namespace JS.ECS
{
    /// <summary>
    /// A Component to add to creatures to determine anatomical layout and capabilities. Holds reference to all extremities and 
    /// </summary>
    public class Body : ComponentBase
    {
        public Anatomy Anatomy;

        public Body(Anatomy anatomy)
        {
            Anatomy = anatomy;
        }

        public override void FireEvent(Event newEvent)
        {
            //
        }

        //IDK what the type will be, but this is used to reference all held objects and all takeable actions
        //Probably a Dictionary of some sort or a paired list ,where each grabber can hold 1 item, or multiple grabbers are 
        //required for a single item
        public List<Entity> Appendages = new List<Entity>();

        //So all equippable items inherit from Armor
        //Armor inherits from Item
        //Weapon should also inherit from Item
        //limbs supposedly inherit from Corpse but I think that's severed limbs

        public List<Physics> Heads = new List<Physics>();
        public List<Physics> Arms = new List<Physics>();
        public List<Physics> Hands = new List<Physics>();
        public List<Physics> Legs = new List<Physics>();
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

public enum BodySlot
{
    Head,
    Body,
    Back,
    Tail,
    Arm,
    Hand,
    Foot,
}