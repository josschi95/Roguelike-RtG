using UnityEngine;
using JS.EventSystem;

[CreateAssetMenu(menuName = "Player/Game Settings")]
public class GameSettings : ScriptableObject
{
    [SerializeField] private GameEvent audioSettingsChangedEvent;

    #region - Graphics -
    [Header("Graphics")]
    [SerializeField] private FullScreenMode windowMode = FullScreenMode.FullScreenWindow;
    public bool FullScreen
    {
        get => windowMode == FullScreenMode.FullScreenWindow;
        set
        {
            if (value == true) windowMode = FullScreenMode.FullScreenWindow;
            else windowMode = FullScreenMode.Windowed;
            PlayerPrefs.SetInt("windowMode", IntToBool.GetInt(value));
            Screen.fullScreenMode = windowMode;
            
        }
    }
    #endregion

    #region - Audio -
    [Header("Audio Settings")]
    [Range(0, 100)]
    [SerializeField] private int masterVolumeModifier = 100;
    [Range(0, 100)]
    [SerializeField] private int musicVolumeModifier = 100;
    [Range(0, 100)]
    [SerializeField] private int sfxVolumeModifier = 100;
    
    public float MasterVolume
    {
        get => masterVolumeModifier;
        set
        {
            masterVolumeModifier = Mathf.Clamp(Mathf.RoundToInt(value), 0, 100);
            PlayerPrefs.SetInt("masterVolumeModifier", masterVolumeModifier);
            audioSettingsChangedEvent?.Invoke();
        }
    }

    public float MusicVolume
    {
        get => musicVolumeModifier;
        set
        {
            musicVolumeModifier = Mathf.Clamp(Mathf.RoundToInt(value), 0, 100);
            PlayerPrefs.SetInt("musicVolumeModifier", masterVolumeModifier);
            audioSettingsChangedEvent?.Invoke();
        }
    }

    public float SFXVolume
    {
        get => sfxVolumeModifier; 
        set
        {
            sfxVolumeModifier = Mathf.Clamp(Mathf.RoundToInt(value), 0, 100);
            PlayerPrefs.SetInt("sfxVolumeModifier", masterVolumeModifier);
            audioSettingsChangedEvent?.Invoke();
        }
    }
    #endregion

    #region - General -
    [Header("General")]
    [SerializeField] private AnimationMode animationMode = AnimationMode.Full;
    public AnimationMode AnimationMode
    {
        get => animationMode;
        set
        {
            animationMode = value;
            PlayerPrefs.SetInt("animationMode", (int)animationMode);
        }
    }

    [Range(25, 100)]
    [SerializeField] private int textSpeed = 50;
    public int TextSpeed
    {
        get => textSpeed;
        set
        {
            textSpeed = Mathf.Clamp(value, 25, 100);
            PlayerPrefs.SetInt("textSpeed", textSpeed);
        }
    }

    [SerializeField] private bool useMetric = true;
    public bool UseMetric
    {
        get => useMetric;
        set
        {
            useMetric = value;
            PlayerPrefs.SetInt("useMetric", IntToBool.GetInt(useMetric));
        }
    }
    #endregion

    public void LoadSavedValues()
    {
        //Graphics
        FullScreen = IntToBool.GetBool(PlayerPrefs.GetInt("windowMode", 1));

        //Audio
        MasterVolume = PlayerPrefs.GetInt("masterVolumeModifier", 100);
        MusicVolume = PlayerPrefs.GetInt("musicVolumeModifier", 100);
        SFXVolume = PlayerPrefs.GetInt("sfxVolumeModifier", 100);

        //General
        AnimationMode = (AnimationMode)PlayerPrefs.GetInt("animationMode", (int)AnimationMode.Full);
        TextSpeed = PlayerPrefs.GetInt("textSpeed", 50);
        UseMetric = IntToBool.GetBool(PlayerPrefs.GetInt("useMetric", 1));
    }
}

public enum AnimationMode
{
    Full,
    Instant,
    Off
}

//Notes : Other settings to add
/*
 * Resolution
 * Mouse Sensitivity? or Turn off Mouse completely?
 * In-game tips?
 * gore/cursing?
 * 
 */