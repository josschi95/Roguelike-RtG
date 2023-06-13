using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JS.UI;

public class TimeDisplay : MonoBehaviour
{
    [SerializeField] private TimeKeeper timeKeeper;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private MoonPhaseHelper moonPhaseHelper;

    [SerializeField] private Image moonImage; 
    private void Start() => DisplayTime();

    //Called from GameEventListener on each GameTick
    public void UpdateDisplay() => DisplayTime();

    private void DisplayTime()
    {
        timeText.text = GetTimeOfDay() + ", " + timeKeeper.Days + timeKeeper.GetSuperScriptOrdinals(timeKeeper.Days) +
            " day of " + timeKeeper.Month.ToString() + ", " + timeKeeper.Year + timeKeeper.GetSuperScriptOrdinals(timeKeeper.Year) +
            " year";

        moonImage.sprite = moonPhaseHelper.GetMoonSprite();
    }

    private string GetTimeOfDay()
    {
        if (timeKeeper.Hours < 6) return "Night";
        if (timeKeeper.Hours < 12) return "Morning";
        if (timeKeeper.Hours < 18) return "Afternoon";
        return "Evening";
    }
}
