using UnityEngine;
using JS.EventSystem;
using Unity.Collections.LowLevel.Unsafe;

namespace JS.CharacterSystem.Creation
{
    [CreateAssetMenu(menuName = "Characters/Character Builder")]
    public class CharacterBuilder : ScriptableObject
    {
        [SerializeField] private GameEvent onCharacterChangeEvent;

        private string characterName;
        private Gender characterGender;
        private int characterAge;
        private CharacterRace primaryRace;
        private CharacterRace secondaryRace;
        private bool isUndead = false;
        private CharacterClass characterClass;

        public string CharacterName { get => characterName; set => SetName(value); }
        public Gender CharacterGender { get => characterGender; set => SetGender(value); }
        public int CharacterAge { get => characterAge; set => SetAge(value); }

        public CharacterRace PrimaryRace { get => primaryRace; set => SetPrimaryRace(value); }
        public CharacterRace SecondaryRace { get => secondaryRace; set => SetSecondaryRace(value); }
        public bool IsUndead { get => isUndead; set => SetUndead(value); }
        public CharacterClass Class { get => characterClass; set => SetClass(value); }

        public void ResetValues()
        {
            characterName = "";
            characterGender = Gender.Male; 
            characterAge = 0;
            primaryRace = null;
            secondaryRace = null;
            isUndead = false;
            onCharacterChangeEvent?.Invoke();
        }

        private void SetName(string name)
        {
            characterName = name;
            onCharacterChangeEvent?.Invoke();
        }

        private void SetGender(Gender gender)
        {
            characterGender = gender;
            onCharacterChangeEvent?.Invoke();
        }

        private void SetAge(int age)
        {
            characterAge = age;
            onCharacterChangeEvent?.Invoke();
        }

        private void SetPrimaryRace(CharacterRace race)
        {
            primaryRace = race;
            secondaryRace = race;

            if (race == null) return;
            if (characterGender == Gender.Male && !race.HasMales)
            {
                if (race.HasFemale) SetGender(Gender.Female);
                else SetGender(Gender.Other);
            }
            else if (characterGender == Gender.Female && !race.HasFemale)
            {
                if (race.HasMales) SetGender(Gender.Male);
                else SetGender(Gender.Other);
            }
            else if (characterGender == Gender.Other && !race.HasOther)
            {
                if (race.HasMales) SetGender(Gender.Male);
                else if (race.HasFemale) SetGender(Gender.Female);
            }

            onCharacterChangeEvent?.Invoke();
        }

        private void SetSecondaryRace(CharacterRace race)
        {
            secondaryRace = race;
            onCharacterChangeEvent?.Invoke();
        }

        private void SetUndead(bool value)
        {
            isUndead = value;
            onCharacterChangeEvent?.Invoke();
        }

        private void SetClass(CharacterClass characterClass)
        {
            this.characterClass = characterClass;
            onCharacterChangeEvent?.Invoke();
        }
    }
}

