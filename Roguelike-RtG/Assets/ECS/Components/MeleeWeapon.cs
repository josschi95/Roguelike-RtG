namespace JS.ECS
{
    public class MeleeWeapon : ComponentBase
    {
        public MeleeWeapon() { }

        public MeleeWeapon(string baseDamage = "1d2", string type = "Blunt", string stat = "STR", string proficiency = "BluntWeapons")
        {
            BaseDamage = baseDamage;
            Type = type;
            Stat = stat;
            Proficiency = proficiency;
        }

        public string BaseDamage = "1d2";
        public string Type = "Blunt";
        public string Stat = "STR";
        public string Proficiency = "BluntWeapons";

        public override void OnEvent(Event newEvent)
        {
            if (newEvent is DeclareMeleeAttack declaration)
            {

            }
        }
    }
}