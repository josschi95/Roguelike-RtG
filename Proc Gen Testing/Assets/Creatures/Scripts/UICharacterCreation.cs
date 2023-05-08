using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JS.CharacterSystem
{
    public class UICharacterCreation : MonoBehaviour
    {
        [SerializeField] private GameObject[] categoryPanels;
        [SerializeField] private Button[] categoryButtons;
        private int activePanel = 0;

        [Space]

        public CharacterRace chosenRace;
        public CharacterRace hybridSecondaryRace;

        //[Header("Character Class")]


        //[Header("Character Skills")]


        //[Header("Character Domain")]


        //[Header("Finalization")]


        private void OnEnable()
        {
            for (int i = 0; i < categoryButtons.Length; i++)
            {
                int cat = i;
                categoryButtons[cat].onClick.AddListener(delegate
                {
                    DisplayCategoryPanel(cat);
                });
            }


        }

        private void OnDisable()
        {
            for (int i = 0; i < categoryButtons.Length; i++)
            {
                categoryButtons[i].onClick.RemoveAllListeners();
            }
        }

        private void DisplayCategoryPanel(int category)
        {
            for (int i = 0; i < categoryPanels.Length; i++)
            {
                activePanel = i;
                if (i == category) categoryPanels[i].SetActive(true);
                else categoryPanels[i].SetActive(false);
            }
        }

        public void SetRace(CharacterRace race)
        {
            chosenRace = race;
            DisplayCategoryPanel(activePanel + 1);
        }

        public void SetHybridRace(CharacterRace hybridRace)
        {
            hybridSecondaryRace = hybridRace;
        }
    }
}