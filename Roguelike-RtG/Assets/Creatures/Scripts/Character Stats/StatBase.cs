using System.Collections.Generic;
using UnityEngine;

namespace JS.CharacterSystem
{
    public class StatBase
    {
        public string Name { get; protected set; }
        public string ShortName { get; protected set; }
        public int MinValue { get; protected set; }
        public int Potential { get; protected set; }
        public int MaxValue { get; protected set; }
        //
        public int BaseValue { get; protected set; } = 10;
        public int Value => GetModifiedValue();
        //
        public int XP { get; protected set; } = 0;
        public int XPToNextLevel { get; protected set; }
        //
        protected List<int> modifiers;

        public StatBase(string name, string shortName, int value = 10, int potential = 100, int min = 1, int max = 100)
        {
            Name = name;
            ShortName = shortName;

            BaseValue = value;
            Potential = potential;
            
            MinValue = min;
            MaxValue = max;

            modifiers = new List<int>();
            CalculateXPToNextLevel();
        }

        public void IncreaseBaseValue(int bonus)
        {
            BaseValue += bonus;
        }

        private int GetModifiedValue()
        {
            int modifiedValue = BaseValue;
            modifiers.ForEach(x => modifiedValue += x);
            return modifiedValue;
        }

        public void AddModifier(int value)
        {
            if (value == 0) return;
            modifiers.Add(value);
        }

        public void RemoveModifier(int value)
        {
            if (value == 0) return;
            modifiers.Remove(value);
        }

        public void OnGainXP(int xp)
        {
            if (BaseValue >= MaxValue) return;

            XP += xp;
            if (XP >= XPToNextLevel) OnLevelUp();
        }

        protected virtual void OnLevelUp()
        {
            if (BaseValue >= MaxValue) return;

            if (BaseValue < Potential) BaseValue++;
            else OnIncreasePotential();

            XP -= XPToNextLevel;
            CalculateXPToNextLevel();

            if (BaseValue >= MaxValue) XP = 0;
            if (XP >= XPToNextLevel) OnLevelUp();
        }

        private void OnIncreasePotential()
        {
            if (Potential >= MaxValue) return;

            Potential++;
        }

        protected void CalculateXPToNextLevel()
        {
            XPToNextLevel = Mathf.RoundToInt(2 * BaseValue + 1) * 3; //Use this for testing
            //XPToNextLevel = Mathf.RoundToInt(2 * BaseValue + 1) * 30; //Use this for actual play
        }
    }
}