using UnityEngine;


public enum TIME_OF_DAY { MORNING, SUNRISE, NOON, AFTERNOON, EVENING, NIGHT};
public enum TIME_OF_WEEK { MONDAY, TUESDAY, WEDNESDAY, THURSDAY, FRIDAY, SATURDAY, SUNDAY};

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
