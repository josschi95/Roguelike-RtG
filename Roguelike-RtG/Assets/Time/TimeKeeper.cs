using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Time Keeper")]
public class TimeKeeper : ScriptableObject
{
    #region - Constants -
    private const int localTickTime = 1;
    private const int worldTickTime = 360;

    private const int secondsInMinute = 60;
    private const int minutesInHour = 60;
    private const int hoursInDay = 24;
    private const int daysInMonth = 30;
    private const int monthsInYear = 12;
    private const int daysInYear = 360;
    #endregion

    #region - Time Values -
    [SerializeField] private int _seconds;
    [SerializeField] private int _minutes;
    [SerializeField] private int _hours;
    [SerializeField] private int _days = 1;
    [SerializeField] private int _months = 1;
    [SerializeField] private int _years;

    [Space] 
    [SerializeField] private MoonPhase _phase;
    [SerializeField] private Season _season;

    public int Seconds => _seconds;
    public int Minutes => _minutes;
    public int Hours => _hours;
    public int Days => _days;
    public int Months => _months;
    public int Years => _years;
    public MoonPhase MoonPhase => _phase;
    public Season Season => _season;
    #endregion

    private bool _local;
    
    //Called from GameEventListener when WorldMap scene is loaded
    public void OnWorldMap()
    {
        _local = false;
    }

    //Called from GameEventListener when LocalMap scene is loaded
    public void OnLocalMap()
    {
        _local = true;
    }

    #region - Clock -
    //Called from GameEventListener when a full game round passes
    public void OnGameTick()
    {
        if (_local) OnSecondsChange(localTickTime);
        else OnSecondsChange(worldTickTime);
    }

    private void OnSecondsChange(int seconds)
    {
        _seconds += seconds;
        while(_seconds >= secondsInMinute)
        {
            _seconds -= secondsInMinute;
            OnMinutesChange();
        }
    }

    private void OnMinutesChange(int minutes = 1) 
    {  
        _minutes += minutes;
        while (_minutes >= minutesInHour)
        {
            _minutes -= minutesInHour;
            OnHoursChange();
        }
    }

    private void OnHoursChange(int hours = 1)
    {
        _hours += hours;
        while(_hours >= hoursInDay)
        {
            _hours -= hoursInDay;
            OnDaysChange();
        }
    }

    private void OnDaysChange(int days = 1)
    {
        _days += days;
        _phase = GetMoonPhase();
        while( _days >= daysInMonth)
        {
            _days = Mathf.Clamp(_days - daysInMonth, 1, int.MaxValue);
            OnMonthChange();
        }
    }

    private void OnMonthChange(int months = 1)
    {
        _months += months;
        while(_months >= monthsInYear)
        {
            _months = Mathf.Clamp(_months - monthsInYear, 1, int.MaxValue);
            OnYearChange();
        }
    }

    private void OnYearChange(int year = 1)
    {
        _years += year;
    }
    #endregion

    #region - Moon -
    private MoonPhase GetMoonPhase()
    {
        if (_days <= 1) return MoonPhase.NewMoon;
        else if (_days <= 6) return MoonPhase.WaxingCrescent;
        else if (_days <= 10) return MoonPhase.FirstQuarter;
        else if (_days <= 15) return MoonPhase.WaxingGibbous;
        else if (_days == 16) return MoonPhase.FullMoon;
        else if (_days <= 21) return MoonPhase.WaningGibbous;
        else if (_days <= 25) return MoonPhase.ThirdQuarter;
        else return MoonPhase.WaningCrescent;
    }

    public bool IsNewMoon() => _days == 1;

    public bool IsFullMoon() => _days == 16;

    public bool CanBeEclipse(int day) => day == 16;
    #endregion

    public float GetAvgGlobalTempModifier()
    {
        //this will return a value between 0 and 1, and so then needs to be converted to C or F
        return Mathf.Sin(0.0028f * Mathf.PI * (_days / daysInYear) - 0.5f);
    }

    public void ResetTime()
    {
        _seconds = 0;
        _minutes = 0;
        _hours = 0;
        _days = 1;
        _months = 1;
        _years = 0;
    }

    public void SetSavedTime(int seconds, int minutes, int hours, int days, int months, int years)
    {
        _seconds = seconds;
        _minutes = minutes;
        _hours = hours;
        _days = days;
        _months = months;
        _years = years;
    }
}

public enum MoonPhase
{
    NewMoon,        // day 1        1 day
    WaxingCrescent, // day 2-6      5 days
    FirstQuarter,   // day 7-10     4 days
    WaxingGibbous,  // day 11-15    5 days
    FullMoon,       // day 16       1 day
    WaningGibbous,  // day 17-21    5 days    
    ThirdQuarter,   // day 22-25    4 days
    WaningCrescent  // day 26-30    5 days    
}

public enum MonthsOfYear
{
    FirstCycle, //First day of Spring
    SecondCycle,
    ThirdCycle,
    FourthCycle,
    FifthCycle,
    SixthCycle,
    SeventhCycle,
    EightCycle,
    NinthCycle,
    TenthCycle,
    EleventhCycle,
    TwelfthCycle,
}

public enum Season
{
    Spring, //First day of NewYear
    Summer,
    Autumn,
    Winter
}