using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameSettings gameSettings;

    [Space]

    [SerializeField] private GameObject confirmChangesPanel;
    [SerializeField] private Button saveChangesButton;
    [SerializeField] private Button discardChangesButton;

    [Header("Graphics")]
    [SerializeField] private GameObject graphicsPanel;
    [SerializeField] private Button saveGraphicsButton;
    [SerializeField] private Button closeGraphicsButton;
    [Space]
    [SerializeField] private Toggle fullScreenToggle;

    [Header("Audio")]
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private Button saveAudioButton;
    [SerializeField] private Button closeAudioButton;
    [Space]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("General")]
    [SerializeField] private GameObject generalPanel;
    [SerializeField] private Button saveGeneralButton;
    [SerializeField] private Button closeGeneralButton;
    [Space]
    [SerializeField] private TMP_Dropdown animationDropdown;
    [SerializeField] private Slider textSpeedSlider;
    [SerializeField] private Toggle useMetricToggle;

    private void OnEnable()
    {
        UpdateUIElements();
        saveGraphicsButton.onClick.AddListener(SaveGraphics);
        saveAudioButton.onClick.AddListener(SaveAudio);
        saveGeneralButton.onClick.AddListener(SaveGeneral);

        closeGraphicsButton.onClick.AddListener(CloseGraphicsPanel);
        closeAudioButton.onClick.AddListener(CloseAudioPanel);
        closeGeneralButton.onClick.AddListener(CloseGeneralPanel);

        saveChangesButton.onClick.RemoveAllListeners();
        discardChangesButton.onClick.RemoveAllListeners();

        int count = System.Enum.GetNames(typeof(AnimationMode)).Length;
        List<string> options = new List<string>();

        animationDropdown.ClearOptions();
        for (int i = 0; i < count; i++)
        {
            options.Add(((AnimationMode)i).ToString());
        }
        animationDropdown.AddOptions(options);
    }

    private void OnDisable()
    {
        saveGraphicsButton.onClick.RemoveAllListeners();
        saveAudioButton.onClick.RemoveAllListeners();
        saveGeneralButton.onClick.RemoveAllListeners();

        closeGraphicsButton.onClick.RemoveAllListeners();
        closeAudioButton.onClick.RemoveAllListeners();
        closeGeneralButton.onClick.RemoveAllListeners();

        saveChangesButton.onClick.RemoveAllListeners();
        discardChangesButton.onClick.RemoveAllListeners();
    }
    
    private void UpdateUIElements()
    {
        //Graphics
        fullScreenToggle.isOn = gameSettings.FullScreen;

        //Audio
        masterVolumeSlider.value = gameSettings.MasterVolume;
        musicVolumeSlider.value = gameSettings.MusicVolume;
        sfxVolumeSlider.value = gameSettings.SFXVolume;

        //General
        animationDropdown.value = (int)gameSettings.AnimationMode;
        textSpeedSlider.value = gameSettings.TextSpeed;
        useMetricToggle.isOn = gameSettings.UseMetric;
    }

    private void ResetConfirmChangesPanel()
    {
        confirmChangesPanel.SetActive(true);
        saveChangesButton.onClick.RemoveAllListeners();
        discardChangesButton.onClick.RemoveAllListeners();
    }

    private void DiscardChanges()
    {
        UpdateUIElements();
        confirmChangesPanel.SetActive(false);
    }

    #region - Graphics -
    private void CloseGraphicsPanel()
    {
        if (!GraphicsHaveChanged()) graphicsPanel.gameObject.SetActive(false);
        else
        {
            ResetConfirmChangesPanel();
            saveChangesButton.onClick.AddListener(SaveGraphics);
            discardChangesButton.onClick.AddListener(delegate
            {
                DiscardChanges();
                graphicsPanel.SetActive(false);
            });
        }
    }

    private bool GraphicsHaveChanged()
    {
        if (gameSettings.FullScreen != fullScreenToggle.isOn) return true;
        return false;
    }

    private void SaveGraphics()
    {
        gameSettings.FullScreen = fullScreenToggle.isOn;

        graphicsPanel.SetActive(false);
        confirmChangesPanel.SetActive(false);
    }
    #endregion

    #region - Audio -
    private void CloseAudioPanel()
    {
        if (!AudioHasChanged()) audioPanel.gameObject.SetActive(false);
        else
        {
            ResetConfirmChangesPanel();
            saveChangesButton.onClick.AddListener(SaveAudio);
            discardChangesButton.onClick.AddListener(delegate
            {
                DiscardChanges();
                audioPanel.SetActive(false);
            });
        }
    }

    private bool AudioHasChanged()
    {
        if (gameSettings.MasterVolume != masterVolumeSlider.value) return true;
        if (gameSettings.MusicVolume != musicVolumeSlider.value) return true;
        if (gameSettings.SFXVolume != sfxVolumeSlider.value) return true;
        return false;
    }

    private void SaveAudio()
    {
        gameSettings.MasterVolume = masterVolumeSlider.value;
        gameSettings.MusicVolume = musicVolumeSlider.value;
        gameSettings.SFXVolume = sfxVolumeSlider.value;

        audioPanel.SetActive(false);
        confirmChangesPanel.SetActive(false);
    }
    #endregion

    #region - General - 
    private void CloseGeneralPanel()
    {
        if (!GeneralHasChanged()) generalPanel.gameObject.SetActive(false);
        else
        {
            ResetConfirmChangesPanel();
            saveChangesButton.onClick.AddListener(SaveGeneral);
            discardChangesButton.onClick.AddListener(delegate
            {
                DiscardChanges();
                generalPanel.SetActive(false);
            });
        }
    }

    private bool GeneralHasChanged()
    {
        if (gameSettings.AnimationMode != (AnimationMode)animationDropdown.value) return true;
        if (gameSettings.TextSpeed != textSpeedSlider.value) return true;
        if (gameSettings.UseMetric != useMetricToggle.isOn) return true;
        return false;
    }

    private void SaveGeneral()
    {
        gameSettings.AnimationMode = (AnimationMode)animationDropdown.value;
        gameSettings.TextSpeed = Mathf.RoundToInt(textSpeedSlider.value);
        gameSettings.UseMetric = useMetricToggle.isOn;

        generalPanel.SetActive(false);
        confirmChangesPanel.SetActive(false);
    }
    #endregion
}
