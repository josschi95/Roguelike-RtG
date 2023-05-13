using System.Collections.Generic;

namespace JS.CharacterSystem
{
    public class Attribute : StatBase
    {
        public int PotentialValue { get; private set; }

        public Attribute(int baseValue = 10, int potential = 100, int absolute = 100)
        {
            BaseValue = baseValue;
            PotentialValue = potential;
            AbsoluteValue = absolute;
            CalculateXPToNextLevel();
            modifiers = new List<int>();
        }

        protected override void OnLevelUp()
        {
            if (BaseValue < PotentialValue) BaseValue++;
            else OnIncreasePotential();

            XP -= XPToNextLevel;
            CalculateXPToNextLevel();

            if (BaseValue >= AbsoluteValue) XP = 0;
            if (XP >= XPToNextLevel) OnLevelUp();
        }

        private void OnIncreasePotential()
        {
            if (PotentialValue >= AbsoluteValue) return;

            PotentialValue++;
        }
    }
}