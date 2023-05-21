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
            classAndRaceText.text = builder.PrimaryRace.RaceName;
            if (builder.SecondaryRace != builder.PrimaryRace)
            {
                classAndRaceText.text += "/" + builder.SecondaryRace.RaceName;
            }
            if ((int)builder.PrimaryRace.RaceCategory + (int)builder.SecondaryRace.RaceCategory == 0)
                classAndRaceText.text += " " + builder.Class.ClassName; //Only display class if not a racial class

            //Gender
            genderText.text = builder.CharacterGender.ToString();

            //Creature Type
            if (builder.IsUndead) creatureTypeText.text = "Undead";
            else creatureTypeText.text = builder.PrimaryRace.Type.TypeName;
            if (builder.SecondaryRace.Type != builder.PrimaryRace.Type)
                creatureTypeText.text += "/" + builder.SecondaryRace.Type.TypeName;

            //Lifespan
            lifespanText.text = builder.CharacterAge.ToString() + " years old,";
            if (!builder.PrimaryRace.LifeExpectancy.Ages && !builder.SecondaryRace.LifeExpectancy.Ages)
                lifespanText.text += " Ageless";
            else
            {
                var primary = builder.PrimaryRace.LifeExpectancy;
                var secondary = builder.SecondaryRace.LifeExpectancy;
                var max = Aging.GetMaxLifespan(primary, secondary);

                if (builder.CharacterAge >= Aging.GetVenerableAge(max))
                {
                    lifespanText.text += " Venerable";
                }
                else if (builder.CharacterAge >= Aging.GetOldAge(max))
                {
                    lifespanText.text += " Old";
                }
                else if (builder.CharacterAge >= Aging.GetMiddleAge(max))
                {
                    lifespanText.text += " Middle Age";
                }
                else if (builder.CharacterAge >= Aging.GetYoungAdultAge(max))
                {
                    lifespanText.text += " Young Adult";
                }
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

