using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JS.CharacterCreation
{
    public class UIRaceSelectionPanel : MonoBehaviour
    {
        public Image image;
        public Button button;
        public TMP_Text text;

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}