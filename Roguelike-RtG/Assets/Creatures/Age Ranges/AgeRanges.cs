namespace JS.CharacterSystem
{
    [System.Serializable]
    public class AgeRanges
    {
        public AgeCategory Category;
        public int Age;
    }
}

/*
 * The following effects stack, and are applied to both the current Attribute and Potential
 * Middle Age: -5 to STR, AGI, VIT, +5 to KNO, WIL, CHA
 * Old Age: -10 to STR, AGI, VIT, +10 to KNO, WIL, CHA
 * Venerable Age: -15 to STR, AGI, VIT, +15 to KNO, WIL, CHA
 * 
 * Since Middle age has a lifeStage of 2, this can be simplified as
 * 
 * int stage = lifestage - 1; if (stage < 0) stage = 0;
 * int modifier = stage * 5;
 * 
 * STR, AGI, VI - modifier
 * KNO, WIL, CHA + modifier
 */