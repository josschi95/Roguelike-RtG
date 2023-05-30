namespace JS.ECS
{
    public class BodyPart
    {
        public string Name;
        public BodyPartTypes Type;
        public Laterality Laterality = Laterality.None;
        public BodyPart AttachedTo;
        public ArmorSlots[] ArmorSlots;
        public bool Integral = false; //true for body, back
        public bool IsAppendage = true; //not true for Thorax, Abdomen, Back
        public bool AverageArmor = false; //true for heads, arms, hands, feet

        public BodyPart(string name, BodyPartTypes type, Laterality laterality, BodyPart attachedTo, ArmorSlots[] armorSlots,  bool integral = false, bool isAppendage = true, bool averageArmor = false)
        {
            Name = name;
            Type = type;
            Laterality = laterality;
            AttachedTo = attachedTo;
            ArmorSlots = armorSlots;

            Integral = integral;
            IsAppendage = isAppendage;
            AverageArmor = averageArmor;
        }

        public void FireEvent(Event newEvent)
        {
            //
        }
    }
}

/*
 * Head
 * Name = "Head"
 * Type = "Head"
 * Laterality = "None"
 * UsuallyOn = Neck;
 * Integral = false;
 * IsAppendage = true;
 * AverageArmor = true;
 * 
 * 
*/