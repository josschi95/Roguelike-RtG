using UnityEngine;
using UnityEngine.UI;

public class UIPlayerMenuNavigation : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private GameObject[] panels;

    private void OnEnable()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[index].onClick.AddListener(delegate
            {
                SetActivePanel(index);
            });
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].onClick.RemoveAllListeners();
        }
    }

    private void SetActivePanel(int index)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if (i == index) panels[i].SetActive(true);
            else panels[i].SetActive(false);
        }
    }
}
