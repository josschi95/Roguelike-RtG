using JS.ECS;

namespace JS.ECS
{
    public class BodyPart// : ComponentBase
    {
        public BodyPart() { }

        public string Name; //Name of the body part
        public BodySlot Slot;
        public bool Integral = false; //true for body
        public bool IsVital = false; //true for heads, does dismembering this component cause death?

        public BodySlot UsuallyOn = BodySlot.Body;
        public bool IsAppendage = true; //not true for Thorax, Abdomen, Back
        public BodyPart AttachedTo;

        public Laterality Laterality = Laterality.None;

        public string LimbBlueprint; //Name of the blueprint for the dismembered body part

        public bool AverageArmor = false; //true for heads, arms, hands, feet
        
        public Entity DefaultBehavior; //fists/claws, beaks/jaws, etc.
        public Entity WeaponOverride; //wielded weapons
        public bool Grasper = false;

        public ArmorSlot[] Armor;

        public void OnEvent(Event newEvent)
        {
            if (newEvent is GetMeleeAttacks getAttacks)
            {
                if (WeaponOverride != null)
                {
                    getAttacks.weapons.Add(EntityManager.GetComponent<MeleeWeapon>(WeaponOverride));
                }
                else if (DefaultBehavior != null)
                {
                    getAttacks.weapons.Add(EntityManager.GetComponent<MeleeWeapon>(DefaultBehavior));
                }
            }
        }

        public void OnDismembered()
        {

        }
    }
}

public class ArmorSlot
{
    public BodySlot BodySlot;
    public Entity Armor;

    public ArmorSlot(BodySlot bodySlot)
    {
        BodySlot = bodySlot;
    }
}

/*public BodyPart(string name, Laterality laterality, BodyPart attachedTo, bool integral = false, bool isAppendage = true, bool averageArmor = false)
{
    Name = name;
    Laterality = laterality;
    AttachedTo = attachedTo;

    Integral = integral;
    IsAppendage = isAppendage;
    AverageArmor = averageArmor;
}*/