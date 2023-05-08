using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JS.CharacterSystem;
using System;
using Unity.VisualScripting;
using System.Net.NetworkInformation;

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

        [Space]

        [SerializeField] private Button confirmRaceButton;
        [SerializeField] private Button previousRaceButton;
        [SerializeField] private Button nextRaceButton;


        [Space]

        [SerializeField] private CharacterRace[] humanoidRaces;
        [SerializeField] private CharacterRace[] demihumanRaces;
        [SerializeField] private CharacterRace[] monstrousRaces;

        [Space]
        [SerializeField] private RectTransform raceButtonParent;
        [SerializeField] private UIRaceSelectionPanel raceButtonPrefab;

        [SerializeField] private GameObject raceInfoPanel;
        [SerializeField] private TMP_Text racialStatHeader, racialStatField, racialDescriptionField;

        private CharacterRace chosenRace;
        private CharacterRace hybridSecondaryRace;

        private int currentIndex;
        private RacialType currentCategory;

        private void OnEnable()
        {
            if (chosenRace != null)
            {
                if (chosenRace.RaceType == RacialType.Humanoid) DisplayHumanoidRaces();
                else if (chosenRace.RaceType == RacialType.DemiHuman) DisplayDemiHumanRaces();
                else DisplayMonstrousRaces();
            }
            else
            {
                DisplayHumanoidRaces();
            }

            humanoidRacebutton.onClick.AddListener(DisplayHumanoidRaces);
            demiHumanRacebutton.onClick.AddListener(DisplayDemiHumanRaces);
            monstrousRacebutton.onClick.AddListener(DisplayMonstrousRaces);

            previousRaceButton.onClick.AddListener(DisplayPreviousRace);
            nextRaceButton.onClick.AddListener(DisplayNextRace);
            confirmRaceButton.onClick.AddListener(OnConfirmRace);
        }

        private void OnDisable()
        {
            humanoidRacebutton.onClick.RemoveAllListeners();
            demiHumanRacebutton.onClick.RemoveAllListeners();
            monstrousRacebutton.onClick.RemoveAllListeners();

            previousRaceButton.onClick.RemoveAllListeners();
            nextRaceButton.onClick.RemoveAllListeners();
            confirmRaceButton.onClick.RemoveAllListeners();
        }

        private void ResetButtons()
        {
            int count = raceButtonParent.transform.childCount;
            for (int i = count - 1; i >= 0; i--)
            {
                Destroy(raceButtonParent.transform.GetChild(i).gameObject);
            }
        }

        private void DisplayHumanoidRaces()
        {
            DisplayRacialOptions(RacialType.Humanoid, humanoidRaces);
        }

        private void DisplayDemiHumanRaces()
        {
            DisplayRacialOptions(RacialType.DemiHuman, demihumanRaces);
        }

        private void DisplayMonstrousRaces()
        {
            DisplayRacialOptions(RacialType.Monstrous, monstrousRaces);
        }

        private void DisplayRacialOptions(RacialType raceType, CharacterRace[] collection)
        {
            ResetButtons();
            for (int i = 0; i < collection.Length; i++)
            {
                int index = i;
                var race = collection[i];

                var raceOption = Instantiate(raceButtonPrefab);
                raceOption.transform.SetParent(raceButtonParent.transform, false);
                raceOption.image.sprite = race.RaceSprite;
                raceOption.text.text = race.RaceName;

                raceOption.button.onClick.AddListener(delegate
                {
                    OnRaceSelected(raceType, index);
                });
            }
        }

        private void OnRaceSelected(RacialType type, int index)
        {
            currentCategory = type;
            currentIndex = index;
            switch (type)
            {
                case RacialType.Humanoid:
                    chosenRace = humanoidRaces[index];
                    break;
                case RacialType.DemiHuman:
                    chosenRace = demihumanRaces[index];
                    break;
                case RacialType.Monstrous:
                    chosenRace = monstrousRaces[index];
                    break;
            }
            raceInfoPanel.SetActive(true);

            racialStatHeader.text = chosenRace.RaceName + "\n";

            //Archetype/Hierarchy
            if (chosenRace.Archetype is RacialChildArchetype child)
            {
                racialStatHeader.text += "(" + chosenRace.RaceType.ToString() + "/" + child.parentArchetype.ArchetypeName + "/" + child.ArchetypeName + ") " + chosenRace.RaceName;
            }
            else
            {
                racialStatHeader.text += "(" + chosenRace.RaceType.ToString() + "/" + chosenRace.Archetype.ArchetypeName + ") " + chosenRace.RaceName;
            }

            racialDescriptionField.text = chosenRace.RaceDescription;

            racialStatField.text = "";
            //Size
            racialStatField.text += "Size: " + chosenRace.Size.Name + "\n";

            //Needs
            if (chosenRace.Archetype.NeedsAir) racialStatField.text += "Breaths: Yes; ";
            else racialStatField.text += "Breaths: No; ";
            if (chosenRace.Archetype.NeedsFood) racialStatField.text += "Eats: Yes; ";
            else racialStatField.text += "Eats: No; ";
            if (chosenRace.Archetype.NeedsSleep) racialStatField.text += "Sleeps: Yes \n";
            else racialStatField.text += "Sleeps: No \n";

            int min = chosenRace.LifeSpan.modifier + chosenRace.LifeSpan.diceCount;
            int max = chosenRace.LifeSpan.modifier + (chosenRace.LifeSpan.diceCount * chosenRace.LifeSpan.diceSides);
            racialStatField.text += "Lifespan: " + min + " - " + max + "\n";

            foreach (var mod in chosenRace.RacialStats.AttributeModifiers)
            {
                char op = '+';
                if (mod.value < 0) op = '-';
                racialStatField.text += mod.attribute.ToString() + " : " + op + Mathf.Abs(mod.value) + "\n";
            }
            foreach (var mod in chosenRace.RacialStats.SkillModifiers)
            {
                char op = '+';
                if (mod.value < 0) op = '-';
                racialStatField.text += mod.skill.ToString() + " : " + op + Mathf.Abs(mod.value) + "\n";
            }
            foreach (var mod in chosenRace.RacialStats.ResistanceModifiers)
            {
                char op = '+';
                if (mod.value < 0) op = '-';
                racialStatField.text += mod.damageType.Name + " : " + op + Mathf.Abs(mod.value) + "\n";
            }
        }

        private void DisplayPreviousRace()
        {
            int index = currentIndex - 1;
            switch (currentCategory)
            {
                case RacialType.Humanoid:
                    if (index < 0) index = humanoidRaces.Length - 1;
                    break;
                case RacialType.DemiHuman:
                    if (index < 0) index = demihumanRaces.Length - 1;
                    break;
                case RacialType.Monstrous:
                    if (index < 0) index = monstrousRaces.Length - 1;
                    break;
            }
            OnRaceSelected(currentCategory, index);
        }

        private void DisplayNextRace()
        {
            int index = currentIndex + 1;
            switch (currentCategory)
            {
                case RacialType.Humanoid:
                    if (index >= humanoidRaces.Length) index = 0;
                    break;
                case RacialType.DemiHuman:
                    if (index >= demihumanRaces.Length) index = 0;
                    break;
                case RacialType.Monstrous:
                    if (index >= monstrousRaces.Length) index = 0;
                    break;
            }
            OnRaceSelected(currentCategory, index);
        }

        private void OnConfirmRace()
        {
            characterCreatorParent.SetRace(chosenRace);
        }
    }
}