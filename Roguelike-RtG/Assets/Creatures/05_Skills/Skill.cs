using System.Collections.Generic;

namespace JS.CharacterSystem
{
    public class Skill : StatBase
    {
        public Skill(string name, string shortName, int baseValue)
        {
            Name = name;
            ShortName = shortName;
            MinValue = 1;
            MaxValue = 100;

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