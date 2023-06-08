namespace JS.ECS
{
    public class BodyPart : ComponentBase
    {
        public BodyPart() { }

        public string Name; //Name of the body part
        public string LimbBlueprint; //Name of the blueprint for the dismembered body part

        public bool Integral = false; //true for body
        public bool IsVital = false; //true for heads, does dismembering this component cause death?
        public Laterality Laterality = Laterality.None;

        public bool AverageArmor = false; //true for heads, arms, hands, feet

        public bool IsAppendage = true; //not true for Thorax, Abdomen, Back
        public BodyPart AttachedTo;
        
        public Entity defaultBehavior; //fists/claws, beaks/jaws, etc.
        public Entity weaponOverride; //wielded weapons

        public BodyPart(string name, Laterality laterality, BodyPart attachedTo, bool integral = false, bool isAppendage = true, bool averageArmor = false)
        {
            Name = name;
            Laterality = laterality;
            AttachedTo = attachedTo;

            Integral = integral;
            IsAppendage = isAppendage;
            AverageArmor = averageArmor;
        }

        public override void OnEvent(Event newEvent)
        {
            if(newEvent is DeclareMeleeAttack declaration)
            {
                if(weaponOverride != null)
                {

                }
                else if (defaultBehavior != null)
                {

                }
            }
        }
    }
}