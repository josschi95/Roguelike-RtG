using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JS.CharacterSystem.Creation
{
    public class UICharacterCreation_Attributes : MonoBehaviour
    {
        [SerializeField] private CreatureCatalog catalog;
        [SerializeField] private CharacterBuilder builder;
        [SerializeField] private UICharacterCreation characterCreation;

        [SerializeField] private Button[] decreaseAttributeButtons;
        [SerializeField] private Button[] increaseAttributeButtons;
        [SerializeField] private TMP_Text[] attributeValueTexts;

        [SerializeField] private TMP_Text[] attributePotentialTexts;
        [SerializeField] private TMP_Text[] attributeAbsoluteTexts;
        [SerializeField] private TMP_Text availablePointsText;

        [SerializeField] private Button nextButton;
        [SerializeField] private GameObject confirmNextPanel;

        private void OnEnable()
        {
            SetButtons();
            OnCharacterChanged();

            for (int i = 0; i < attributePotentialTexts.Length; i++)
            {
                attributePotentialTexts[i].text = builder.AttributePotentials[i].ToString();

            }

            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(CheckForUnspentPoints);
        }

        private void OnDisable()
        {
            for (int i = 0; i < decreaseAttributeButtons.Length; i++)
            {
                decreaseAttributeButtons[i].onClick.RemoveAllListeners();
                increaseAttributeButtons[i].onClick.RemoveAllListeners();
            }
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(characterCreation.Next);
        }


        private void SetButtons()
        {
            for (int i = 0; i < decreaseAttributeButtons.Length; i++)
            {
                int index = i;
                decreaseAttributeButtons[index].onClick.AddListener(delegate
                {
                    builder.DecreaseAttribute(index);
                });
                increaseAttributeButtons[index].onClick.AddListener(delegate
                {
                    builder.IncreaseAttribute(index);
                });
            }
        }

        public void OnCharacterChanged()
        {
            availablePointsText.text = builder.AvailableAttributePoints.ToString();

            for (int i = 0; i < attributeValueTexts.Length; i++)
            {
                attributeValueTexts[i].text = builder.AttributeValues[i].ToString();
            }

            for (int i = 0; i < increaseAttributeButtons.Length; i++)
            {
                //Enable/Disable increase buttons if there are points to spend
                increaseAttributeButtons[i].gameObject.SetActive(builder.AvailableAttributePoints > 0);

                //Enable/Disable increase buttons individually based on cost and available points
                int index = i;
                increaseAttributeButtons[index].interactable = builder.AvailableAttributePoints >= builder.IncreaseAttributeCost[index];

                //Enable/Disable decrease buttons if the value is above its minimum value
                decreaseAttributeButtons[index].interactable = builder.AttributeValues[index] > builder.MinAttributeValues[index];
            }
        }

        private void CheckForUnspentPoints()
        {
            if (builder.AvailableAttributePoints > 0)
            {
                confirmNextPanel.SetActive(true);
            }
            else characterCreation.Next();
        }
    }
}