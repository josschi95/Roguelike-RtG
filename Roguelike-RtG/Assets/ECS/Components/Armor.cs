namespace JS.ECS
{
    public class Armor : ComponentBase
    {
        public int ArmorValue;
        public int DodgeValue;
        public int WornOn;
    }
}

public enum ArmorSlots
{
    Head,   //Helms, Hoods, Hats, Caps, Crowns, Masks
    Eyes,   //Glasses, Blindfolds, Goggles, 
    Body,   //Armor, Robes, 
    Back,   //Capes, Cloaks, Mantles, Wings
    Neck,   //Amulets, Necklaces, Medallions
    Belt,   //Belts, Girdles, Bandoliers
    Arms,   //Bracers, Bucklers, Manacles, Shackles, Bracelets (if wrists)
    Hands,  //Gauntlets, Gloves
    Ring,   //Rings
    Feet,   //Boots, Sandles, Shoes
}

public enum WeaponSlots
{
    Hand,               //Unarmed, Pugilist Weapons, Blades, Axes, Blunt, Polearms, 
    MissileWeapon,      //Bows, Crossbows, Javelins, Throwing Axes/Knives/Clubs
    MissileAmmunition,  //Arrows, Bolts
}

/*
 * For reference, a recycling suit in CoQ has the following components
 * Armor: AV, DV, RF, WornOn
 * Render: no clue
 * Commerce: value
 * Examiner: complexity
 * LiquidVolume: max, volume, initial
 * RecyclingSuitFiller
 * Description: string
 * Physics: weight
 * TinerItem: Bits, canDisassemble, canBuild, canRepair
 * (tag) Tier
*/