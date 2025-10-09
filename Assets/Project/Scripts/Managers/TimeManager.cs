using GameCreator.Runtime.Characters;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;


public enum DAYTIME { Morning, Sunrise, Noon, Afternoon, Evening, Night};
public enum WEEKDAY { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday};

public class ClockChangedArgs
{
    public float Now { get; }
    public int Hours { get; }
    public int Minutes { get; }

    public ClockChangedArgs(float now, int hours, int minutes)
    {
        Now = now;
        Hours = hours;
        Minutes = minutes;
    }
}



public class DayPhaseChangedArgs
{
    public float TimeOfDay { get; }
    public DAYTIME DayTime { get; }
    public DayPhaseChangedArgs(float timeOfDay, DAYTIME dayTime)
    {
        TimeOfDay = timeOfDay;
        DayTime = dayTime;
    }
    /*
    public float SunAngle { get; }
    public float NormalizedTime { get; }
    public int DayNumber { get; }
    public DayPhase Phase { get; }

    
    */
}

public class WeekDayPhaseChangedArgs
{
    public float DayNumber { get; }

    public WEEKDAY WeekDay { get; }
    public WeekDayPhaseChangedArgs(float dayNumber, WEEKDAY weekDay)
    {
        DayNumber = dayNumber;
        WeekDay = weekDay;
    }
    /*
    public float SunAngle { get; }
    public float NormalizedTime { get; }
    public int DayNumber { get; }
    public DayPhase Phase { get; }

    
    */
}


public class TimeManager : GameSystem
{
    // Methods
    public void ToggleTime()
    {
        isPaused = !isPaused;
    }

    public void StopTime()
    {
        isPaused = true;
    }

    public void StartTime()
    {
        isPaused = false;
    }

    public void SetTime(string time)
    {
        // Expecting format "HH:mm"
        if (string.IsNullOrEmpty(time)) return;

        string[] parts = time.Split(':');
        if (parts.Length != 2) return;

        if (int.TryParse(parts[0], out int hours) && int.TryParse(parts[1], out int minutes))
        {
            SetTime(hours, minutes);
        }
    }

    public bool IsTimeWithin(string timeStart, string timeEnd)
    {
        // Expecting format "HH:mm"
        if (string.IsNullOrEmpty(timeStart) || string.IsNullOrEmpty(timeEnd)) return false;

        string[] parts_1 = timeStart.Split(':');
        string[] parts_2 = timeEnd.Split(':');

        if (parts_1.Length != 2 || parts_2.Length != 2) return false;

        if (int.TryParse(parts_1[0], out int hours_1) && int.TryParse(parts_1[1], out int minutes_1) &&
            int.TryParse(parts_2[0], out int hours_2) && int.TryParse(parts_2[1], out int minutes_2))
        {
            hours_1 = Mathf.Clamp(hours_1, 0, 23);
            minutes_1 = Mathf.Clamp(minutes_1, 0, 59);
            float start = (hours_1 + (minutes_1 / 60f)) / 24f;

            hours_2 = Mathf.Clamp(hours_2, 0, 23);
            minutes_2 = Mathf.Clamp(minutes_2, 0, 59);
            float end = (hours_2 + (minutes_2 / 60f)) / 24f;

            if(start < now && now < end)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }


    public void SetTime(int _hours, int _minutes)
    {
        _hours = Mathf.Clamp(_hours, 0, 23);
        _minutes = Mathf.Clamp(_minutes, 0, 59);
        now =  (_hours + (_minutes / 60f)) / 24f;
    }

    


    public DAYTIME GetDayTime()
    {
        if (prayerTimes == null)
        {
            Debug.LogWarning("Prayer times is not set.");
            return DAYTIME.Night;
        }
        return prayerTimes.GetDayTime(now);
    }


    public WEEKDAY GetWeekDay()
    {
        switch (dayNumber % 7)
        {
            case 0: return WEEKDAY.Monday;
            case 1: return WEEKDAY.Tuesday;
            case 2: return WEEKDAY.Wednesday;
            case 3: return WEEKDAY.Thursday;
            case 4: return WEEKDAY.Friday;
            case 5: return WEEKDAY.Saturday;
            case 6: return WEEKDAY.Sunday;
            default: return WEEKDAY.Monday; // Fallback
        }
    }

    public (int, int) GetTime()
    {
        return (hours, minutes);
    }

    public string GetTimeString()
    {
        return $"{hours:D2}:{minutes:D2}";
    }

    public (int, int) GetTimeFromHoursAndMinutesFromNow(int hours, int minutes)
    {
        // Convert the current time to total minutes
        float totalMinutes = now * 1440f; // 1440 minutes in a day
        // Add the specified hours and minutes
        totalMinutes += (hours * 60) + minutes;
        // Normalize to a day
        totalMinutes %= 1440f;
        // Calculate new hours and minutes
        int newHours = Mathf.FloorToInt(totalMinutes / 60);
        int newMinutes = Mathf.FloorToInt(totalMinutes % 60);
        return (newHours, newMinutes);
    }

    public string GetTimeFromHoursAndMinutesFromNowString(string time)
    {
        // Expecting format "HH:mm"
        if (string.IsNullOrEmpty(time)) return "00:00";

        string[] parts = time.Split(':');
        if (parts.Length != 2) return "00:00";

        if (int.TryParse(parts[0], out int hours) && int.TryParse(parts[1], out int minutes))
        {
            var t = GetTimeFromHoursAndMinutesFromNow(hours, minutes);
            return $"{t.Item1:D2}:{t.Item2:D2}";
        }
        else
        {
            return "00:00"; // Default to midnight
        }

    }




    public float GetTimePercentage()
    {
        return now;
    }

    public float GetSunAngle()
    {
        if (prayerTimes == null)
        {
            Debug.Log("Prayer times is not set.");
            return 0.0f;
        }
        return prayerTimes.GetSunAngle(now);
    }

    public float GetSunIntensity()
    {
        if (prayerTimes == null)
        {
            Debug.Log("Prayer times is not set.");
            return 0.0f;
        }
        return Mathf.Clamp01(Mathf.Sin((GetSunAngle() - 90) * Mathf.Deg2Rad));
    }



    [SerializeField] private int dayNumber = 0;
    


    [Header("Settings")]
    [SerializeField][Range(0f, 1f)] private float now;
    [SerializeField] private float targetDayLengthInMinutes = 24f;
    [SerializeField] public PrayerTimes prayerTimes;

    private int hours => Mathf.FloorToInt(now * 24f);
    private int minutes => Mathf.FloorToInt(((now * 24f) - hours) * 60f);
    private bool isPaused = false;
    private float timeScale => 24f / (targetDayLengthInMinutes / 60f);

    public static event UnityAction<ClockChangedArgs> onClockUpdate;

    public static event UnityAction<DayPhaseChangedArgs> onMorning;
    public static event UnityAction<DayPhaseChangedArgs> onSunrise;
    public static event UnityAction<DayPhaseChangedArgs> onNoon;
    public static event UnityAction<DayPhaseChangedArgs> onAfternoon;
    public static event UnityAction<DayPhaseChangedArgs> onEvening;
    public static event UnityAction<DayPhaseChangedArgs> onNight;
    public static event UnityAction<DayPhaseChangedArgs> onDayTimeChanged;


    private DAYTIME currentDayTime;

    public static event UnityAction<WeekDayPhaseChangedArgs> onMonday;
    public static event UnityAction<WeekDayPhaseChangedArgs> onTuesday;
    public static event UnityAction<WeekDayPhaseChangedArgs> onWednesday;
    public static event UnityAction<WeekDayPhaseChangedArgs> onThursday;
    public static event UnityAction<WeekDayPhaseChangedArgs> onFriday;
    public static event UnityAction<WeekDayPhaseChangedArgs> onSaturday;
    public static event UnityAction<WeekDayPhaseChangedArgs> onSunday;
    public static event UnityAction<WeekDayPhaseChangedArgs> onWeekDayChanged;

    private WEEKDAY currentWeekDay;


    private void TriggerClockUpdate()
    {
        var args = new ClockChangedArgs(now, hours, minutes);
        onClockUpdate?.Invoke(args);
    }

    private void TriggerMorning()
    {
        var args = new DayPhaseChangedArgs(now, currentDayTime);
        onMorning?.Invoke(args);
    }

    private void TriggerSunrise()
    {
        var args = new DayPhaseChangedArgs(now, currentDayTime);
        onSunrise?.Invoke(args);
    }
    private void TriggerNoon()
    {
        var args = new DayPhaseChangedArgs(now, currentDayTime);
        onNoon?.Invoke(args);
    }
    private void TriggerAfternoon()
    {
        var args = new DayPhaseChangedArgs(now, currentDayTime);
        onAfternoon?.Invoke(args);
    }
    private void TriggerEvening()
    {
        var args = new DayPhaseChangedArgs(now, currentDayTime);
        onEvening?.Invoke(args);
    }
    private void TriggerNight()
    {
        var args = new DayPhaseChangedArgs(now, currentDayTime);
        onNight?.Invoke(args);
    }


    private void TriggerDayTimeChanged()
    {        
        currentDayTime = GetDayTime();

        switch (currentDayTime)
        {
            case DAYTIME.Morning: TriggerMorning(); break;
            case DAYTIME.Sunrise: TriggerSunrise(); break;
            case DAYTIME.Noon: TriggerNoon(); break;
            case DAYTIME.Afternoon: TriggerAfternoon(); break;
            case DAYTIME.Evening: TriggerEvening(); break;
            case DAYTIME.Night: TriggerNight(); break;
        }

        var args = new DayPhaseChangedArgs(now, currentDayTime);
        onDayTimeChanged?.Invoke(args);
    }

    private void TriggerMonday()
    {
        var args = new WeekDayPhaseChangedArgs(dayNumber, currentWeekDay);
        onMonday?.Invoke(args);
    }

    private void TriggerTuesday()
    {
        var args = new WeekDayPhaseChangedArgs(dayNumber, currentWeekDay);
        onTuesday?.Invoke(args);
    }

    private void TriggerWednesday()
    {
        var args = new WeekDayPhaseChangedArgs(dayNumber, currentWeekDay);
        onWednesday?.Invoke(args);
    }

    private void TriggerThursday()
    {
        var args = new WeekDayPhaseChangedArgs(dayNumber, currentWeekDay);
        onThursday?.Invoke(args);
    }

    private void TriggerFriday()
    {
        var args = new WeekDayPhaseChangedArgs(dayNumber, currentWeekDay);
        onFriday?.Invoke(args);
    }

    private void TriggerSaturday()
    {
        var args = new WeekDayPhaseChangedArgs(dayNumber, currentWeekDay);
        onSaturday?.Invoke(args);
    }

    private void TriggerSunday()
    {
        var args = new WeekDayPhaseChangedArgs(dayNumber, currentWeekDay);
        onSunday?.Invoke(args);
    }


    private void TriggerWeekDayChanged()
    {
        dayNumber += 1;
        currentWeekDay = GetWeekDay();


        switch (currentWeekDay)
        {
            case WEEKDAY.Monday: TriggerMonday(); break;
            case WEEKDAY.Tuesday: TriggerTuesday(); break;
            case WEEKDAY.Wednesday: TriggerWednesday(); break;
            case WEEKDAY.Thursday: TriggerThursday(); break;
            case WEEKDAY.Friday: TriggerFriday(); break;
            case WEEKDAY.Saturday: TriggerSaturday(); break;
            case WEEKDAY.Sunday: TriggerSunday(); break;
        }

        var args = new WeekDayPhaseChangedArgs(dayNumber, currentWeekDay);
        onWeekDayChanged?.Invoke(args);
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        
    }

    public override void Init()
    {
        prayerTimes.SetDayTimes();
    }

    public override void Shutdown()
    {
    }


    // Update is called once per frame
    private void Update()
    {
        if(isPaused)
        {
            return;
        } 

        if (Application.isPlaying)
        {
            now += Time.deltaTime * timeScale / 86400f;
            TriggerClockUpdate();

            if (now > 1)
            {
                now -= 1;
                TriggerWeekDayChanged();
            }

            if(currentDayTime != GetDayTime())
            {
                TriggerDayTimeChanged();
            }

        }
    }

    
}
public static class TimeUtils
{
    public static string WeekDayToString(WEEKDAY input)
    {
        switch (input)
        {
            case WEEKDAY.Monday: return "Monday";
            case WEEKDAY.Tuesday: return "Tuesday";
            case WEEKDAY.Wednesday: return "Wednesday";
            case WEEKDAY.Thursday: return "Thursday";
            case WEEKDAY.Friday: return "Friday";
            case WEEKDAY.Saturday: return "Saturday";
            case WEEKDAY.Sunday: return "Sunday";
            default: return "Unknown";
        }
    }

    public static string DayTimeToString(DAYTIME input)
    {
        switch (input)
        {
            case DAYTIME.Morning: return "Morning";
            case DAYTIME.Sunrise: return "Sunrise";
            case DAYTIME.Noon: return "Noon";
            case DAYTIME.Afternoon: return "Afternoon";
            case DAYTIME.Evening: return "Evening";
            case DAYTIME.Night: return "Night";
            default: return "Unknown";
        }
    }
}


public class EditorTimeManager : EditorSingleton<TimeManager>
{

}
