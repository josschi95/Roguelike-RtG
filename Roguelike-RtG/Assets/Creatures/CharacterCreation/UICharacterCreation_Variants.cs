using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

namespace JS.CharacterSystem.Creation
{
    public class UICharacterCreation_Variants : MonoBehaviour
    {
        [SerializeField] private CharacterBuilder characterBuilder;
        [SerializeField] private Button nextButton;
        [Header("Gender")]
        [SerializeField] private Button maleButton;
        [SerializeField] private Button femaleButton;
        [SerializeField] private Button otherButton;

        [Header("Age")]
        [SerializeField] private TMP_Text ageText;
        [SerializeField] private Slider ageSlider;

        [Space]

        [SerializeField] private Toggle undeadToggle;

        private void OnEnable()
        {
            maleButton.onClick.AddListener(delegate
            {
                characterBuilder.CharacterGender = Gender.Male;
            });
            femaleButton.onClick.AddListener(delegate
            {
                characterBuilder.CharacterGender = Gender.Female;
            });
            otherButton.onClick.AddListener(delegate
            {
                characterBuilder.CharacterGender = Gender.Other;
            });

            ageSlider.onValueChanged.AddListener(UpdateAgeDisplay);
            int min = characterBuilder.PrimaryRace.AgeRanges[2].Age; //young adult
            var life = characterBuilder.PrimaryRace.LifeSpan;
            int max = life.modifier + (life.diceCount * life.diceSides) - 1; //max lifespan
            ageSlider.minValue = min;
            ageSlider.maxValue = max;
            ageSlider.value = min;

            undeadToggle.onValueChanged.AddListener(OnToggleUndead);

            nextButton.enabled = true;
        }

        private void OnDisable()
        {
            maleButton.onClick.RemoveAllListeners();
            femaleButton.onClick.RemoveAllListeners();
            otherButton.onClick.RemoveAllListeners();

            ageSlider.onValueChanged.RemoveAllListeners();
            undeadToggle.onValueChanged.RemoveAllListeners();

            nextButton.enabled = false;
        }

        public void OnCharacterChanged()
        {
            maleButton.interactable = characterBuilder.PrimaryRace.HasMales;
            femaleButton.interactable = characterBuilder.PrimaryRace.HasFemale;
            otherButton.interactable = characterBuilder.PrimaryRace.HasOther;
        }

        private void OnToggleUndead(bool value)
        {
            characterBuilder.IsUndead = value;
        }

        private void UpdateAgeDisplay(float value)
        {
            int age = Mathf.RoundToInt(value);
            ageText.text = age.ToString();

            for (int i = characterBuilder.PrimaryRace.AgeRanges.Length - 1; i >= 0; i--)
            {
                if (age >= characterBuilder.PrimaryRace.AgeRanges[i].Age)
                {
                    ageText.text += " (" + characterBuilder.PrimaryRace.AgeRanges[i].Category.Name + ")";
                    break;
                }
            }
        }
    }
}