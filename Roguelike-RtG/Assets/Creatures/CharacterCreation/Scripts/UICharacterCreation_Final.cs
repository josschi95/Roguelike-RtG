using UnityEngine;
using TMPro;

namespace JS.CharacterSystem.Creation
{
    public class UICharacterCreation_Final : MonoBehaviour
    {
        [SerializeField] private CharacterBuilder builder;

        [SerializeField] private TMP_Text classAndRaceText;
        [SerializeField] private TMP_Text creatureTypeText;
        [SerializeField] private TMP_Text genderText;
        [SerializeField] private TMP_Text lifespanText;
        [SerializeField] private TMP_Text domainText;
        [SerializeField] private TMP_Text[] attributeValuesTexts;


        private void OnEnable() => DisplayCharacterInfo();

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

