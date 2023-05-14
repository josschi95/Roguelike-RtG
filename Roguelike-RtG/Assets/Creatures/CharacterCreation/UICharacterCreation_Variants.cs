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
            maleButton.onClick.AddListener(delegate
            {
                characterBuilder.CharacterGender = Gender.Male;
            });
            maleButton.interactable = characterBuilder.PrimaryRace.HasMales ||
                characterBuilder.SecondaryRace.HasMales;

            femaleButton.onClick.AddListener(delegate
            {
                characterBuilder.CharacterGender = Gender.Female;
            });
            femaleButton.interactable = characterBuilder.PrimaryRace.HasFemale ||
                characterBuilder.SecondaryRace.HasFemale;
            
            otherButton.onClick.AddListener(delegate
            {
                characterBuilder.CharacterGender = Gender.Other;
            });
            otherButton.interactable = characterBuilder.PrimaryRace.HasOther ||
                characterBuilder.SecondaryRace.HasOther;

            ageSlider.onValueChanged.AddListener(UpdateAgeDisplay);

            //Take average of primary and secondary life expectancies
            ageSlider.minValue = Mathf.RoundToInt((characterBuilder.PrimaryRace.LifeExpectancy.YoungAdultAge +
                characterBuilder.SecondaryRace.LifeExpectancy.YoungAdultAge) * 0.5f);

            ageSlider.maxValue = Mathf.RoundToInt((characterBuilder.PrimaryRace.LifeExpectancy.MaxLifeExpectancy +
                characterBuilder.SecondaryRace.LifeExpectancy.MaxLifeExpectancy) * 0.5f) - 1;

            ageSlider.value = ageSlider.minValue;

            undeadToggle.onValueChanged.AddListener(OnToggleUndead);

            if (!characterBuilder.PrimaryRace.Type.CanBeUndead &&
                !characterBuilder.SecondaryRace.Type.CanBeUndead)
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
            ageText.text = age.ToString();

            var primary = characterBuilder.PrimaryRace.LifeExpectancy;
            var secondary = characterBuilder.SecondaryRace.LifeExpectancy;

            if (age >= Mathf.RoundToInt((primary.VenerableAge +
                secondary.VenerableAge) * 0.5f))
            {
                ageText.text += " (Venerable)";
            }
            else if (age >= Mathf.RoundToInt((primary.OldAge +
                secondary.OldAge) * 0.5f))
            {
                ageText.text += " (Old)";
            }
            else if (age >= Mathf.RoundToInt((primary.MiddleAge +
                secondary.MiddleAge) * 0.5f))
            {
                ageText.text += " (Middle Age)";
            }
            else if (age >= Mathf.RoundToInt((primary.YoungAdultAge +
                secondary.YoungAdultAge) * 0.5f))
            {
                ageText.text += " (Young Adult)";
            }
            else
            {
                Debug.LogWarning("Fix this");
            }
        }
    }
}