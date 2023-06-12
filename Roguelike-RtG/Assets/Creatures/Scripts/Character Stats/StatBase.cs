using System.Collections.Generic;
using UnityEngine;

namespace JS.CharacterSystem
{
    /// <summary>
    /// Class used for HP/SP/MP, AV/DV, MoveSpeed, Speed, Attributes, Skills, and Proficiencies
    /// </summary>
    public class StatBase
    {
        public string Name { get; protected set; }
        public string ShortName { get; protected set; }
        //
        public int CurrentValue;
        public int Value { get; protected set; } = 10;
        public int MinValue { get; protected set; } = 1;
        public int Potential { get; protected set; } = 100;
        public int MaxValue { get; protected set; } = 100;
        //
        public int XP { get; protected set; } = 0;
        public int XPToNextLevel { get; protected set; }

        public StatBase() { } 
        
        public StatBase(string name, string shortName, int value = 10, int potential = 100, int min = 1, int max = 100)
        {
            Name = name;
            ShortName = shortName;

            Value = value;
            CurrentValue = value;
            
            Potential = potential;
            MinValue = min;
            MaxValue = max;

            CalculateXPToNextLevel();
        }

        public void IncreaseBaseValue(int bonus)
        {
            Value += bonus;
        }

        public void OnGainXP(int xp)
        {
            if (Value >= MaxValue) return;

            XP += xp;
            if (XP >= XPToNextLevel) OnLevelUp();
        }

        protected virtual void OnLevelUp()
        {
            if (Value >= MaxValue) return;

            if (Value < Potential) Value++;
            else OnIncreasePotential();

            XP -= XPToNextLevel;
            CalculateXPToNextLevel();

            if (Value >= MaxValue) XP = 0;
            if (XP >= XPToNextLevel) OnLevelUp();
        }

        private void OnIncreasePotential()
        {
            if (Potential >= MaxValue) return;

            Potential++;
        }

        protected void CalculateXPToNextLevel()
        {
            XPToNextLevel = Mathf.RoundToInt(2 * Value + 1) * 3; //Use this for testing
            //XPToNextLevel = Mathf.RoundToInt(2 * BaseValue + 1) * 30; //Use this for actual play
        }
    }
}