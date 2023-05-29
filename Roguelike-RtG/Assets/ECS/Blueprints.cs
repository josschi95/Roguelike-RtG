using JS.CharacterSystem;
using JS.ECS.Tags;

namespace JS.ECS
{
    public class Blueprints
    {
        public static Entity GetObject(string name = "Entity")
        {
            var entity = new Entity(name);
            entity.AddComponent(new Transform());
            return entity;
        }

        public static Entity GetPhysicalObject(string name = "PhysicalObject")
        {
            var entity = GetObject(name);
            //Render
            entity.AddComponent(new Physics());
            entity.AddComponent(new MeleeWeapon());
            entity.AddComponent(new Commerce());
            entity.AddComponent(new Description());

            entity.AddComponent(new Brain());
            entity.AddComponent(new Body(Anatomy.Humanoid));
            entity.AddComponent(new Inventory());
            //Experience
            //Skills
            //Spells
            //Abilities
            //RandomLoot

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

            return entity;
        }

        public static Entity GetCreature(string name = "BaseCreature")
        {
            var entity = GetPhysicalObject(name);




            return entity;
        }

        public static Entity GetItem(string name = "Item")
        {
            var entity = GetInorganicObject(name);

            entity.AddTag(new Item());

            return entity;
        }
    }
}