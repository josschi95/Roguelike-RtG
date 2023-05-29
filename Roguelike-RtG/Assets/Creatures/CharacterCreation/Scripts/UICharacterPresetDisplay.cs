using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JS.CharacterSystem.Creation
{
    public class UICharacterPresetDisplay : MonoBehaviour
    {
        [SerializeField] private CreaturePresetSO[] defaultPresets;
        [SerializeField] private CustomCharacterPresets playerPresets;
        [SerializeField] private CharacterBuilder builder;
        [SerializeField] private CreatureCatalog catalog;

        [Space]

        [SerializeField] private RectTransform panelParent;
        [SerializeField] private UISelectionElement selectionElement;

        [Space]

        [SerializeField] private TMP_Text classAndRaceText;
        [SerializeField] private TMP_Text creatureTypeText;
        [SerializeField] private TMP_Text genderText;
        [SerializeField] private TMP_Text lifespanText;
        [SerializeField] private TMP_Text domainText;
        [SerializeField] private TMP_Text[] attributeValuesTexts;

        private void ClearPanel()
        {
            for (int i = panelParent.childCount - 1; i >= 0; i--)
            {
                Destroy(panelParent.GetChild(i).gameObject);
            }
        }

        public void DisplayDefaults()
        {
            ClearPanel();

            foreach(var preset in defaultPresets)
            {
                var element = Instantiate(selectionElement);
                element.transform.SetParent(panelParent.transform, false);

                element.Text.text = preset.Name;
                element.Button.onClick.AddListener(delegate
                {
                    SetValues(preset);
                    DisplayCharacterInfo();
                });
            }
        }

        public void DisplayPlayerPresets()
        {
            ClearPanel();

            foreach(var preset in playerPresets.presets)
            {
                var element = Instantiate(selectionElement);
                element.transform.SetParent(panelParent.transform, false);

                element.Text.text = preset.presetName;
                element.Button.onClick.AddListener(delegate
                {
                    SetValues(preset);
                    DisplayCharacterInfo();
                });
            }
        }

        private void SetValues(CreaturePresetSO preset)
        {
            builder.CharacterName = preset.Name;
            builder.Race = preset.Race;
            builder.CharacterGender = preset.CharacterGender;
            builder.IsUndead = preset.IsUndead;
            builder.CharacterAge = preset.CharacterAge;
            builder.Domain = preset.Domain;
            for (int i = 0; i < preset.AttributeValues.Length; i++)
            {
                builder.SetAttributeValue(i, preset.AttributeValues[i].value);
            }
        }

        private void SetValues(CreaturePresetData preset)
        {
            builder.CharacterName = preset.presetName;
            builder.Race = catalog.GetRace(preset.raceID);
            builder.Class = catalog.GetClass(preset.classID);
            builder.Domain = catalog.GetDomain(preset.domainID);
            builder.CharacterGender = (CharacterGender)preset.gender;
            builder.IsUndead = preset.isUndead;
            builder.CharacterAge = preset.age;
            for (int i = 0; i < preset.attributeValues.Length; i++)
            {
                builder.SetAttributeValue(i, preset.attributeValues[i]);
            }
        }

        private void DisplayCharacterInfo()
        {
            //Class and Race
            classAndRaceText.text = builder.Race.RaceName;

            //Only display class if not a racial class
            if (builder.Race.RaceCategory == RacialCategory.Humanoid)
                classAndRaceText.text += " " + builder.Class.ClassName;

            //Gender
            genderText.text = builder.CharacterGender.ToString();

            //Creature Type
            if (builder.IsUndead) creatureTypeText.text = "Undead";
            else creatureTypeText.text = builder.Race.Type.TypeName;

            //Lifespan
            lifespanText.text = builder.CharacterAge.ToString() + " years old,";
            if (!builder.Race.LifeExpectancy.Ages) lifespanText.text += " Ageless";
            else
            {
                var life = builder.Race.LifeExpectancy;
                if (builder.CharacterAge >= life.VenerableAge) lifespanText.text += " Venerable";
                else if (builder.CharacterAge >= life.OldAge) lifespanText.text += " Old";
                else if (builder.CharacterAge >= life.MiddleAge) lifespanText.text += " Middle Age";
                else if (builder.CharacterAge >= life.YoungAdultAge) lifespanText.text += " Young Adult";
            }

            //Domain
            domainText.text = builder.Domain.name;

            //Attributes
            for (int i = 0; i < attributeValuesTexts.Length; i++)
            {
                attributeValuesTexts[i].text = builder.AttributeValues[i].ToString();
            }


        }
    }
}