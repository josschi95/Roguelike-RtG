using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles button press events in the MainMenu scene. Temporary.
/// </summary>
public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private Button confirmQuitButton;

    private void OnEnable() => SetButtonEvents();
    private void OnDisable() => ClearButtonEvents();

    private void SetButtonEvents()
    {

        confirmQuitButton.onClick.AddListener(OnConfirmQuitGame);
    }

    private void ClearButtonEvents()
    {

        confirmQuitButton.onClick.RemoveListener(OnConfirmQuitGame);
    }


    private void OnConfirmQuitGame()
    {
        Application.Quit();
    }
}
