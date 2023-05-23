using JS.CommandSystem;
using JS.WorldMap;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles button press events in the MainMenu scene. Temporary.
/// </summary>
public class MainMenuHandler : MonoBehaviour
{
    public bool characterSaveExists;
    [SerializeField] private WorldData worldMapData;
    private bool worldSaveExists;
    
    [Space]

    [SerializeField] private LoadSavedWorldCommand loadWorldCommand;
    [SerializeField] private EmptyCommand newWorldCommand;
    [SerializeField] private EmptyCommand newCharacterCommand;

    [Space]

    [SerializeField] private Button newWorldButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button confirmQuitButton;

    [Space]

    [SerializeField] private GameObject confirmNewWorldPanel;

    private void OnEnable() => Init();
    private void OnDisable() => ClearButtonEvents();

    private void Init()
    {
        loadWorldCommand?.Invoke();
        confirmNewWorldPanel.SetActive(false);

        worldSaveExists = worldMapData.SaveExists;

        //continueButton.interactable = characterSaveExists;
        continueButton.interactable = worldSaveExists; //Swapped for testing

        if (worldSaveExists)
        {
            newWorldButton.onClick.AddListener(delegate
            {
                confirmNewWorldPanel.SetActive(true);
            });
        }
        else
        {
            newWorldButton.onClick.AddListener(delegate
            {
                newWorldCommand?.Invoke();
            });
        }



        confirmQuitButton.onClick.AddListener(OnConfirmQuitGame);
    }

    private void ClearButtonEvents()
    {
        newWorldButton.onClick.RemoveAllListeners();
        confirmQuitButton.onClick.RemoveListener(OnConfirmQuitGame);
    }

    private void OnConfirmQuitGame()
    {
        Application.Quit();
    }
}
