using JS.ECS;
using System.Collections.Generic;
using UnityEngine;

//Is it worth considering merging the CorpseManager into this system?

namespace JS.ECS
{
    public class BodySystem : MonoBehaviour
    {
        private static BodySystem instance;

        private List<Body> bodies;

        private List<BodyPart> dismemberableParts;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;
            bodies = new List<Body>();
            dismemberableParts = new List<BodyPart>();
        }

        public static void Register(Body newBody)
        {
            if (!instance.bodies.Contains(newBody))
                instance.bodies.Add(newBody);
        }

        public static void Unregister(Body oldBody)
        {
            instance.bodies.Remove(oldBody);
            //I definitely thing I need to move the corpseManager functions to this as well, 
            //I suppose the corpse should also hold reference to which body parts are present on it, in the event of pre-death dismemberment
            //so basically have a string list for all body parts which can be harvested from the corpse

            //Drop all equipment and add it to an inventory on the corpse
            //Clear all lists
        }

        #region - Equipment -
        public static bool TryEquipArmor(Body body, Armor armor)
        {
            return instance.TryEquipNewArmor(body, armor);
        }

        private bool TryEquipNewArmor(Body body, Armor armor)
        {
            for (int i = 0; i < body.ArmorSlots.Count; i++)
            {
                if (body.ArmorSlots[i].Slot != armor.Slot) continue; //Need to find matching type
                if (body.ArmorSlots[i].Armor != null) continue; //Need to unequp existing item first

                //Remove item from inventory
                EntityManager.GetComponent<Inventory>(body.entity).Contents.Remove(armor.entity);
                body.ArmorSlots[i].Armor = armor.entity; //place int armor slot
                return true;
            }
            return false;
        }

        public static bool TryEquipItem(Body body, Entity item)
        {
            return instance.TryEquipNewItem(body, item);
        }

        private bool TryEquipNewItem(Body body, Entity item)
        {
            for (int i = 0; i < body.BodyParts.Count; i++)
            {
                if (!body.BodyParts[i].IsGrasper) continue; //Needs to be a body part that can hold items (hands)
                if (body.BodyParts[i].HeldItem != null) continue; //grasper needs to be empty

                EntityManager.GetComponent<Inventory>(body.entity).Contents.Remove(item);
                body.BodyParts[i].HeldItem = item;
                Debug.Log("Equipping " + item.Name + " in " + body.BodyParts[i].Name);
                return true;
            }
            return false;
        }

        public static void UnequipArmor(ArmorSlot slot)
        {
            if (slot.Armor == null) return;


        }

        #endregion
        private void OnDismemberment(Body body)
        {
            var part = FindDismemberableBodyPart(body);
            if (part == null) return;

            //EntityManager.FireEvent(body.entity, new Dismem)
        }

        /// <summary>
        /// Loops through all body parts and makes a list of which can be dismembered
        /// </summary>
        private BodyPart FindDismemberableBodyPart(Body body)
        {
            dismemberableParts.Clear();
            bool atLeastOneVital = false; //heads can be dismembered but only if there is more than one
            for (int i = 0; i < body.BodyParts.Count; i++)
            {
                if (!body.BodyParts[i].IsAppendage) continue; //non-appendages (body) cannot be dismembered
                if (body.BodyParts[i].IsVital && !atLeastOneVital) //keep at least one vital (head) part
                {
                    atLeastOneVital = true;
                    continue;
                }

                dismemberableParts.Add(body.BodyParts[i]);
            }
            if (dismemberableParts.Count == 0) return null;

            int index = Random.Range(0, dismemberableParts.Count);
            return dismemberableParts[index];
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