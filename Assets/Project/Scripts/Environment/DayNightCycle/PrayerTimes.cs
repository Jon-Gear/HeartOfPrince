using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;



[CreateAssetMenu(fileName = "Prayer Times", menuName = "Prayer Times", order = 1)] [System.Serializable]
public class PrayerTimes : ScriptableObject
{
    public struct DayTimeData
    {
        public float time;
        public DAYTIME dayTime;
        public float sunAngle;
    }

    private void OnValidate()
    {
        SetDayTimes();
    }


    public DAYTIME GetDayTime(float time)
    {
        for(int i = 0; i < dayTimes1.Count() - 1; i++)
        {
            if(dayTimes1[i].time <= time && time < dayTimes1[i + 1].time)
            {
                return dayTimes1[i].dayTime;
            }
        }

        return DAYTIME.Night;
    }

    public float GetSunAngle(float time)
    {
        for (int i = 0; i < dayTimes1.Count() - 1; i++)
        {
            if (dayTimes1[i].time <= time && time < dayTimes1[i + 1].time)
            {
                float t = Mathf.InverseLerp(dayTimes1[i].time, dayTimes1[i + 1].time, time);
                return Mathf.Lerp(dayTimes1[i].sunAngle, dayTimes1[i + 1].sunAngle, t);
            }
        }

        return 0.0f;
    }

    private List<DayTimeData> dayTimes1 = new List<DayTimeData>();


    private Dictionary<float, DAYTIME> dayTimes = new Dictionary<float, DAYTIME>();


    [Header("Morning")]
    [SerializeField] string morningTime = "04:00";

    [Header("Sunrise")]
    [SerializeField] string sunriseTime = "06:00";

    [Header("Noon")]
    [SerializeField] string noonTime = "13:00";
    
    [Header("Afternoon")]
    [SerializeField] string afternoonTime = "16:00";
    
    [Header("Evening")]
    [SerializeField] string eveningTime = "17:00";
    
    [Header("Night")]
    [SerializeField] string nightTime = "20:00";


    void SetDayTimes()
    {
        dayTimes1.Add(new DayTimeData
        {
            time = ParseTimeTo1fOrWarn("00:00", "Night"),
            dayTime = DAYTIME.Night,
            sunAngle = 0.0f
        });

        dayTimes1.Add(new DayTimeData
        {
            time = ParseTimeTo1fOrWarn(morningTime, "Morning"),
            dayTime = DAYTIME.Morning,
            sunAngle = 72.0f
        });

        dayTimes1.Add(new DayTimeData
        {
            time = ParseTimeTo1fOrWarn(sunriseTime, "Sunrise"),
            dayTime = DAYTIME.Sunrise,
            sunAngle = 90.0f
        });


        dayTimes1.Add(new DayTimeData
        {
            time = ParseTimeTo1fOrWarn(noonTime, "Noon"),
            dayTime = DAYTIME.Noon,
            sunAngle = 195.0f
        });

        dayTimes1.Add(new DayTimeData
        {
            time = ParseTimeTo1fOrWarn(afternoonTime, "Afternoon"),
            dayTime = DAYTIME.Afternoon,
            sunAngle = 233.5f
        });

        dayTimes1.Add(new DayTimeData
        {
            time = ParseTimeTo1fOrWarn(eveningTime, "Evening"),
            dayTime = DAYTIME.Evening,
            sunAngle = 260.0f
        });

        dayTimes1.Add(new DayTimeData
        {
            time = ParseTimeTo1fOrWarn(nightTime, "Night"),
            dayTime = DAYTIME.Night,
            sunAngle = 288
        });


        dayTimes1.Add(new DayTimeData
        {
            time = ParseTimeTo1fOrWarn("24:00", "Night"),
            dayTime = DAYTIME.Night,
            sunAngle = 360
        });
    }
    
    

    private float ParseTimeTo1fOrWarn(string time, string label)
    {
        string[] parts = time.Split(':');
        if (parts.Length == 2 &&
            int.TryParse(parts[0], out int h) && h >= 0 && h <= 24 &&
            int.TryParse(parts[1], out int m) && m >= 0 && m <= 59)
        {
            return ConvertHourAndMinutesTo1f(h, m);
        }

        Debug.LogWarning($"Invalid time format for {label}. Expected HH:MM, got \"{time}\"");
        return 0f; // Default to midnight
    }

    private float ConvertHourAndMinutesTo1f(int hours, int minutes)
    {
        return ((float)hours + (float)minutes / 60.0f) / 24.0f;
    }


}
