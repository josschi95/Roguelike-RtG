using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JS.CharacterSystem.Creation
{
    public class UICharacterCreation_Race : MonoBehaviour
    {
        [SerializeField] private CreatureCatalog creatureCatalog;
        [SerializeField] private CharacterBuilder characterBuilder;
        [SerializeField] private UICharacterCreation characterCreatorParent;

        [Space]

        [SerializeField] private TMP_Text headerText;

        [Header("Character Races")]
        [SerializeField] private Button humanoidRacebutton;
        [SerializeField] private Button demiHumanRacebutton;
        [SerializeField] private Button monstrousRacebutton;

        [Space]

        [SerializeField] private RectTransform raceButtonParent;
        [SerializeField] private UISelectionElement selectionElement;

        private void OnEnable()
        {
            ResetRace();

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
        }

        private void OnDisable()
        {
            humanoidRacebutton.onClick.RemoveAllListeners();
            demiHumanRacebutton.onClick.RemoveAllListeners();
            monstrousRacebutton.onClick.RemoveAllListeners();
        }

        private void ResetButtons()
        {
            for (int i = raceButtonParent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(raceButtonParent.transform.GetChild(i).gameObject);
            }
        }

        private void DisplayRacialOptions(RacialCategory cat)
        {
            ResetButtons();
            foreach(var race in creatureCatalog.Races)
            {
                if (race.RaceCategory != cat) continue;

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

        private void ResetRace()
        {
            characterBuilder.Race = null;
        }

        private void OnConfirmRace(CharacterRace race)
        {
            characterBuilder.Race = race;
            characterCreatorParent.Next();
        }
    }
}