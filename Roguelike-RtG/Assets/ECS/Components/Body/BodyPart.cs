using UnityEngine;

namespace JS.ECS
{
    public class BodyPart// : ComponentBase
    {
        public string Name; //Name of the body part
        public BodyPartType Type;

        public bool IsAppendage = true; //not true for Body, determines if it can be dismembered
        public bool IsVital = false; //true for heads, does dismembering this component cause death?

        public BodyPartType UsuallyOn = BodyPartType.Body;
        public BodyPart AttachedTo; //if the AttachedTo BodyPart is dismembered, this body part and its children are lost as well

        public Laterality Laterality = Laterality.None;

        public string LimbBlueprint; //Name of the blueprint for the dismembered body part

        public bool AverageArmor = false; //true for heads, arms, hands, feet
        
        public Entity DefaultBehavior; //fists/claws, beaks/jaws, etc.
        public Entity WeaponOverride; //wielded weapons
        public bool Grasper = false; //Can the body part hold items/weapons

        public ArmorSlot[] Armor;

        public void OnEvent(Event newEvent)
        {
            if (newEvent is GetMeleeAttacks getAttacks) OnGetMeleeAttacks(getAttacks);
        }

        private void OnGetMeleeAttacks(GetMeleeAttacks attacks)
        {
            if (WeaponOverride != null)
            {
                attacks.weapons.Add(EntityManager.GetComponent<MeleeWeapon>(WeaponOverride));
            }
            else if (DefaultBehavior != null)
            {
                attacks.weapons.Add(EntityManager.GetComponent<MeleeWeapon>(DefaultBehavior));
            }
        }
    }
}
//So I'm going to need to make some changes here
//Primary Limb should have a 100% chance to attack, whereas each subsequent attack should have a lower chance
//Or.... should only the primary limb make an attack if it doesn't have a weapon?
//That works for two handed but not for things like tails, tentacles, etc. 
//So yeah I think the ChanceToAttack is of course the better method