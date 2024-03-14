using UnityEngine;

namespace JS.World.Time
{
    [CreateAssetMenu(menuName = "UI/Moon Phases")]
    public class MoonPhaseHelper : ScriptableObject
    {
        [SerializeField] private TimeKeeper timeKeeper;

        [Space]

        [SerializeField] private Sprite[] newMoon;
        [SerializeField] private Sprite[] waxingCrescentMoon;
        [SerializeField] private Sprite[] firstQuarterMoon;
        [SerializeField] private Sprite[] waxingGibbousMoon;
        [SerializeField] private Sprite[] fullMoon;
        [SerializeField] private Sprite[] waningGibbousMoon;
        [SerializeField] private Sprite[] thirdQuarterMoon;
        [SerializeField] private Sprite[] waningCrescentMoon;

        public Sprite GetMoonSprite()
        {
            switch (timeKeeper.MoonPhase)
            {
                case MoonPhase.NewMoon: return GetMoonSprite(newMoon);
                case MoonPhase.WaxingCrescent: return GetMoonSprite(waxingCrescentMoon);
                case MoonPhase.FirstQuarter: return GetMoonSprite(firstQuarterMoon);
                case MoonPhase.WaxingGibbous: return GetMoonSprite(waxingGibbousMoon);
                case MoonPhase.FullMoon: return GetMoonSprite(fullMoon);
                case MoonPhase.WaningGibbous: return GetMoonSprite(waningGibbousMoon);
                case MoonPhase.ThirdQuarter: return GetMoonSprite(thirdQuarterMoon);
                case MoonPhase.WaningCrescent: return GetMoonSprite(waningCrescentMoon);
            }
            return null;
        }

        private Sprite GetMoonSprite(Sprite[] collection)
        {
            switch (timeKeeper.Hours)
            {
                case 19: return collection[0];
                case 20: return collection[1];
                case 21: return collection[2];
                case 22: return collection[3];
                case 23: return collection[4];
                case 0: return collection[5];
                case 1: return collection[6];
                case 2: return collection[7];
                case 3: return collection[8];
                case 4: return collection[9];
                case 5: return collection[10];
            }
            return null;
        }
    }
}