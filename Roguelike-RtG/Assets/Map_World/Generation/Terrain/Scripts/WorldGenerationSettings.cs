using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JS.World.Map.Generation
{
    public class WorldGenerationSettings : MonoBehaviour
    {
        [SerializeField] private WorldGenerator worldGenerator;

        [Header("World Size")]
        [SerializeField] private Button[] worldSizeButtons;
        private Image[] worldSizeButtonGraphics;

        [Header("World Age")]
        [SerializeField] private Button[] worldAgeButtons;
        private Image[] worldAgeButtonGraphics;

        [Space]

        [SerializeField] private Button randomizeSeedButton;
        [SerializeField] private TMP_InputField seedInputField;

        [Space]

        [SerializeField] private Button generateWorldButton;

        [Space]

        [SerializeField] private Sprite _selectedImage;
        [SerializeField] private Sprite _disabledImage;

        private void OnEnable()
        {
            AssignUIElementEvents();
            SetDefaultValues();
        }

        private void OnDisable() => ClearUIElementEvents();

        private void AssignUIElementEvents()
        {
            // World Size
            worldSizeButtonGraphics = new Image[worldSizeButtons.Length];
            for (int i = 0; i < worldSizeButtons.Length; i++)
            {
                int size = i;
                worldSizeButtonGraphics[size] = worldSizeButtons[size].GetComponent<Image>();

                worldSizeButtons[size].onClick.AddListener(delegate 
                {
                    OnSetWorldSize(size); 
                });
            }

            worldAgeButtonGraphics = new Image[worldAgeButtons.Length];
            for (int i = 0; i < worldAgeButtons.Length; i++)
            {
                int age = i;
                worldAgeButtonGraphics[age] = worldAgeButtons[age].GetComponent<Image>();

                worldAgeButtons[age].onClick.AddListener(delegate
                {
                    OnSetWorldAge(age);
                });
            }


            randomizeSeedButton.onClick.AddListener(RandomizeSeed);
            seedInputField.onSubmit.AddListener(delegate
            {
                OnSetSeed(seedInputField.text);
            });
        }

        private void SetDefaultValues()
        {
            RandomizeSeed();

            //OnSetWorldSize(2);
            //OnSetWorldAge(2);
            OnSetWorldSize(0);
            OnSetWorldAge(0);
        }

        private void ClearUIElementEvents()
        {
            randomizeSeedButton.onClick.RemoveListener(RandomizeSeed);

            for (int i = 0; i < worldSizeButtons.Length; i++)
            {
                worldSizeButtons[i].onClick.RemoveAllListeners();
            }

            for (int i = 0; i < worldAgeButtons.Length; i++)
            {
                worldAgeButtons[i].onClick.RemoveAllListeners();
            }
        }

        private void OnSetSeed(string value)
        {
            Debug.Log(seedInputField.text);
            int intValue = int.Parse(value);
            intValue = Mathf.Clamp(intValue, 1, int.MaxValue);
            worldGenerator.SetSeed(intValue);
            OnSeedChanged();
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
            seedInputField.text = worldGenerator.Seed.ToString();
        }

        private void OnSetWorldSize(int size)
        {
            for (int i = 0;i < worldSizeButtonGraphics.Length; i++)
            {
                if (i == size) worldSizeButtonGraphics[i].sprite = _selectedImage;
                else worldSizeButtonGraphics[i].sprite = _disabledImage;
            }
            worldGenerator.SetSize(size);
        }

        private void OnSetWorldAge(int age)
        {
            for (int i = 0; i < worldAgeButtonGraphics.Length; i++)
            {
                if (i == age) worldAgeButtonGraphics[i].sprite = _selectedImage;
                else worldAgeButtonGraphics[i].sprite = _disabledImage;
            }
            worldGenerator.SetWorldAge(age);
        }
    }
}