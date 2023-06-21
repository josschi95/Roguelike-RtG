using JS.ECS;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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

        #region - Hands -
        /// <summary>
        /// Automatically equip item in the first available slot.
        /// </summary>
        public static bool TryAutoEquipItem(Body body, Entity item)
        {
            return instance.TryAutoEquipNewItem(body, item);
        }

        private bool TryAutoEquipNewItem(Body body, Entity item)
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

        /// <summary>
        /// Equip item in the given slot.
        /// </summary>
        public static bool TryEquipItem(Entity parent, BodyPart part, Entity item)
        {
            return instance.TryEquipNewItem(parent, part, item);
        }

        private bool TryEquipNewItem(Entity parent, BodyPart part, Entity item)
        {
            if (!part.IsGrasper) return false;
            if (part.HeldItem != null) return false;

            part.HeldItem = item;
            EntityManager.GetComponent<Inventory>(parent).Contents.Remove(item);
            Debug.Log("Equipping " + item.Name + " in " + part.Name);
            return true;
        }

        public static void UnequipItem(Entity parent, BodyPart part)
        {
            instance.UnequipNewItem(parent, part);
        }

        private void UnequipNewItem(Entity parent, BodyPart part)
        {
            if (part == null || part.HeldItem == null) return;

            var item = part.HeldItem;
            part.HeldItem = null;
            EntityManager.GetComponent<Inventory>(parent).Contents.Add(item);
            Debug.Log("Unequipping " + item.Name + " from " + part.Laterality + " " + part.Name);
        }
        #endregion

        #region - Missile & Projectiles -
        public static bool TryEquipMissile(Body body,  Entity missile)
        {
            return instance.TryEquipNewMissile(body, missile);
        }

        public static bool TryEquipProjectile(Body body, Entity projectile)
        {
            return instance.TryEquipNewProjectile(body, projectile);
        }

        private bool TryEquipNewMissile(Body body, Entity missile)
        {
            if (body.MissileWeapon != null) return false;
            if (!EntityManager.TryGetComponent<MissileWeapon>(missile, out _)) return false;

            body.MissileWeapon = missile;
            EntityManager.GetComponent<Inventory>(body.entity).Contents.Remove(missile);
            Debug.Log("Equipping " + missile.Name + " in MissileWeapon slot");
            return true;
        }

        private bool TryEquipNewProjectile(Body body, Entity projectile)
        {
            if (body.Projectiles != null) return false;
            //Probably will end up having some Ammo tag or component, check for that

            body.Projectiles = projectile;
            EntityManager.GetComponent<Inventory>(body.entity).Contents.Remove(projectile);
            Debug.Log("Equipping " + projectile.Name + " in Projectiles slot");
            return true;
        }

        public static void UnequipRanged(Body body, Entity item)
        {
            instance.UnequipOldRanged(body, item);
        }

        private void UnequipOldRanged(Body body, Entity item)
        {
            if (body == null  || item == null) return;

            if (item == body.MissileWeapon)
            {
                body.MissileWeapon = null;
                EntityManager.GetComponent<Inventory>(body.entity).Contents.Add(item);
                Debug.Log("Unequipping " + item.Name + " from Missile slot");
            }
            else if (item == body.Projectiles)
            {
                body.Projectiles = null;
                EntityManager.GetComponent<Inventory>(body.entity).Contents.Add(item);
                Debug.Log("Unequipping " + item.Name + " from Projectile slot");
            }
        }
        #endregion

        #region - Armor -
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

        public static void UnequipArmor(Body body, ArmorSlot slot)
        {
            instance.UnequipOldArmor(body, slot);
        }

        private void UnequipOldArmor(Body body, ArmorSlot oldSlot)
        {
            if (body == null ||  oldSlot == null || oldSlot.Armor == null) return;

            var armor = oldSlot.Armor;
            oldSlot.Armor = null;
            EntityManager.GetComponent<Inventory>(body.entity).Contents.Add(armor);
            Debug.Log("Unequipping " + armor.Name + " in " + oldSlot.Slot.ToString() + " slot");
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