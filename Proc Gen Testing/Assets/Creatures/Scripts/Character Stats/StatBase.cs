using System.Collections.Generic;
using UnityEngine;

namespace JS.CharacterSystem
{
    public abstract class StatBase
    {
        public int BaseValue { get; protected set; } = 10;
        public int Value => GetModifiedValue();
        public int AbsoluteValue { get; protected set; }

        public int XP { get; protected set; } = 0;
        public int XPToNextLevel { get; protected set; }


        protected List<int> modifiers;

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
            if (BaseValue >= AbsoluteValue) return;

            XP += xp;
            if (XP >= XPToNextLevel) OnLevelUp();
        }

        protected virtual void OnLevelUp()
        {
            //meant to be overwritten
        }

        protected void CalculateXPToNextLevel()
        {
            XPToNextLevel = Mathf.RoundToInt(2 * BaseValue + 1) * 3; //Use this for testing
            //XPToNextLevel = Mathf.RoundToInt(2 * BaseValue + 1) * 30; //Use this for actual play
        }
    }
}