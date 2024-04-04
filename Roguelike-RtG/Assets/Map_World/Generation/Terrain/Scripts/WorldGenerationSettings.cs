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

        [Header("Latitude")]
        [SerializeField] private TMP_Text _latitudeText;
        [SerializeField] private Slider _southLatitudeSlider;
        [SerializeField] private Slider _northLatitudeSlider;

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
            generateWorldButton.onClick.AddListener(BeginWorldGeneration);

            // Seed
            randomizeSeedButton.onClick.AddListener(RandomizeSeed);
            seedInputField.onSubmit.AddListener(delegate
            {
                OnSetSeed(seedInputField.text);
            });

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

            // World Age
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

            // Latitude
            _southLatitudeSlider.onValueChanged.AddListener(OnSouthernLatitudeChange);
            _northLatitudeSlider.onValueChanged.AddListener(OnNorthernLatitudeChange);

            _southLatitudeSlider.minValue = -90;
            _southLatitudeSlider.maxValue = 90;
            _southLatitudeSlider.value = -90;
            _northLatitudeSlider.minValue = -90;
            _northLatitudeSlider.maxValue = 90;
            _northLatitudeSlider.value = 90;
        }

        private void SetDefaultValues()
        {
            RandomizeSeed();

            //OnSetWorldSize(2);
            //OnSetWorldAge(2);
            OnSetWorldSize(0);
            OnSetWorldAge(0);
            OnLatitudeChange();

            worldGenerator.SetNorthLatitude(_northLatitudeSlider.value);
            worldGenerator.SetSouthLatitude(_southLatitudeSlider.value);
        }

        private void ClearUIElementEvents()
        {
            generateWorldButton.onClick.RemoveAllListeners();
            randomizeSeedButton.onClick.RemoveAllListeners();

            for (int i = 0; i < worldSizeButtons.Length; i++)
            {
                worldSizeButtons[i].onClick.RemoveAllListeners();
            }

            for (int i = 0; i < worldAgeButtons.Length; i++)
            {
                worldAgeButtons[i].onClick.RemoveAllListeners();
            }

            _southLatitudeSlider.onValueChanged.RemoveAllListeners();
            _northLatitudeSlider.onValueChanged.RemoveAllListeners();
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

        private void OnSouthernLatitudeChange(float value)
        {
            if (_southLatitudeSlider.value > _northLatitudeSlider.value)
            {
                _southLatitudeSlider.value = _northLatitudeSlider.value;
            }
            worldGenerator.SetSouthLatitude(_southLatitudeSlider.value);
            OnLatitudeChange();
        }

        private void OnNorthernLatitudeChange(float value)
        {
            if (_northLatitudeSlider.value < _southLatitudeSlider.value)
            {
                _northLatitudeSlider.value = _southLatitudeSlider.value;
            }
            worldGenerator.SetNorthLatitude(_northLatitudeSlider.value);
            OnLatitudeChange();
        }

        private void OnLatitudeChange()
        {
            string south = string.Empty;
            string north = string.Empty;

            if (_southLatitudeSlider.value < 0) south = "S";
            else if (_southLatitudeSlider.value > 0) south = "N";
            if (_northLatitudeSlider.value < 0) north = "S";
            else if (_northLatitudeSlider.value > 0) north = "N";

            _latitudeText.text = $"{_southLatitudeSlider.value}\u00B0{south} / {_northLatitudeSlider.value}\u00B0{north}";
        }

        private void BeginWorldGeneration()
        {
            worldGenerator.OnBeginWorldGeneration();
        }
    }
}