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
    private const int daysInWeek = 10;
    private const int weeksInMonth = 3;
    private const int monthsInYear = 12;
    private const int daysInYear = 360;
    #endregion

    #region - Time Values -
    [SerializeField] private int _seconds;
    [SerializeField] private int _minutes;
    [SerializeField] private int _hours;
    [SerializeField] private int _days;
    [SerializeField] private int _weeks;
    [SerializeField] private int _months;
    [SerializeField] private int _years;

    public int Seconds => _seconds;
    public int Minutes => _minutes;
    public int Hours => _hours;
    public int Days => _days;
    public int Weeks => _weeks;
    public int Months => _months;
    public int Years => _years;
    #endregion

    private bool _local;

    public void OnWorldMap()
    {
        _local = false;
    }

    public void OnLocalMap()
    {
        _local = true;
    }

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
        while( _days >= daysInWeek)
        {
            _days -= daysInWeek;
            OnWeekChange();
        }
    }

    private void OnWeekChange(int weeks = 1)
    {
        _weeks += weeks;
        while(_weeks >= weeksInMonth)
        {
            _weeks -= weeksInMonth;
            OnMonthChange();
        }
    }

    private void OnMonthChange(int months = 1)
    {
        _months += months;
        while(_months >= monthsInYear)
        {
            _months -= monthsInYear;
            OnYearChange();
        }
    }

    private void OnYearChange(int year = 1)
    {
        _years += year;
    }

    public void ResetTime()
    {
        _seconds = 0;
        _minutes = 0;
        _hours = 0;
        _days = 0;
        _weeks = 0;
        _months = 0;
        _years = 0;
    }

    public void SetSavedTime(int seconds, int minutes, int hours, int days, int weeks, int months, int years)
    {
        _seconds = seconds;
        _minutes = minutes;
        _hours = hours;
        _days = days;
        _weeks = weeks;
        _months = months;
        _years = years;
    }
}
