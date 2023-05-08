using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JS.CharacterCreation
{
    public class UICharacterCreation_Race : MonoBehaviour
    {

        [Header("Character Races")]
        [SerializeField] private Button humanoidRacebutton;
        [SerializeField] private Button demiHumanRacebutton;
        [SerializeField] private Button monstrousRacebutton;

        [Space]

        [SerializeField] private GameObject raceButtonParent;

        private void OnEnable()
        {
            humanoidRacebutton.onClick.AddListener(DisplayHumanoidRaces);
            demiHumanRacebutton.onClick.AddListener(DisplayDemiHumanraces);
            monstrousRacebutton.onClick.AddListener(DisplayMonstrousRaces);
        }

        private void OnDisable()
        {
            humanoidRacebutton.onClick.RemoveAllListeners();
            demiHumanRacebutton.onClick.RemoveAllListeners();
            monstrousRacebutton.onClick.RemoveAllListeners();
        }

        private void DisplayHumanoidRaces()
        {

        }

        private void DisplayDemiHumanraces()
        {

        }

        private void DisplayMonstrousRaces()
        {

        }
    }
}