using UnityEngine;

public enum DAYLOOPSTATE {  };

public class DayGameLoop : Singleton<DayGameLoop>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var timeManager = TimeManager.Instance;

        (int hours, int minutes) = timeManager.GetTime();

        if(hours == 20 && minutes == 0)
        {
            EndDay();
        }
    }

    void StartNewDay()
    {
        TimeManager.Instance.SetTime(6, 0);
    }

    void EndDay()
    {

    }
}
