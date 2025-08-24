using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    [SerializeField] bool onMorning = false;
    [SerializeField] bool onSunrise = false;
    [SerializeField] bool onNoon = false;
    [SerializeField] bool onAfternoon = false;
    [SerializeField] bool onEvening = true;
    [SerializeField] bool onNight = false;


    private Light lightComponent;

    public void ToggleLight()
    {
        lightComponent.enabled = !lightComponent.enabled;
    }

    public void TurnOnLight()
    {
        lightComponent.enabled = true;
    }

    public void TurnOffLight()
    {
        lightComponent.enabled = false;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        lightComponent = GetComponent<Light>();

        TimeManager.onMorning += OnMorning;
        TimeManager.onSunrise += OnSunrise;
        TimeManager.onNoon += OnNoon;
        TimeManager.onAfternoon += OnAfternoon;
        TimeManager.onEvening += OnEvening;
        TimeManager.onNight += OnNight;


        var args = new DayPhaseChangedArgs(TimeManager.Instance.GetTimePercentage(), TimeManager.Instance.GetDayTime());

        switch (TimeManager.Instance.GetDayTime())
        {
            case DAYTIME.Morning: OnMorning(args); break;
            case DAYTIME.Sunrise: OnSunrise(args); break;
            case DAYTIME.Noon: OnNoon(args); break;
            case DAYTIME.Afternoon: OnAfternoon(args); break;
            case DAYTIME.Evening: OnEvening(args); break;
            case DAYTIME.Night: OnNight(args); break;
        }

    }

    private void OnDestroy()
    {
        TimeManager.onMorning -= OnMorning;
        TimeManager.onSunrise -= OnSunrise;
        TimeManager.onNoon -= OnNoon;
        TimeManager.onAfternoon -= OnAfternoon;
        TimeManager.onEvening -= OnEvening;
        TimeManager.onNight -= OnNight;
    }

    private void OnEnable()
    {
        TimeManager.onMorning += OnMorning;
        TimeManager.onSunrise += OnSunrise;
        TimeManager.onNoon += OnNoon;
        TimeManager.onAfternoon += OnAfternoon;
        TimeManager.onEvening += OnEvening;
        TimeManager.onNight += OnNight;
    }

    private void OnDisable()
    {
        TimeManager.onMorning -= OnMorning;
        TimeManager.onSunrise -= OnSunrise;
        TimeManager.onNoon -= OnNoon;
        TimeManager.onAfternoon -= OnAfternoon;
        TimeManager.onEvening -= OnEvening;
        TimeManager.onNight -= OnNight;
    }


    public void OnMorning(DayPhaseChangedArgs args)
    {
        if (onMorning)
        {
            TurnOnLight();
        }
        else
        {
            TurnOffLight();
        }
    }

    public void OnSunrise(DayPhaseChangedArgs args)
    {
        if(onSunrise)
        {
            TurnOnLight();
        }
        else
        {
            TurnOffLight();
        }
    }

    public void OnNoon(DayPhaseChangedArgs args)
    {
        if(onNoon)
        {
            TurnOnLight();
        }
        else
        {
            TurnOffLight();
        }

    }

    public void OnAfternoon(DayPhaseChangedArgs args)
    {
        if(onAfternoon)
        {
            TurnOnLight();
        }
        else
        {
            TurnOffLight();
        }
    }

    public void OnEvening(DayPhaseChangedArgs args)
    {
        if(onEvening)
        {
            TurnOnLight();
        }
        else
        {
            TurnOffLight();
        }
    }

    public void OnNight(DayPhaseChangedArgs args)
    {
        if(onNight)
        {
            TurnOnLight();
        }
        else
        {
            TurnOffLight();
        }
    }
}
