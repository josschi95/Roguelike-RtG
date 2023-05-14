using UnityEngine;
using UnityEngine.UI;
using JS.WorldGeneration;
using TMPro;

public class WorldGenerationSettings : MonoBehaviour
{
    [SerializeField] private WorldGenerator worldGenerator;

    [Space] [Space]

    [SerializeField] private Button[] worldSizeButtons;

    [Space]

    [SerializeField] private Button randomizeSeedButton;
    [SerializeField] private TMP_InputField seedInputField;

    [Space]

    [SerializeField] private Button generateWorldButton;

    private void OnEnable() => AssignUIElementEvents();
    private void OnDisable() => ClearUIElementEvents();


    private void AssignUIElementEvents()
    {
        for (int i = 0; i < worldSizeButtons.Length; i++)
        {
            int size = i;
            worldSizeButtons[size].onClick.AddListener(delegate { worldGenerator.SetSize(size); });
        }

        RandomizeSeed();

        randomizeSeedButton.onClick.AddListener(RandomizeSeed);
        seedInputField.onSubmit.AddListener(delegate { Debug.Log(seedInputField.text); });
    }

    private void ClearUIElementEvents()
    {
        randomizeSeedButton.onClick.RemoveListener(RandomizeSeed);

        for (int i = 0; i < worldSizeButtons.Length; i++)
        {
            worldSizeButtons[i].onClick.RemoveAllListeners();
        }
    }



    //Randomizes seed and updates display
    private void RandomizeSeed()
    {
        worldGenerator.SetRandomSeed();
        OnSeedChanged();
    }

    //Updates current seed display
    private void OnSeedChanged()
    {
        seedInputField.text = worldGenerator.seed.ToString();
    }
}
