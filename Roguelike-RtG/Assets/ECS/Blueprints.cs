using System;
using JS.CharacterSystem;
using JS.ECS.Tags;

namespace JS.ECS
{
    public static class Blueprints
    {

        public static Entity GetPhysicalObject(string name = "PhysicalObject")
        {
            var entity = new Entity(name);
            //Render
            entity.AddComponent(new Physics());
            entity.AddComponent(new MeleeWeapon());
            entity.AddComponent(new Commerce());
            entity.AddComponent(new Description());

            entity.AddStat(new StatBase("HitPoints", "HP", 10, 0, int.MaxValue, int.MaxValue));
            entity.AddStat(new StatBase("ArmorValue", "AV", 0, 0, 100, 100));
            entity.AddStat(new StatBase("DodgeValue", "DV", 0, 0, 100, 100));


            return entity;
        }

        public static Entity GetInorganicObject(string name = "InorganicObject")
        {
            var entity = GetPhysicalObject(name);

            entity.AddComponent(new Inorganic());

            entity.AddTag(new Gender("Neutral"));
            entity.AddTag(new Breakable());
            return entity;
        }

        public static Entity GetCreature(string name = "BaseCreature")
        {
            var entity = GetPhysicalObject(name);

            AddBody(entity);
            AddAttributes(entity);

            entity.AddComponent(new TimedActor());
            entity.AddComponent(new Brain());
            entity.AddComponent(new Inventory());
            entity.AddComponent(new Combat());
            entity.AddComponent(new DamageResistances(GetBaseResistances()));
            entity.AddStat(new StatBase("MoveSpeed", "MS", 100, 200, 1, 200));

            //Experience
            //Skills
            //Spells
            //Proficiencies
            //Abilities
            //RandomLoot

            return entity;
        }

        public static Entity GetItem(string name = "Item")
        {
            var entity = GetInorganicObject(name);
            entity.AddComponent(new ObjectStack());

            entity.AddTag(new Item());

            return entity;
        }

        public static Entity GetMeleeWeapon(string name = "MeleeWeapon")
        {
            var entity = GetItem(name);
            var physics = entity.GetComponent<Physics>();
            physics.Category = PhysicsCategory.MeleeWeapon;
            physics.Weight = 5;


            return entity;
        }

        public static Entity GetMissileWeapon(string name = "MissileWeapon")
        {
            var entity = GetItem(name);
            var physics = entity.GetComponent<Physics>();
            physics.Category = PhysicsCategory.MissileWeapon;
            physics.Weight = 5;


            return entity;
        }

        public static Entity GetNaturalWeapon(string name = "NaturalWeapon")
        {
            var entity = GetMeleeWeapon(name);

            entity.AddComponent(new NoDamage());
            //NoBreak
            var physics = entity.GetComponent<Physics>();
            physics.Weight = 0;
            physics.IsSolid = false;
            physics.IsTakeable = false;
            physics.Category = PhysicsCategory.NaturalWeapon; 

            var melee = entity.GetComponent<MeleeWeapon>();
            melee.Proficiency = "BluntWeapons";
            melee.Stat = "Strength";

            entity.AddTag(new NeverStack());
            entity.AddTag(new NaturalGear());

            return entity;
        }

        public static void AddBody(Entity entity)
        {
            var body = new Body(Anatomy.Humanoid);

            var abdomenSlots = new ArmorSlots[1]
            {
                ArmorSlots.Belt,
            };
            var thoraxSlots = new ArmorSlots[2]
            {
                ArmorSlots.Body,
                ArmorSlots.Shoulders,
            };
            var abdomen = new BodyPart("Abdomen", BodyPartTypes.Hips, Laterality.None, null, abdomenSlots, true, false, false);
            var thorax = new BodyPart("Thorax", BodyPartTypes.Trunk, Laterality.None, abdomen, thoraxSlots, true, false, false);
            abdomen.AttachedTo = thorax;
            body.BodyParts.Add(abdomen);
            body.BodyParts.Add(thorax);

            var headSlots = new ArmorSlots[3]
            {
                ArmorSlots.Head,
                ArmorSlots.Eyes,
                ArmorSlots.Neck,
            };
            body.BodyParts.Add(new BodyPart("Head", BodyPartTypes.Head, Laterality.None, thorax, headSlots, false, true, true));

            var armSlots = new ArmorSlots[1]
            {
                ArmorSlots.Arms,
            };
            var leftArm = new BodyPart("ArmLeft", BodyPartTypes.Arm, Laterality.Left, thorax, armSlots, false, true, true);
            var rightArm = new BodyPart("ArmRight", BodyPartTypes.Arm, Laterality.Right, thorax, armSlots, false, true, true);
            body.BodyParts.Add(leftArm);
            body.BodyParts.Add(rightArm);

            var handSlots = new ArmorSlots[2]
            {
                ArmorSlots.Hands,
                ArmorSlots.Ring,
            };
            body.BodyParts.Add(new BodyPart("HandLeft", BodyPartTypes.Hand, Laterality.Left, leftArm, handSlots, false, true, true));
            body.BodyParts.Add(new BodyPart("HandRight", BodyPartTypes.Hand, Laterality.Right, rightArm, handSlots, false, true, true));

            var leftLeg = new BodyPart("LegLeft", BodyPartTypes.Leg, Laterality.Left, abdomen, null, false, true, true);
            var rightLeg = new BodyPart("LegRight", BodyPartTypes.Leg, Laterality.Right, abdomen, null, false, true, true);
            body.BodyParts.Add(leftLeg);
            body.BodyParts.Add(rightLeg);

            var footSlots = new ArmorSlots[1]
            {
                ArmorSlots.Feet,
            };
            body.BodyParts.Add(new BodyPart("FootLeft", BodyPartTypes.Foot, Laterality.Left, leftLeg, footSlots, false, true, true));
            body.BodyParts.Add(new BodyPart("FootRight", BodyPartTypes.Foot, Laterality.Right, rightLeg, footSlots, false, true, true));

            entity.AddComponent(body);
        }

        public static void AddAttributes(Entity entity)
        {
            entity.AddStat(new StatBase("Strength", "STR", 10, 40));
            entity.AddStat(new StatBase("Agility", "AGI", 10, 40));
            entity.AddStat(new StatBase("Vitality", "VIT", 10, 40));
            entity.AddStat(new StatBase("Knowledge", "KNO", 10, 40));
            entity.AddStat(new StatBase("Willpower", "WIL", 10, 40));
            entity.AddStat(new StatBase("Charisma", "CHA", 10, 40));
        }

        public static StatBase[] GetBaseResistances()
        {
            var resitances = new StatBase[14];

            for (int i = 0; i < resitances.Length; i++)
            {
                var name = Enum.GetName(typeof(DamageTypes), i) + " Resistance";
                resitances[i] = new StatBase(name, name, 0, 100, -100, 200);
            }

            return resitances;
        }
    }
}