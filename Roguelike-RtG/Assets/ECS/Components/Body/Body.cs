using System.Collections.Generic;

namespace JS.ECS
{
    /// <summary>
    /// A component that represents anatomical layout of a crature and holds reference to all of its body part entities 
    /// </summary>
    public class Body : ComponentBase
    {
        public Body() { }

        public string Anatomy = "Humanoid";

        public List<BodyPart> BodyParts;
        public List<ArmorSlot> ArmorSlots;

        public Entity MissileWeapon;
        public Entity Projectiles;

        public BodyPart PrimaryLimb;

        public override void OnRegistered() => BodyFactory.CreateAnatomy(this);
        public override void Disassemble() => BodySystem.Unregister(this);

        public override void OnEvent(Event newEvent)
        {
            for (int i = 0; i < BodyParts.Count; i++)
            {
                BodyParts[i].OnEvent(newEvent);
            }
            for (int i = 0; i < ArmorSlots.Count; i++)
            {
                ArmorSlots[i].OnEvent(newEvent);
            }
            if (MissileWeapon != null) EntityManager.FireEvent(MissileWeapon, newEvent);
            if (Projectiles != null) EntityManager.FireEvent(Projectiles, newEvent);
        }
    }
}