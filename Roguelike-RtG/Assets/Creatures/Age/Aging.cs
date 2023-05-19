using UnityEngine;

namespace JS.CharacterSystem
{
    public static class Aging
    {
        public static int GetMinLifespan(LifeExpectancy primaryLifespan, LifeExpectancy secondaryLifespan)
        {
            if (!primaryLifespan.Ages && !secondaryLifespan.Ages) return 0;

            //Double mortal lifespan if one parent is immortal
            if (primaryLifespan.Ages && !secondaryLifespan.Ages) return primaryLifespan.MinLifeExpectancy * 2;
            if (!primaryLifespan.Ages && secondaryLifespan.Ages) return secondaryLifespan.MinLifeExpectancy * 2;

            //Else return the average of the two
            return Mathf.RoundToInt((primaryLifespan.MinLifeExpectancy + secondaryLifespan.MinLifeExpectancy) * 0.5f);
        }

        public static int GetMaxLifespan(LifeExpectancy primaryLifespan, LifeExpectancy secondaryLifespan)
        {
            if (!primaryLifespan.Ages && !secondaryLifespan.Ages) return 0;

            //Double mortal lifespan if one parent is immortal
            if (primaryLifespan.Ages && !secondaryLifespan.Ages) return primaryLifespan.MaxLifeExpectancy * 2;
            if (!primaryLifespan.Ages && secondaryLifespan.Ages) return secondaryLifespan.MaxLifeExpectancy * 2;

            //Else return the average of the two
            return Mathf.RoundToInt((primaryLifespan.MaxLifeExpectancy + secondaryLifespan.MaxLifeExpectancy) * 0.5f);
        }

        public static int GetVenerableAge(int maxLifeExpectancy) => Mathf.RoundToInt(maxLifeExpectancy * 0.75f); // 3/4 max life
        public static int GetOldAge(int maxLifeExpectancy) => Mathf.RoundToInt(maxLifeExpectancy * 0.5f); //Half of Max
        public static int GetMiddleAge(int maxLifeExpectancy) => Mathf.RoundToInt(maxLifeExpectancy * 0.325f); //Half of Venerable
        public static int GetYoungAdultAge(int maxLifeExpectancy) => Mathf.RoundToInt(maxLifeExpectancy * 0.1625f); //Half of Middle Age

    }
}

