using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Lighting Preset", menuName = "Lighting Preset", order = 1)] [System.Serializable]
public class LightingPreset : ScriptableObject
{
    private void OnValidate()
    {
        SetDayTimes();
    }

    public DAYTIME GetDayTime(float dayTime)
    {
        DAYTIME current = DAYTIME.Night;
        float latest = 0f;

        foreach (KeyValuePair<float, DAYTIME> pair in dayTimes)
        {
            if (dayTime >= pair.Key && pair.Key >= latest)
            {
                latest = pair.Key;
                current = pair.Value;
            }
        }

        return current;
    }

    private Dictionary<float, DAYTIME> dayTimes = new Dictionary<float, DAYTIME>();


    [Header("Morning")]
    [SerializeField] string morningTime = "04:00";

    [Header("Sunrise")]
    [SerializeField] string sunriseTime = "08:00";

    [Header("Noon")]
    [SerializeField] string noonTime = "12:00";
    
    [Header("Afternoon")]
    [SerializeField] string afternoonTime = "15:00";
    
    [Header("Evening")]
    [SerializeField] string eveningTime = "18:00";
    
    [Header("Night")]
    [SerializeField] string nightTime = "21:00";


    void SetDayTimes()
    {
        dayTimes[ParseTimeTo1fOrWarn("00:00", "Night")] = DAYTIME.Night; 
        dayTimes[ParseTimeTo1fOrWarn(morningTime, "Morning")] = DAYTIME.Morning;
        dayTimes[ParseTimeTo1fOrWarn(sunriseTime, "Sunrise")] = DAYTIME.Sunrise;
        dayTimes[ParseTimeTo1fOrWarn(noonTime, "Noon")] = DAYTIME.Noon;
        dayTimes[ParseTimeTo1fOrWarn(afternoonTime, "Afternoon")] = DAYTIME.Afternoon;
        dayTimes[ParseTimeTo1fOrWarn(eveningTime, "Evening")] = DAYTIME.Evening;
        dayTimes[ParseTimeTo1fOrWarn(nightTime, "Night")] = DAYTIME.Night;
        dayTimes[ParseTimeTo1fOrWarn("24:00","Night")] = DAYTIME.Night;
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
