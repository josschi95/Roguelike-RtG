using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
            UpdatePanelOptions();
        }

        private void OnDisable()
        {
            maleButton.onClick.RemoveAllListeners();
            femaleButton.onClick.RemoveAllListeners();
            otherButton.onClick.RemoveAllListeners();

            ageSlider.onValueChanged.RemoveAllListeners();
            undeadToggle.onValueChanged.RemoveAllListeners();
        }

        private void UpdatePanelOptions()
        {
            #region - Gender -
            maleButton.onClick.AddListener(delegate
            {
                characterBuilder.CharacterGender = CharacterGender.Male;
            });
            maleButton.interactable = characterBuilder.Race.HasMales;

            femaleButton.onClick.AddListener(delegate
            {
                characterBuilder.CharacterGender = CharacterGender.Female;
            });
            femaleButton.interactable = characterBuilder.Race.HasFemale;
            
            otherButton.onClick.AddListener(delegate
            {
                characterBuilder.CharacterGender = CharacterGender.Other;
            });
            otherButton.interactable = characterBuilder.Race.HasOther;
            #endregion

            //Age
            ageSlider.onValueChanged.AddListener(UpdateAgeDisplay);
            ageSlider.minValue = characterBuilder.Race.LifeExpectancy.YoungAdultAge;
            ageSlider.maxValue = characterBuilder.Race.LifeExpectancy.MaxLifeExpectancy - 1;

            if (!characterBuilder.Race.LifeExpectancy.Ages)
            {
                ageSlider.minValue = 0;
                ageSlider.maxValue = 1000;
            }

            ageSlider.value = ageSlider.minValue;

            undeadToggle.onValueChanged.AddListener(OnToggleUndead);

            if (!characterBuilder.Race.Type.CanBeUndead)
            {
                undeadToggle.interactable = false;
            }
            else undeadToggle.interactable = true;
        }

        private void OnToggleUndead(bool value)
        {
            characterBuilder.IsUndead = value;
        }

        private void UpdateAgeDisplay(float value)
        {
            int age = Mathf.RoundToInt(value);
            characterBuilder.CharacterAge = age;
            ageText.text = age.ToString();

            var life = characterBuilder.Race.LifeExpectancy;
            if (!life.Ages) ageText.text += " (Ageless)";
            else if (age >= life.VenerableAge) ageText.text += " (Venerable)";
            else if (age >= life.OldAge) ageText.text += " (Old)";
            else if (age >= life.MiddleAge) ageText.text += " (Middle Age)";
            else if (age >= life.YoungAdultAge) ageText.text += " (Young Adult)";
        }
    }
}