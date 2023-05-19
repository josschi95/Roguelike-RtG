using UnityEngine;
using TMPro;

namespace JS.CharacterSystem.Creation
{
    public class UICharacterCreation_Final : MonoBehaviour
    {
        [SerializeField] private CharacterBuilder builder;

        [SerializeField] private TMP_Text genderRaceText;
        [SerializeField] private TMP_Text lifespanText;

        private void OnEnable() => DisplayCharacterInfo();

        private void DisplayCharacterInfo()
        {
            //Gender and Race
            genderRaceText.text = builder.CharacterGender.ToString() + " ";
            if (builder.IsUndead) genderRaceText.text += "Undead ";
            if (builder.PrimaryRace == builder.SecondaryRace)
            {
                genderRaceText.text += builder.PrimaryRace.RaceName;
            }
            else
            {
                genderRaceText.text += builder.PrimaryRace.RaceName + "/" + builder.SecondaryRace.RaceName;
            }

            lifespanText.text = builder.CharacterAge.ToString();
            if (!builder.PrimaryRace.LifeExpectancy.Ages && !builder.SecondaryRace.LifeExpectancy.Ages) lifespanText.text += " (Ageless)";
            else
            {

            }
        }
    }
}

