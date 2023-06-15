namespace JS.ECS
{
    public class MeleeWeapon : ComponentBase
    {
        public MeleeWeapon() { }

        public string BaseDamage = "1d2"; //How much damage this weapon deals on a hit
        public int Accuracy = 0; //Bonus to rolls made to hit a target
        public int CritRange = 0; //Bonus to the Crititcal Hit Range for attacks made with this weapon
        public string Type = "Blunt"; //The damage type this weapon deals
        public string Stat = "STR"; //what Attribute is used for the weapon's damage bonus - STR means hitting harder, AGI means hitting more precisely
        public string Proficiency = "BluntWeapons"; //What proficiency stat is used, gives bonus to accuracy and damage
    }
}