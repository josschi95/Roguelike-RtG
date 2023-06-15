namespace JS.ECS.Materials
{
    public class ObjectMaterial
    {
        public string Name; //The name of the material

        public int HP; //the hitpoints of an item made from this material
        public int Hardness; //the Damage Reduction to incoming damage dealt to this objects of this material

        public float WeightModifier = 1f; //the modifier applied to the base weight of an object made from this material
        public float CostModifier = 1f; //the modifier applied to the base cost of an object made from this material

        public int AV; //the AV bonus to Armor and Shields made from this material
        public int DV; //the DV bonus to Armor and Shields made from this material
        public int Damage; //The Damage bonus to weapons made from this material

        public string[] Immunities; //damage types that this material is immune to
        public string[] Resistances; //damage types that this material is resistant to
        public string[] Weaknesses; //damage types that this material is weak to

        public string[] Effects; //Any additional effects imposed by this material
    }
}