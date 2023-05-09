using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;
using System.Diagnostics;

namespace JS.CharacterSystem
{
    public class UICharacterCreation : MonoBehaviour
    {
        [SerializeField] private GameObject[] categoryPanels;
        private int activePanel = 0;

        [Space]

        private string playerName;
        private Gender playerGender;
        public Gender PlayerGender
        {
            get => playerGender;
            set
            {
                SetGender(value);
            }
        }

        public bool IsUndead { get; set; }
        public bool IsHybrid { get; set; }

        private CharacterRace chosenRace;
        private CharacterRace secondaryRace;

        [SerializeField] private Button backButton;
        [SerializeField] private Button helpButton;

        [Header("Character Sheet")]
        [SerializeField] private TMP_InputField nameInput;
        [SerializeField] private Image playerSprite;
        [SerializeField] private TMP_Text playerRaceText;
        [SerializeField] private TMP_Text playerArchetypeText;
        [SerializeField] private TMP_Text playerSizeText;
        [SerializeField] private TMP_Text playerGenderText;
        [SerializeField] private TMP_Text playerAgeText;

        [Space]

        [SerializeField] private TMP_Text descriptionText;

        private void Start()
        {
            backButton.onClick.AddListener(Back);

            nameInput.onSubmit.AddListener(delegate
            {
                SetName(nameInput.text);
            });
        }

        private void OnDestroy()
        {
            nameInput.onSubmit.RemoveAllListeners();
        }

        //[Header("Character Class")]


        //[Header("Character Skills")]


        //[Header("Character Domain")]


        //[Header("Finalization")]

        public void Back()
        {
            if (activePanel == 0)
            {
                //return to main menu
            }
            else
            {
                DisplayCategoryPanel(activePanel - 1);
            }
        }

        public void Next()
        {
            if (activePanel == categoryPanels.Length - 1)
            {
                //complete
            }
            else
            {
                DisplayCategoryPanel(activePanel + 1);
            }
        }

        private void DisplayCategoryPanel(int category)
        {
            activePanel = category;
            for (int i = 0; i < categoryPanels.Length; i++)
            {
                if (i == category) categoryPanels[i].SetActive(true);
                else categoryPanels[i].SetActive(false);
            }
        }

        private void SetName(string name)
        {
            nameInput.text = name;
            playerName = name;
        }

        private void SetGender(Gender gender)
        {
            playerGender = gender;
            playerGenderText.text = "Gender: " + gender.ToString()[0];
        }

        public void ResetRace()
        {
            chosenRace = null;
            secondaryRace = null;
        }

        public void SetRace(CharacterRace race)
        {
            chosenRace = race;
            playerRaceText.text = chosenRace.RaceName;

            playerArchetypeText.text = chosenRace.Archetype.ArchetypeName;
            if (chosenRace.Archetype is RacialChildArchetype child)
            {
                playerArchetypeText.text += " (" + child.ArchetypeName + ")";
            }

            playerSizeText.text = chosenRace.Size.Name;
        }

        public void SetHybridRace(CharacterRace hybridRace)
        {
            secondaryRace = hybridRace;
            playerRaceText.text = chosenRace.RaceName + "/" + secondaryRace.RaceName;

            playerArchetypeText.text = chosenRace.Archetype.ArchetypeName;
            if (chosenRace.Archetype is RacialChildArchetype child)
            {
                playerArchetypeText.text += " (" + child.ArchetypeName + ")";
            }

            playerArchetypeText.text += "/" + secondaryRace.Archetype.ArchetypeName;
            if (secondaryRace.Archetype is RacialChildArchetype secondaryChild)
            {
                playerArchetypeText.text += " (" + secondaryChild.ArchetypeName + ")";
            }
        }

        public void OnRaceHover(CharacterRace race)
        {
            //Display info
            descriptionText.text = race.RaceDescription + "\n";

            //Archetype/Hierarchy
            if (race.Archetype is RacialChildArchetype child)
            {
                descriptionText.text += "(" + race.RaceType.ToString() + "/" + child.parentArchetype.ArchetypeName + "/" + child.ArchetypeName + ") " + race.RaceName + "\n";
            }
            else
            {
                descriptionText.text += "(" + race.RaceType.ToString() + "/" + race.Archetype.ArchetypeName + ") " + race.RaceName + "\n";
            }

            descriptionText.text += "Size: " + race.Size.Name + "\n";

            //Needs
            if (race.Archetype.NeedsAir) descriptionText.text += "Breaths: Yes; ";
            else descriptionText.text += "Breaths: No; ";
            if (race.Archetype.NeedsFood) descriptionText.text += "Eats: Yes; ";
            else descriptionText.text += "Eats: No; ";
            if (race.Archetype.NeedsSleep) descriptionText.text += "Sleeps: Yes \n";
            else descriptionText.text += "Sleeps: No \n";

            int min = race.LifeSpan.modifier + race.LifeSpan.diceCount;
            int max = race.LifeSpan.modifier + (race.LifeSpan.diceCount * race.LifeSpan.diceSides);
            descriptionText.text += "Lifespan: " + min + " - " + max + "\n";

            foreach (var mod in race.RacialStats.AttributeModifiers)
            {
                char op = '+';
                if (mod.value < 0) op = '-';
                descriptionText.text += mod.attribute.ToString() + " : " + op + Mathf.Abs(mod.value) + "\n";
            }
            foreach (var mod in race.RacialStats.SkillModifiers)
            {
                char op = '+';
                if (mod.value < 0) op = '-';
                descriptionText.text += mod.skill.ToString() + " : " + op + Mathf.Abs(mod.value) + "\n";
            }
            foreach (var mod in race.RacialStats.ResistanceModifiers)
            {
                char op = '+';
                if (mod.value < 0) op = '-';
                descriptionText.text += mod.damageType.Name + " : " + op + Mathf.Abs(mod.value) + "\n";
            }
        }

        internal void OnRaceHoverExit()
        {
            descriptionText.text = "";
        }
    }
}
public enum Gender { Male, Female, Other }