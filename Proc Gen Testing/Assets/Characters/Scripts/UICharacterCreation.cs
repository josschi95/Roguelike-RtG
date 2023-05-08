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


        [Space]

        [SerializeField] private CharacterRace[] humanoidRaces;
        [SerializeField] private CharacterSubRace[] humanoidSubRaces;

        [Space]

        [SerializeField] private CharacterRace[] demihumanRaces;
        [SerializeField] private CharacterSubRace[] demihumanSubRaces;

        [Space]

        [SerializeField] private CharacterRace[] monstrousRaces;
        [SerializeField] private CharacterSubRace[] monstrousSubRaces;

        [SerializeField] private CharacterSubRace chosenRace;

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
                if (i == category) categoryPanels[i].SetActive(true);
                else categoryPanels[i].SetActive(false);
            }
        }

        public void SetRace(CharacterSubRace race)
        {
            chosenRace = race;
        }
    }
}