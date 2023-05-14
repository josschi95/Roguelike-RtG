using System.Collections.Generic;

namespace JS.CharacterSystem
{
    public class Skill : StatBase
    {
        public Skill(int baseValue)
        {
            BaseValue = baseValue;
            AbsoluteValue = 100;

            CalculateXPToNextLevel();
            modifiers = new List<int>();
        }

        protected override void OnLevelUp()
        {
            if (BaseValue >= AbsoluteValue) return;
            BaseValue++;

            XP -= XPToNextLevel;
            CalculateXPToNextLevel();

            if (BaseValue >= AbsoluteValue) XP = 0;
            if (XP >= XPToNextLevel) OnLevelUp();
        }
    }
}