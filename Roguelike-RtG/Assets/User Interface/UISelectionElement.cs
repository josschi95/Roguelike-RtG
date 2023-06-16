using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UISelectionElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void OnHoverCallback();
    public OnHoverCallback onHoverEnter;
    public OnHoverCallback onHoverExit;

    [field: SerializeField] public Button Button { get; private set; }
    [field: SerializeField] public TMP_Text Text { get; private set; }

    public void OnPointerEnter(PointerEventData eventData) => onHoverEnter?.Invoke();

    public void OnPointerExit(PointerEventData eventData) => onHoverExit?.Invoke();

    private void OnDestroy()
    {
        Button.onClick.RemoveAllListeners();
        onHoverEnter = null;
        onHoverExit = null;
    }
}