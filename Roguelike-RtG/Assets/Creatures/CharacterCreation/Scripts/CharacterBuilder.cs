using UnityEngine;
using JS.Architecture.EventSystem;
using JS.DomainSystem;

namespace JS.CharacterSystem.Creation
{
    //NOTE//
    //I don't think I actually need this script
    //All of the methods in here can just be moved to another for CharacterCreation, likely the one by same name
    //And then just replace this with a CreaturePresetSO set aside for assigning values

    [CreateAssetMenu(menuName = "Characters/Character Builder")]
    public class CharacterBuilder : ScriptableObject
    {
        private const int humanoidAttributePoints = 50;
        private const int demihumanAttributePoints = 75;
        private const int monstrousAttributePoints = 100;

        [SerializeField] private GameEvent onCharacterChangeEvent;
        [SerializeField] private CreatureCatalog catalog;

        [Space]

        [SerializeField] private string characterName;
        [SerializeField] private CharacterGender characterGender;
        [SerializeField] private int characterAge;
        [SerializeField] private AgeCategory ageCategory;
        [SerializeField] private CharacterRace race;
        [SerializeField] private bool isUndead = false;
        [SerializeField] private CharacterClass characterClass;

        //Attributes
        [SerializeField] private int[] attributeValues = new int[6];
        [SerializeField] private int[] minimumAttributeValues = new int[6];
        [SerializeField] private int[] attributePotentials = new int[6];
        [SerializeField] private int[] increaseAttributeCost = new int[6];
        [SerializeField] private int availableAttributePoints;
        [SerializeField] private int maxAttributePoints;

        private int[] skillValues = new int[20];

        private Domain domain;

        private void Awake()
        {
            attributeValues = new int[catalog.PrimaryAttributes.Length];
            skillValues = new int[catalog.Skills.Length];
        }

        public string CharacterName { get => characterName; set => SetName(value); }
        public CharacterGender CharacterGender { get => characterGender; set => SetGender(value); }
        public int CharacterAge { get => characterAge; set => SetAge(value); }
        public AgeCategory AgeCategory => ageCategory;
        public CharacterRace Race { get => race; set => SetRace(value); }
        public bool IsUndead { get => isUndead; set => SetUndead(value); }
        public CharacterClass Class { get => characterClass; set => SetClass(value); }

        public int[] AttributeValues => attributeValues;
        public int[] MinAttributeValues => minimumAttributeValues;
        public int[] AttributePotentials => attributePotentials;

        public int[] IncreaseAttributeCost => increaseAttributeCost;

        public int AvailableAttributePoints => availableAttributePoints;

        public int[] SkillValues
        {
            get => skillValues; set
            {
                skillValues = value;
                onCharacterChangeEvent?.Invoke();
            }
        }

        public Domain Domain
        {
            get => domain; set
            {
                domain = value;
                onCharacterChangeEvent?.Invoke();
            }
        }

        public void ResetValues()
        {
            characterName = "";
            characterGender = CharacterGender.Male; 
            characterAge = 0;
            race = null;
            isUndead = false;
            onCharacterChangeEvent?.Invoke();
        }

        private void SetName(string name)
        {
            characterName = name;
            onCharacterChangeEvent?.Invoke();
        }

        #region - Race -
        public void ResetRace()
        {
            SetRace(null);
        }

        private void SetRace(CharacterRace race)
        {
            this.race = race;
            RefactorGender();
            RefactorAttributes();
            onCharacterChangeEvent?.Invoke();
        }

        private void SetUndead(bool value)
        {
            isUndead = value;
            onCharacterChangeEvent?.Invoke();
        }
        #endregion

        #region - Gender -
        private void RefactorGender()
        {
            if (race == null) return;

            if (characterGender == CharacterGender.Male)
            {
                if (!race.HasMales)
                {
                    if (race.HasFemale) SetGender(CharacterGender.Female);
                    else SetGender(CharacterGender.Other);
                }
            }
            if (characterGender == CharacterGender.Female)
            {
                if (!race.HasFemale)
                {
                    if (race.HasMales) SetGender(CharacterGender.Female);
                    else SetGender(CharacterGender.Other);
                }
            }
            else SetGender(CharacterGender.Other);
        }

        private void SetGender(CharacterGender gender)
        {
            characterGender = gender;
            onCharacterChangeEvent?.Invoke();
        }
        #endregion

        #region - Age -
        public void ResetAge()
        {
            characterAge = 0;
            ageCategory = null;
            onCharacterChangeEvent?.Invoke();
        }

        private void SetAge(int age)
        {
            characterAge = age;
            ageCategory = FindAgeCategory();
            onCharacterChangeEvent?.Invoke();
        }

        private AgeCategory FindAgeCategory()
        {
            var life = race.LifeExpectancy;

            if (characterAge >= life.VenerableAge)
            {
                return catalog.Ages[catalog.Ages.Length - 1];
            }
            else if (characterAge >= life.OldAge)
            {
                return catalog.Ages[catalog.Ages.Length - 2];
            }
            else if (characterAge >= life.MiddleAge)
            {
                return catalog.Ages[catalog.Ages.Length - 3];
            }
            else if (characterAge >= life.YoungAdultAge)
            {
                return catalog.Ages[catalog.Ages.Length - 4];
            }
            return catalog.Ageless;
        }
        #endregion

        #region - Class -
        public void ResetClass()
        {
            SetClass(null);
        }

        private void SetClass(CharacterClass characterClass)
        {
            this.characterClass = characterClass;
            onCharacterChangeEvent?.Invoke();
        }
        #endregion

        #region - Attributes -
        public void ResetAttributes()
        {
            for (int i = 0; i < attributeValues.Length; i++)
            {
                attributeValues[i] = 0;
                minimumAttributeValues[i] = 0;
                attributePotentials[i] = 0;
                increaseAttributeCost[i] = 0;
            }
            availableAttributePoints = 0;
            maxAttributePoints = 0;
        }

        private void RefactorAttributes()
        {
            SetAvailableAttributePoints();
            SetAttributePotentials();
        }

        //Sets the number of available attribute points based on race
        private void SetAvailableAttributePoints()
        {
            if (race == null) return;

            switch (race.RaceCategory)
            {
                case RacialCategory.Humanoid:
                    maxAttributePoints = humanoidAttributePoints;
                    break;
                case RacialCategory.Demihuman:
                    maxAttributePoints = demihumanAttributePoints;
                    break;
                case RacialCategory.Monstrous:
                    maxAttributePoints = monstrousAttributePoints;
                    break;
            }
            availableAttributePoints = maxAttributePoints;
        }

        private void SetAttributePotentials()
        {
            if (race == null) return;

            for (int i = 0; i < attributePotentials.Length; i++)
            {
                minimumAttributeValues[i] = race.BaseAttributes[i].value;
                attributePotentials[i] = race.AttributePotentials[i].value;
                SetAttributeValue(i, minimumAttributeValues[i]);
            }
        }

        public void SetAttributeValue(int index, int value)
        {
            attributeValues[index] = value;
            
            increaseAttributeCost[index] = SetIncreaseAttributeCost(index);
        }

        public void IncreaseAttribute(int index)
        {
            //Cannot raise above starting potential during character creation
            if (attributeValues[index] >= attributePotentials[index]) return;

            SetAttributeValue(index, attributeValues[index] + 1);

            //Not enought points, need to set it first to get the correct cost for increasing value
            if (availableAttributePoints < increaseAttributeCost[index])
            {
                SetAttributeValue(index, attributeValues[index] - 1);
                return;
            }
            
            availableAttributePoints -= increaseAttributeCost[index];
            onCharacterChangeEvent?.Invoke();
        }

        public void DecreaseAttribute(int index)
        {
            //Cannot lower values any further
            if (attributeValues[index] <= minimumAttributeValues[index]) return;

            availableAttributePoints += increaseAttributeCost[index];
            SetAttributeValue(index, attributeValues[index] - 1); 
            onCharacterChangeEvent?.Invoke();
        }

        private int SetIncreaseAttributeCost(int index)
        {
            return attributeValues[index] switch
            {
                < 0 => throw new UnityException("Value cannot be less than 0"),
                < 20 => 1,
                < 30 => 2,
                < 40 => 3,
                < 50 => 4,
                < 60 => 5,
                < 70 => 6,
                < 80 => 7,
                < 90 => 8,
                < 100 => 9,
                >= 100 => 10,
                //_ => 1,
            };
        }
        #endregion
    }
}