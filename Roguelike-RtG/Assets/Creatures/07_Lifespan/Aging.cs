using UnityEngine;

namespace JS.CharacterSystem
{
    public static class Aging
    {
        public static int GetVenerableAge(int maxLifeExpectancy) 
            => Mathf.RoundToInt(maxLifeExpectancy * 0.75f); // 3/4 max life
        
        public static int GetOldAge(int maxLifeExpectancy) 
            => Mathf.RoundToInt(maxLifeExpectancy * 0.5f); //Half of Max
        
        public static int GetMiddleAge(int maxLifeExpectancy) 
            => Mathf.RoundToInt(maxLifeExpectancy * 0.325f); //Half of Venerable
        
        public static int GetYoungAdultAge(int maxLifeExpectancy) 
            => Mathf.RoundToInt(maxLifeExpectancy * 0.1625f); //Half of Middle Age
    }
}

