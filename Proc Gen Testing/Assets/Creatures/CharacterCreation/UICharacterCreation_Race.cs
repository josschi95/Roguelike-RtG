using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JS.CharacterSystem.Creation
{
    public class UICharacterCreation_Race : MonoBehaviour
    {
        private enum RaceMethod
        {
            Single,
            Maternal,
            Paternal,
        }
        private RaceMethod method;


        [SerializeField] private CharacterBuilder characterBuilder;
        [SerializeField] private UICharacterCreation characterCreatorParent;

        [Space]

        [SerializeField] private TMP_Text headerText;
        private TMP_Text crossbreedButtonText;

        [Header("Character Races")]
        [SerializeField] private Button humanoidRacebutton;
        [SerializeField] private Button demiHumanRacebutton;
        [SerializeField] private Button monstrousRacebutton;
        [SerializeField] private Button crossbreedbutton;

        [Space]

        [SerializeField] private CharacterRace[] humanoidRaces;
        [SerializeField] private CharacterRace[] demihumanRaces;
        [SerializeField] private CharacterRace[] monstrousRaces;

        [Space]

        [SerializeField] private RectTransform raceButtonParent;
        [SerializeField] private UIRaceSelectionPanel raceButtonPrefab;

        private void Awake()
        {
            crossbreedButtonText = crossbreedbutton.GetComponentInChildren<TMP_Text>();
        }

        private void OnEnable()
        {
            ResetRace();
            SetMethod(RaceMethod.Single);

            DisplayRacialOptions(humanoidRaces);

            humanoidRacebutton.onClick.AddListener(delegate
            {
                DisplayRacialOptions(humanoidRaces);
            });
            demiHumanRacebutton.onClick.AddListener(delegate
            {
                DisplayRacialOptions(demihumanRaces);
            });
            monstrousRacebutton.onClick.AddListener(delegate
            {
                DisplayRacialOptions(monstrousRaces);
            });
            crossbreedbutton.onClick.AddListener(delegate
            {
                ToggleCrossbreed();
                DisplayRacialOptions(humanoidRaces);
            });
        }

        private void OnDisable()
        {
            humanoidRacebutton.onClick.RemoveAllListeners();
            demiHumanRacebutton.onClick.RemoveAllListeners();
            monstrousRacebutton.onClick.RemoveAllListeners();
        }

        private void ResetButtons()
        {
            int count = raceButtonParent.transform.childCount;
            for (int i = count - 1; i >= 0; i--)
            {
                Destroy(raceButtonParent.transform.GetChild(i).gameObject);
            }
        }

        private void ToggleCrossbreed()
        {
            //Switch to Crossbreed
            if (method == RaceMethod.Single) SetMethod(RaceMethod.Maternal);
            //Switch to single race
            else SetMethod(RaceMethod.Single);

            ResetRace();
        }

        private void SetMethod(RaceMethod method)
        {
            this.method = method;
            if (method == RaceMethod.Single)
            {
                headerText.text = "Select Your Race";
                crossbreedButtonText.text = "Crossbreed";
            }
            else if (method == RaceMethod.Maternal)
            {
                headerText.text = "Select Maternal Race";
                crossbreedButtonText.text = "Cancel";
            }
            else
            {
                headerText.text = "Select Paternal Race";
                crossbreedButtonText.text = "Cancel";
            }
        }

        private void DisplayRacialOptions(CharacterRace[] collection)
        {
            ResetButtons();
            for (int i = 0; i < collection.Length; i++)
            {
                int index = i;
                var race = collection[index];

                if (!IsValidRace(race)) continue;
     
                var raceOption = Instantiate(raceButtonPrefab);
                raceOption.transform.SetParent(raceButtonParent.transform, false);

                raceOption.SetRace(race, characterCreatorParent);
                raceOption.button.onClick.AddListener(delegate
                {
                    OnConfirmRace(race);
                });
            }
        }

        private bool IsValidRace(CharacterRace race)
        {
            if (method == RaceMethod.Single) return true;

            if (race.ValidCrossBreeds.Length == 0) return false;

            if (method == RaceMethod.Maternal)
            {
                if (!race.HasFemale) return false;
                return true;
            }
            else if (!race.HasMales) return false;

            foreach(var validPair in characterBuilder.PrimaryRace.ValidCrossBreeds)
            {
                if (validPair == race) return true;
            }
            return false;
        }

        private void ResetRace()
        {
            characterBuilder.PrimaryRace = null;
            characterBuilder.SecondaryRace = null;
        }

        private void OnConfirmRace(CharacterRace race)
        {
            if (method == RaceMethod.Single)
            {
                characterBuilder.PrimaryRace = race;
                characterCreatorParent.Next();
            }
            else if (method == RaceMethod.Maternal)
            {
                characterBuilder.PrimaryRace = race;
                SetMethod(RaceMethod.Paternal);
            }
            else
            {
                characterBuilder.SecondaryRace = race;
                characterCreatorParent.Next();
            }
        }
    }
}