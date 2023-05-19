using UnityEngine;
using JS.EventSystem;
using JS.DomainSystem;

namespace JS.CharacterSystem.Creation
{
    [CreateAssetMenu(menuName = "Characters/Character Builder")]
    public class CharacterBuilder : ScriptableObject
    {
        private const int humanoidAttributePoints = 50;
        private const int demihumanAttributePoints = 75;
        private const int monstrousAttributePoints = 100;

        [SerializeField] private GameEvent onCharacterChangeEvent;
        [SerializeField] private CreatureCatalog catalog;

        private string characterName;
        [SerializeField] private Gender characterGender;
        [SerializeField] private int characterAge;
        [SerializeField] private AgeCategory ageCategory;
        [SerializeField] private CharacterRace primaryRace;
        [SerializeField] private CharacterRace secondaryRace;
        [SerializeField] private bool isUndead = false;
        [SerializeField] private CharacterClass characterClass;

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
        public Gender CharacterGender { get => characterGender; set => SetGender(value); }
        public int CharacterAge { get => characterAge; set => SetAge(value); }
        public AgeCategory AgeCategory => ageCategory;
        public CharacterRace PrimaryRace { get => primaryRace; set => SetPrimaryRace(value); }
        public CharacterRace SecondaryRace { get => secondaryRace; set => SetSecondaryRace(value); }
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
            ageCategory = FindAgeCategory();
            onCharacterChangeEvent?.Invoke();
        }

        private AgeCategory FindAgeCategory()
        {
            var primary = primaryRace.LifeExpectancy;
            var secondary = secondaryRace.LifeExpectancy;
            var max = Aging.GetMaxLifespan(primary, secondary);

            if (characterAge >= Aging.GetVenerableAge(max))
            {
                return catalog.Ages[catalog.Ages.Length - 1];
            }
            else if (characterAge >= Aging.GetOldAge(max))
            {
                return catalog.Ages[catalog.Ages.Length - 2];
            }
            else if (characterAge >= Aging.GetMiddleAge(max))
            {
                return catalog.Ages[catalog.Ages.Length - 3];
            }
            else if (characterAge >= Aging.GetYoungAdultAge(max))
            {
                return catalog.Ages[catalog.Ages.Length - 4];
            }

            throw new System.Exception("Fix this.");
        }

        #region - Creature Race -
        private void SetPrimaryRace(CharacterRace race)
        {
            primaryRace = race;
            secondaryRace = race;
            RefactorGender();
            RefactorAttributes();
            onCharacterChangeEvent?.Invoke();
        }

        private void SetSecondaryRace(CharacterRace race)
        {
            secondaryRace = race;
            RefactorGender();
            RefactorAttributes();
            onCharacterChangeEvent?.Invoke();
        }

        private void RefactorGender()
        {
            if (characterGender == Gender.Male)
            {
                if (!primaryRace.HasMales && !secondaryRace.HasMales)
                {
                    if (primaryRace.HasFemale || secondaryRace.HasFemale) SetGender(Gender.Female);
                    else SetGender(Gender.Other);
                }
            }
            if (characterGender == Gender.Female)
            {
                if (!primaryRace.HasFemale && !secondaryRace.HasFemale)
                {
                    if (primaryRace.HasMales || secondaryRace.HasMales) SetGender(Gender.Female);
                    else SetGender(Gender.Other);
                }
            }
            else SetGender(Gender.Other);
        }

        private void RefactorAttributes()
        {
            SetAvailableAttributePoints();
            SetAttributePotentials();
        }

        //Sets the number of available attribute points based on race
        private void SetAvailableAttributePoints()
        {
            if (primaryRace == null) return;

            int primary = humanoidAttributePoints;
            if (primaryRace.RaceCategory == RacialCategory.Demihuman) primary = demihumanAttributePoints;
            else if (primaryRace.RaceCategory == RacialCategory.Monstrous) primary = monstrousAttributePoints;
            int secondary = primary;

            if (primaryRace != secondaryRace)
            {
                secondary = humanoidAttributePoints;
                if (secondaryRace.RaceCategory == RacialCategory.Demihuman) secondary = demihumanAttributePoints;
                else if (secondaryRace.RaceCategory == RacialCategory.Monstrous) secondary = monstrousAttributePoints;
            }

            maxAttributePoints = Mathf.RoundToInt((primary + secondary) * 0.5f);
            availableAttributePoints = maxAttributePoints;
        }

        private void SetAttributePotentials()
        {
            if (primaryRace == null) return;

            for (int i = 0; i < attributePotentials.Length; i++)
            {
                minimumAttributeValues[i] = primaryRace.BaseAttributes[i].value;
                attributePotentials[i] = primaryRace.AttributePotentials[i].value;
                SetAttributeValue(i, minimumAttributeValues[i]);
            }

            if (primaryRace == secondaryRace) return;

            for (int i = 0; i < attributePotentials.Length; i++)
            {
                //Average two values
                minimumAttributeValues[i] = Mathf.RoundToInt((primaryRace.BaseAttributes[i].value + secondaryRace.BaseAttributes[i].value) * 0.5f);
                int value = Mathf.RoundToInt((primaryRace.AttributePotentials[i].value + secondaryRace.AttributePotentials[i].value) * 0.5f);
                attributePotentials[i] = value;
                SetAttributeValue(i, minimumAttributeValues[i]);
            }
        }
        #endregion

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

        private void SetAttributeValue(int index, int value)
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
    }
}