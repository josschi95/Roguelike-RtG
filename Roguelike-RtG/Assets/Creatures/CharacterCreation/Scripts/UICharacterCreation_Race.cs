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


        [SerializeField] private CreatureCatalog creatureCatalog;
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

        [SerializeField] private RectTransform raceButtonParent;
        [SerializeField] private UISelectionElement selectionElement;

        private void Awake()
        {
            crossbreedButtonText = crossbreedbutton.GetComponentInChildren<TMP_Text>();
        }

        private void OnEnable()
        {
            ResetRace();
            SetMethod(RaceMethod.Single);

            DisplayRacialOptions(RacialCategory.Humanoid);

            humanoidRacebutton.onClick.AddListener(delegate
            {
                DisplayRacialOptions(RacialCategory.Humanoid);
            });
            demiHumanRacebutton.onClick.AddListener(delegate
            {
                DisplayRacialOptions(RacialCategory.Demihuman);
            });
            monstrousRacebutton.onClick.AddListener(delegate
            {
                DisplayRacialOptions(RacialCategory.Monstrous);
            });
            crossbreedbutton.onClick.AddListener(delegate
            {
                ToggleCrossbreed();
                DisplayRacialOptions(RacialCategory.Humanoid);
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

        private void DisplayRacialOptions(RacialCategory cat)
        {
            ResetButtons();
            foreach(var race in creatureCatalog.Races)
            {
                if (race.RaceCategory != cat) continue;
                if (!IsValidRace(race)) continue;

                var element = Instantiate(selectionElement);
                element.transform.SetParent(raceButtonParent.transform, false);
                element.Text.text = race.RaceName;


                element.onHoverEnter += delegate
                {
                    characterCreatorParent.OnRaceHover(race);
                };

                element.onHoverExit += characterCreatorParent.OnHoverExit;

                //element.SetRace(race, characterCreatorParent);
                element.Button.onClick.AddListener(delegate
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