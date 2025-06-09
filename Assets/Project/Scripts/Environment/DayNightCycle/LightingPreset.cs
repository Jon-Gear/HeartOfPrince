using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Lighting Preset", menuName = "Koshaki/Lighting Preset", order = 1)] [System.Serializable]
public class LightingPreset : ScriptableObject
{
    private Gradient AmbientColor;
    private Gradient IlluminationColor;
    private Gradient SunColor;
    private Gradient MoonColor;


    private Dictionary<float, TIME_OF_DAY> timesOfDay = new Dictionary<float, TIME_OF_DAY>();

    public TIME_OF_DAY GetTimeOfDay(float timeOfDay)
    {
        foreach (KeyValuePair<float, TIME_OF_DAY> pair in timesOfDay)
        {
            if(timeOfDay > pair.Key)
            {
                return pair.Value;
            }
        }
        
        return TIME_OF_DAY.NIGHT;
    }
    
    
    public Color GetAmbientColor(float timeOfDay)
    {
        return AmbientColor.Evaluate(timeOfDay);
    }

    public Color GetSunColor(float intensity)
    {
        return SunColor.Evaluate(intensity);
    }

    public Color GetMoonColor(float intensity)
    {
        return MoonColor.Evaluate(intensity);
    }


    [Header("Morning (Fajr)")]
    
    [SerializeField, Range(0, 24)] private int morningHours;

    [SerializeField, Range(0, 59)] private int morningMinutes; 
    [SerializeField] private Color morningAmbientColor;
    [SerializeField] private Color morningIlluminationColor;



    
    [Header("Sunrise (Shuruq)")]
    [SerializeField, Range(0, 24)] private int sunriseHours;
    [SerializeField, Range(0, 59)] private int sunriseMinutes;
    [SerializeField] private Color sunriseAmbientColor;
    [SerializeField] private Color sunriseIlluminationColor;
    
    
    [Header("Noon (Zuhr)")]
    [SerializeField, Range(0, 24)] private int noonHours;
    [SerializeField, Range(0, 59)] private int noonMinutes; 
    [SerializeField] private Color noonAmbientColor;
    [SerializeField] private Color noonIlluminationColor;
    


    [Header("Afternoon (Asr)")]

    [SerializeField, Range(0, 24)] private int afternoonHours;
    [SerializeField, Range(0, 59)] private int afternoonMinutes; 
    [SerializeField] private Color afternoonAmbientColor;
    [SerializeField] private Color afternoonIlluminationColor;
    


    [Space(10)]
    [Header("Evening (Maghrib)")]

    [SerializeField, Range(0, 24)] private int eveningHours;
    [SerializeField, Range(0, 59)] private int eveningMinutes; 
    [SerializeField] private Color eveningAmbientColor;
    [SerializeField] private Color eveningIlluminationColor;
 

    [Header("Night (Isha)")]

    [SerializeField, Range(0, 24)] private int nightHours;
    [SerializeField, Range(0, 59)] private int nightMinutes; 
    [SerializeField] private Color nightAmbientColor;
    [SerializeField] private Color nightIlluminationColor;


    // To-do
    /*
    For fog:
    - Set a setting for start and end fog (this is important for night scenes to use a smaller number, afternoon scenes must use a larger number)
    - Set a setting for light intensity (this is important for night scenes), should be ON after morning, and OFF after magrib. This will simulate the horizon line.
    - Directional color should be able to be set in the 'times' section
    - Add a setting for setting rotation (x axis)
    - 
    */


    

    

    private void OnValidate()
    {
        SetGradientValues();
        SetTimesOfDay();
    }

    private void SetTimesOfDay()
    {
        timesOfDay[ConvertHourAndMinutesTo1f(nightHours, nightMinutes)] = TIME_OF_DAY.NIGHT;
        timesOfDay[ConvertHourAndMinutesTo1f(eveningHours, eveningMinutes)] = TIME_OF_DAY.EVENING;
        timesOfDay[ConvertHourAndMinutesTo1f(afternoonHours, afternoonMinutes)] = TIME_OF_DAY.AFTERNOON;
        timesOfDay[ConvertHourAndMinutesTo1f(noonHours, noonMinutes)] = TIME_OF_DAY.NOON;
        timesOfDay[ConvertHourAndMinutesTo1f(sunriseHours, sunriseMinutes)] = TIME_OF_DAY.SUNRISE;
        timesOfDay[ConvertHourAndMinutesTo1f(morningHours, morningMinutes)] = TIME_OF_DAY.MORNING;
        timesOfDay[ConvertHourAndMinutesTo1f(0, 0)] = TIME_OF_DAY.NIGHT;
        Debug.Log("Times of day set");
    }

    private void SetGradientValues()
    {
        GradientColorKey[] colorKeys = new GradientColorKey[8];

        #region Ambient Color
        Gradient newAmbientColorGradient = new Gradient();
        
        // night (beginning of day)
        colorKeys[0].color = nightAmbientColor;
        colorKeys[0].time = ConvertHourAndMinutesTo1f(0, 0);

        // morning
        colorKeys[1].color = morningAmbientColor;
        colorKeys[1].time = ConvertHourAndMinutesTo1f(morningHours, morningMinutes);

        // sunrise
        colorKeys[2].color = sunriseAmbientColor;
        colorKeys[2].time = ConvertHourAndMinutesTo1f(sunriseHours, sunriseMinutes);
        
        // noon
        colorKeys[3].color = noonAmbientColor;
        colorKeys[3].time = ConvertHourAndMinutesTo1f(noonHours, noonMinutes);
        
        // afternoon
        colorKeys[4].color = afternoonAmbientColor;
        colorKeys[4].time = ConvertHourAndMinutesTo1f(afternoonHours, afternoonMinutes);

        // evening
        colorKeys[5].color = eveningAmbientColor;
        colorKeys[5].time = ConvertHourAndMinutesTo1f(eveningHours, eveningMinutes);

        // night (end of day)

        colorKeys[6].color = nightAmbientColor;
        colorKeys[6].time = ConvertHourAndMinutesTo1f(nightHours, nightMinutes);
        
        colorKeys[7].color = nightAmbientColor;
        colorKeys[7].time = ConvertHourAndMinutesTo1f(24, 0);
        

        newAmbientColorGradient.SetKeys(colorKeys, new GradientAlphaKey[0]);
        #endregion

        #region Illumination Color
        Gradient newIlluminationColorGradient = new Gradient();
        
        // night (beginning of day)
        colorKeys[0].color = nightIlluminationColor;
        colorKeys[0].time = ConvertHourAndMinutesTo1f(0, 0);

        // morning
        colorKeys[1].color = morningIlluminationColor;
        colorKeys[1].time = ConvertHourAndMinutesTo1f(morningHours, morningMinutes);

        // sunrise
        colorKeys[2].color = sunriseIlluminationColor;
        colorKeys[2].time = ConvertHourAndMinutesTo1f(sunriseHours, sunriseMinutes);
        
        // noon
        colorKeys[3].color = noonIlluminationColor;
        colorKeys[3].time = ConvertHourAndMinutesTo1f(noonHours, noonMinutes);
        
        // afternoon
        colorKeys[4].color = afternoonIlluminationColor;
        colorKeys[4].time = ConvertHourAndMinutesTo1f(afternoonHours, afternoonMinutes);

        // evening
        colorKeys[5].color = eveningIlluminationColor;
        colorKeys[5].time = ConvertHourAndMinutesTo1f(eveningHours, eveningMinutes);

        // night (end of day)

        colorKeys[6].color = nightIlluminationColor;
        colorKeys[6].time = ConvertHourAndMinutesTo1f(nightHours, nightMinutes);
        
        colorKeys[7].color = nightIlluminationColor;
        colorKeys[7].time = ConvertHourAndMinutesTo1f(24, 0);
        

        newIlluminationColorGradient.SetKeys(colorKeys, new GradientAlphaKey[0]);
        #endregion

        Debug.Log("Setting set");
        this.AmbientColor = newAmbientColorGradient;
        this.IlluminationColor = newIlluminationColorGradient;
        
    }
    
    private float ConvertHourAndMinutesTo1f(int hours, int minutes)
    {
        return ((float)hours + (float)minutes / 60.0f) / 24.0f;
    }
}
