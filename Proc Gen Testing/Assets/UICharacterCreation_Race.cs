using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JS.CharacterSystem;
using System;
using Unity.VisualScripting;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace JS.CharacterCreation
{
    public class UICharacterCreation_Race : MonoBehaviour
    {
        [SerializeField] private UICharacterCreation characterCreatorParent;

        [Space]

        [Header("Character Races")]
        [SerializeField] private Button humanoidRacebutton;
        [SerializeField] private Button demiHumanRacebutton;
        [SerializeField] private Button monstrousRacebutton;
        [SerializeField] private Button[] genderButtons;

        [Space]

        [SerializeField] private Toggle undeadToggle;
        [SerializeField] private Toggle hybridToggle;

        [Space]

        [SerializeField] private CharacterRace[] humanoidRaces;
        [SerializeField] private CharacterRace[] demihumanRaces;
        [SerializeField] private CharacterRace[] monstrousRaces;

        [Space]

        [SerializeField] private RectTransform raceButtonParent;
        [SerializeField] private UIRaceSelectionPanel raceButtonPrefab;

        public bool SelectingPrimaryRace = true;

        private void OnEnable()
        {
            DisplayRacialOptions(humanoidRaces);

            humanoidRacebutton.onClick.AddListener(delegate
            {
                DisplayRacialOptions(humanoidRaces);
            });
            demiHumanRacebutton.onClick.AddListener(delegate
            {
                DisplayRacialOptions(demihumanRaces);
            });
            monstrousRacebutton.onClick.AddListener(delegate
            {
                DisplayRacialOptions(monstrousRaces);
            });

            for (int i = 0; i < genderButtons.Length; i++)
            {
                int index = i;
                genderButtons[i].onClick.AddListener(delegate
                {
                    characterCreatorParent.PlayerGender = (Gender)index;
                });
            }

            hybridToggle.onValueChanged.AddListener(delegate { ToggleHybrid(hybridToggle.isOn); });
        }

        private void OnDisable()
        {
            humanoidRacebutton.onClick.RemoveAllListeners();
            demiHumanRacebutton.onClick.RemoveAllListeners();
            monstrousRacebutton.onClick.RemoveAllListeners();
        }

        private void ResetButtons()
        {
            int count = raceButtonParent.transform.childCount;
            for (int i = count - 1; i >= 0; i--)
            {
                Destroy(raceButtonParent.transform.GetChild(i).gameObject);
            }
        }

        private void DisplayRacialOptions(CharacterRace[] collection)
        {
            ResetButtons();
            for (int i = 0; i < collection.Length; i++)
            {
                int index = i;
                var race = collection[index];

                var raceOption = Instantiate(raceButtonPrefab);
                raceOption.transform.SetParent(raceButtonParent.transform, false);
                raceOption.SetRace(race, characterCreatorParent);
                raceOption.button.onClick.AddListener(delegate
                {
                    OnConfirmRace(race);
                });
            }
        }

        private void ToggleHybrid(bool isOn)
        {
            if (isOn)
            {

            }
            else
            {
                characterCreatorParent.SetHybridRace(null);
            }
        }

        private void ResetRace()
        {
            characterCreatorParent.SetRace(null);
            characterCreatorParent.SetHybridRace(null);
        }

        private void OnConfirmRace(CharacterRace race)
        {
            if (SelectingPrimaryRace)
            {
                characterCreatorParent.SetRace(race);
                if (hybridToggle.isOn)
                {
                    SelectingPrimaryRace = false;
                }
                else characterCreatorParent.Next();
            }
            else
            {
                characterCreatorParent.SetHybridRace(race);
                characterCreatorParent.Next();
            }
        }
    }
}