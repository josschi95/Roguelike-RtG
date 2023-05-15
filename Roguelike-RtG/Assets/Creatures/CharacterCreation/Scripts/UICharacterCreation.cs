using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JS.EventSystem;

namespace JS.CharacterSystem.Creation
{
    public class UICharacterCreation : MonoBehaviour
    {
        private enum CreationPanel
        {
            Race,
            Variants,
            Class,
            Skills,
            Domain,
            Final,
        }

        [SerializeField] private CharacterBuilder characterBuilder;
        [SerializeField] private GameEvent returnToMainMenuEvent;
        [SerializeField] private GameEvent completeCharacterCreationEvent;

        [SerializeField] private GameObject[] categoryPanels;
        private CreationPanel panelIndex = 0;

        public bool IsUndead { get; set; }
        public bool IsHybrid { get; set; }

        [SerializeField] private Button backButton;
        [SerializeField] private Button helpButton;
        [SerializeField] private Button nextButton;

        [Header("Character Sheet")]
        [SerializeField] private TMP_InputField nameInput;
        [SerializeField] private Image characterSprite;
        [SerializeField] private TMP_Text raceText;
        [SerializeField] private TMP_Text typeText;
        [SerializeField] private TMP_Text subtypeText;
        [SerializeField] private TMP_Text sizeText;
        [SerializeField] private TMP_Text genderText;
        [SerializeField] private TMP_Text ageText;

        [Space]

        [SerializeField] private TMP_Text infoText;

        private void Awake() => SetDefaults();

        private void Start()
        {
            backButton.onClick.AddListener(Back);

            nameInput.onSubmit.AddListener(delegate
            {
                characterBuilder.CharacterName = nameInput.text;
            });

        }

        private void OnDestroy()
        {
            nameInput.onSubmit.RemoveAllListeners();
        }

        private void SetDefaults()
        {
            ClearFields();
            DisplayCategoryPanel(0);
            characterBuilder.ResetValues();

            //set race to human, gender male, age to young
       
        }

        private void ClearFields()
        {
            raceText.text = "";
            typeText.text = "";
            subtypeText.text = "";
            sizeText.text = "";
        }

        #region - Navigation -
        private void Back()
        {
            switch (panelIndex)
            {
                case CreationPanel.Race:
                    OnHoverExit();
                    returnToMainMenuEvent?.Invoke();
                    break;
                case CreationPanel.Variants:
                    DisplayCategoryPanel((int)panelIndex - 1);
                    nextButton.gameObject.SetActive(false);
                    break;
                case CreationPanel.Class:
                    DisplayCategoryPanel((int)panelIndex - 1);
                    nextButton.gameObject.SetActive(true);
                    break;
                case CreationPanel.Skills:
                    //Skip class if character has a non-humanoid race
                    if ((int)characterBuilder.PrimaryRace.RaceCategory + 
                        (int)characterBuilder.SecondaryRace.RaceCategory > 0)
                    {
                        DisplayCategoryPanel((int)panelIndex - 2);
                    }
                    else DisplayCategoryPanel((int)panelIndex - 1);
                    break;
                case CreationPanel.Domain:
                    DisplayCategoryPanel((int)panelIndex - 1);
                    break;
                case CreationPanel.Final:
                    DisplayCategoryPanel((int)panelIndex - 1);
                    break;
            }
        }

        public void Next()
        {
            switch (panelIndex)
            {
                case CreationPanel.Race:
                    OnHoverExit();
                    DisplayCategoryPanel((int)panelIndex + 1);
                    nextButton.gameObject.SetActive(true);
                    break;
                case CreationPanel.Variants:
                    //Skip class if character has a non-humanoid race
                    if ((int)characterBuilder.PrimaryRace.RaceCategory +
                        (int)characterBuilder.SecondaryRace.RaceCategory > 0)
                    {
                        DisplayCategoryPanel((int)panelIndex + 2);
                    }
                    else DisplayCategoryPanel((int)panelIndex + 1);
                    nextButton.gameObject.SetActive(false);
                    break;
                case CreationPanel.Class:
                    DisplayCategoryPanel((int)panelIndex + 1);
                    break;
                case CreationPanel.Skills:
                    DisplayCategoryPanel((int)panelIndex + 1);
                    break;
                case CreationPanel.Domain:
                    DisplayCategoryPanel((int)panelIndex + 1);
                    break;
                case CreationPanel.Final:
                    completeCharacterCreationEvent?.Invoke();
                    break;
            }
        }

        private void DisplayCategoryPanel(int index)
        {
            for (int i = 0; i < categoryPanels.Length; i++)
            {
                categoryPanels[i].SetActive(false);
            }
            categoryPanels[index].SetActive(true);
            panelIndex = (CreationPanel)index;
        }
        #endregion

        public void OnCharacterChanged()
        {
            SetRaceDependentText();
            genderText.text = "Gender: " + characterBuilder.CharacterGender.ToString()[0];
        }

        public void SetRaceDependentText()
        {
            if (characterBuilder.PrimaryRace == null)
            {
                ClearFields();
                return;
            }

            #region - Primary Race -
            var primary = characterBuilder.PrimaryRace;

            //Race
            raceText.text = primary.RaceName;
            
            //Type
            if (characterBuilder.IsUndead) typeText.text = "Type: Undead";
            else typeText.text = "Type: " + primary.Type.TypeName;
            
            //Subtypes
            subtypeText.text = "Subtypes:";
            for (int i = 0; i < primary.Subtypes.Length; i++)
            {
                subtypeText.text += " " + primary.Subtypes[i].ArchetypeName + ",";
            }

            //Size
            sizeText.text = primary.Size.Name;
            #endregion

            #region - Secondary Race -
            if (characterBuilder.SecondaryRace == characterBuilder.PrimaryRace)
            {
                subtypeText.text = subtypeText.text.Substring(0, subtypeText.text.Length - 1);
                return;
            }
            var secondary = characterBuilder.SecondaryRace;
            
            //Race
            raceText.text += "/" + secondary.RaceName;
            
            //Type
            if (!characterBuilder.IsUndead && secondary.Type != primary.Type)
                typeText.text += "/" + secondary.Type.TypeName;
            
            //Subtypes
            for (int i = 0; i < secondary.Subtypes.Length; i++)
            {
                subtypeText.text += " " + secondary.Subtypes[i].ArchetypeName + ",";
            }
            subtypeText.text = subtypeText.text.Substring(0, subtypeText.text.Length - 1);

            #endregion
        }

        public void OnRaceHover(CharacterRace race)
        {
            //Type
            infoText.text = race.RaceName + "\n" + race.Type.TypeName;
            if (race.RaceCategory == RacialCategory.Monstrous) 
                infoText.text += " (" + race.RaceCategory.ToString() + ")";
            infoText.text += "\n";

            //Subtypes
            for (int i = 0; i < race.Subtypes.Length; i++)
            {
                if (i > 0) infoText.text += " ";
                infoText.text += race.Subtypes[i].ArchetypeName + ",";
            }
            if (race.Subtypes.Length > 0)
                infoText.text = infoText.text.Substring(0, 
                    infoText.text.Length - 1);
            infoText.text += "\n";

            //Size
            infoText.text += "Size: " + race.Size.Name + "\n";

            //Needs
            infoText.text += "Needs:";
            if (race.Type.NeedsAir) infoText.text += " Air,";
            if (race.Type.NeedsFood) infoText.text += " Food,";
            if (race.Type.NeedsSleep) infoText.text += " Sleep,";
            infoText.text = infoText.text.Substring(0, infoText.text.Length - 1);
            infoText.text += "\n";

            //Lifespan
            infoText.text += "Lifespan: " + race.LifeExpectancy.MinLifeExpectancy + 
                " - " + race.LifeExpectancy.MaxLifeExpectancy + " years\n";

            //Attribute Modifiers
            foreach (var mod in race.RacialStats.AttributeModifiers)
            {
                char op = '+';
                if (mod.value < 0) op = '-';
                infoText.text += mod.attribute.ToString() + " : " + op + Mathf.Abs(mod.value) + "\n";
            }
            //Skill Modifiers
            foreach (var mod in race.RacialStats.SkillModifiers)
            {
                char op = '+';
                if (mod.value < 0) op = '-';
                infoText.text += mod.skill.ToString() + " : " + op + Mathf.Abs(mod.value) + "\n";
            }
            //Resistance Modifiers
            foreach (var mod in race.RacialStats.ResistanceModifiers)
            {
                char op = '+';
                if (mod.value < 0) op = '-';
                infoText.text += mod.damageType.Name + " : " + op + Mathf.Abs(mod.value) + "\n";
            }

            //Display info
            infoText.text += race.RaceDescription;
        }

        public void OnClassHover(CharacterClass characterClass)
        {
            //name
            infoText.text = characterClass.ClassName + "\n";
            
            //category/tier
            infoText.text += characterClass.Archetype.ToString() + 
                " (" + characterClass.Tier.Name + ")\n";

            //description
            infoText.text += characterClass.Description;
        }

        public void OnHoverExit()
        {
            infoText.text = "";
        }
    }
}
public enum Gender { Male, Female, Other }