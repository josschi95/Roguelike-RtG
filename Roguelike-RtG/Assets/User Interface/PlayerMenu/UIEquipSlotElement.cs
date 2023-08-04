using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIEquipSlotElement : MonoBehaviour
{
    [SerializeField] private TMP_Text slotName, itemName;
    public TMP_Text slotText => slotName;
    public TMP_Text itemText => itemName;
    [SerializeField] private Button button;
    public Button Button => button;

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
