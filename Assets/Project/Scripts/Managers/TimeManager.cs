using UnityEngine;


public enum DAYTIME { Morning, Sunrise, Noon, Afternoon, Evening, Night};
public enum WEEKDAY { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday};

public class TimeManager : EditorSingleton<TimeManager>
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

    public void ResumeTime()
    {
        isPaused = false;
    }

    public void SetTime(int _hours, int _minutes)
    {
        _hours = Mathf.Clamp(_hours, 0, 23);
        _minutes = Mathf.Clamp(_minutes, 0, 59);
        now =  (_hours + (_minutes / 60f)) / 24f;
    }

    public string ToString(WEEKDAY input)
    {
        switch(input)
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



    public string ToString(DAYTIME input)
    {
        switch(input)
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


    public DAYTIME GetDayTime()
    {
        if (lightingPreset == null)
        {
            Debug.LogWarning("Lighting preset is not set.");
            return DAYTIME.Night;
        }
        return lightingPreset.GetDayTime(now);
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

    public float GetTimePercentage()
    {
        return now;
    }

    public bool IsMonday()
    {
        return dayNumber % 7 == 0;
    }
    
    public bool IsTuesday()
    {
        return dayNumber % 7 == 1;
    }


    public bool IsWednesday()
    {
        return dayNumber % 7 == 2;
    }

    public bool IsThursday()
    {
        return dayNumber % 7 == 3;
    }


    public bool IsFriday()
    {
        return dayNumber % 7 == 4;
    }


    public bool IsSaturday()
    {
        return dayNumber % 7 == 5;
    }

    public bool IsSunday()
    {
        return dayNumber % 7 == 6;
    }



    [SerializeField] private int dayNumber = 0;
    


    [Header("Settings")]
    [SerializeField][Range(0f, 1f)] private float now;
    [SerializeField] private float targetDayLengthInMinutes = 24f;
    [SerializeField] LightingPreset lightingPreset;

    private int hours => Mathf.FloorToInt(now * 24f);
    private int minutes => Mathf.FloorToInt(((now * 24f) - hours) * 60f);
    private bool isPaused = false;
    private float timeScale => 24f / (targetDayLengthInMinutes / 60f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        now = 0.5f;   
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
            now += Time.deltaTime * timeScale / 864000f;
            if(now > 1)
            {
                now -= 1;
            }
        }
    }
}
