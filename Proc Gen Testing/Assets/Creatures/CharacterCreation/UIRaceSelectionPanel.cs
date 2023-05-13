using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace JS.CharacterSystem.Creation
{
    public class UIRaceSelectionPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public UICharacterCreation characterCreator { get; set; }
        public CharacterRace characterRace { get; set; }

        public Button button;
        public TMP_Text text;

        public void SetRace(CharacterRace race, UICharacterCreation creator)
        {
            characterRace = race;
            text.text = race.RaceName;
            characterCreator = creator;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            characterCreator.OnRaceHover(characterRace);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            characterCreator.OnRaceHoverExit();
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
            characterCreator = null;
            characterRace = null;
        }
    }
}