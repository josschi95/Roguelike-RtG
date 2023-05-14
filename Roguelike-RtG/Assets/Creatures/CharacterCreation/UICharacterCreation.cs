using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JS.CharacterSystem.Creation;
using UnityEngine.Analytics;

namespace JS.CharacterSystem
{
    public class UICharacterCreation : MonoBehaviour
    {
        [SerializeField] private CharacterBuilder characterBuilder;

        [SerializeField] private GameObject[] categoryPanels;
        private int panelIndex = 0;

        public bool IsUndead { get; set; }
        public bool IsHybrid { get; set; }

        [SerializeField] private Button backButton;
        [SerializeField] private Button helpButton;
        [SerializeField] private Button nextButton;

        [Header("Character Sheet")]
        [SerializeField] private TMP_InputField nameInput;
        [SerializeField] private Image characterSprite;
        [SerializeField] private TMP_Text characterRaceText;
        [SerializeField] private TMP_Text characterArchetypeText;
        [SerializeField] private TMP_Text characterSizeText;
        [SerializeField] private TMP_Text characterGenderText;
        [SerializeField] private TMP_Text characterAgeText;

        [Space]

        [SerializeField] private TMP_Text descriptionText;

        private void Awake() => SetDefaults();

        private void Start()
        {
            backButton.onClick.AddListener(Back);

            nameInput.onSubmit.AddListener(delegate
            {
                characterBuilder.CharacterName = nameInput.text;
            });

            nextButton.onClick.AddListener(Next);
        }

        private void OnDestroy()
        {
            nameInput.onSubmit.RemoveAllListeners();
            nextButton.onClick.RemoveAllListeners();
        }

        private void SetDefaults()
        {
            //set race to human, gender male, age to young
       
        }

        //[Header("Character Class")]


        //[Header("Character Skills")]


        //[Header("Character Domain")]


        //[Header("Finalization")]

        public void Back()
        {
            OnRaceHoverExit();
            if (panelIndex == 0)
            {
                //return to main menu
            }
            else
            {
                DisplayCategoryPanel(panelIndex - 1);
            }
        }

        public void Next()
        {
            OnRaceHoverExit();
            if (panelIndex == categoryPanels.Length - 1)
            {
                //complete
            }
            else
            {
                DisplayCategoryPanel(panelIndex + 1);
            }
        }

        public void OnCharacterChanged()
        {
            SetRaceDependentText();
            characterGenderText.text = "Gender: " + characterBuilder.CharacterGender.ToString()[0];
        }

        private void DisplayCategoryPanel(int index)
        {
            for (int i = 0; i < categoryPanels.Length; i++)
            {
                categoryPanels[i].SetActive(false);
            }
            categoryPanels[index].SetActive(true);
            panelIndex = index;
        }

        public void SetRaceDependentText()
        {
            if (characterBuilder.PrimaryRace == null)
            {
                characterRaceText.text = "";
                characterArchetypeText.text = "";
                characterSizeText.text = "";
                return;
            }

            #region - Primary Race -
            var primary = characterBuilder.PrimaryRace;

            //Undead
            if (characterBuilder.IsUndead) characterRaceText.text = "Undead ";
            else characterRaceText.text = "";
            //Race
            characterRaceText.text += primary.RaceName;

            //Archetype
            if (primary.Archetype is CreatureSubTypes child)
            {
                characterArchetypeText.text = child.parentArchetype.ArchetypeName + " (" + child.ArchetypeName + ")";
            }
            else characterArchetypeText.text = primary.Archetype.ArchetypeName;

            //Size
            characterSizeText.text = primary.Size.Name;
            #endregion

            #region - Secondary Race -
            if (characterBuilder.SecondaryRace == characterBuilder.PrimaryRace) return;
            var secondary = characterBuilder.SecondaryRace;
            
            //Race
            characterRaceText.text += "/" + secondary.RaceName;

            //Archetype
            if (secondary.Archetype == primary.Archetype) return;

            if (secondary.Archetype is CreatureSubTypes child2)
            {
                characterArchetypeText.text += "/" + child2.parentArchetype.ArchetypeName + " (" + child2.ArchetypeName + ")";
            }
            else characterArchetypeText.text += "/" + secondary.Archetype.ArchetypeName;
            #endregion
        }

        public void OnRaceHover(CharacterRace race)
        {
            //Display info
            descriptionText.text = race.RaceDescription + "\n";

            //Archetype/Hierarchy
            if (race.Archetype is CreatureSubTypes child)
            {
                descriptionText.text += "(" + race.RaceCategory.ToString() + "/" + child.parentArchetype.ArchetypeName + "/" + child.ArchetypeName + ") " + race.RaceName + "\n";
            }
            else
            {
                descriptionText.text += "(" + race.RaceCategory.ToString() + "/" + race.Archetype.ArchetypeName + ") " + race.RaceName + "\n";
            }

            descriptionText.text += "Size: " + race.Size.Name + "\n";

            //Needs
            descriptionText.text += "Needs: ";
            if (race.Archetype.NeedsAir) descriptionText.text += "Air, ";
            //else descriptionText.text += "Breaths: No; ";
            if (race.Archetype.NeedsFood) descriptionText.text += "Food, ";
            //else descriptionText.text += "Eats: No; ";
            if (race.Archetype.NeedsSleep) descriptionText.text += "Sleep";
            //else descriptionText.text += "Sleeps: No \n";
            descriptionText.text += "\n";

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

        public void OnRaceHoverExit()
        {
            descriptionText.text = "";
        }
    }
}
public enum Gender { Male, Female, Other }