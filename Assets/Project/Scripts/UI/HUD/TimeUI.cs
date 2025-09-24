using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI clockText;
    [SerializeField] TextMeshProUGUI weekDayText;
    [SerializeField] TextMeshProUGUI dayTimeText;
    [SerializeField] Image DayIcon;

    [Header("Daytime Icons")]

    [SerializeField] Sprite morningIcon;
    [SerializeField] Sprite sunriseIcon;
    [SerializeField] Sprite noonIcon;
    [SerializeField] Sprite afternoonIcon;
    [SerializeField] Sprite eveningIcon;
    [SerializeField] Sprite nightIcon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
        // DayTime
        TimeManager.onMorning += OnMorning;
        TimeManager.onSunrise += OnSunrise;
        TimeManager.onNoon += OnNoon;
        TimeManager.onAfternoon += OnAfternoon;
        TimeManager.onEvening += OnEvening;
        TimeManager.onNight += OnNight;

        // Weekday
        TimeManager.onClockUpdate += OnClockUpdate;
        TimeManager.onDayTimeChanged += OnDayTimeChanged;
        TimeManager.onWeekDayChanged += OnWeekDayChanged;
    }

    private void Awake()
    {
        // DayTime
        TimeManager.onMorning += OnMorning;
        TimeManager.onSunrise += OnSunrise;
        TimeManager.onNoon += OnNoon;   
        TimeManager.onAfternoon += OnAfternoon;
        TimeManager.onEvening += OnEvening;
        TimeManager.onNight += OnNight;

        // Weekday
        TimeManager.onClockUpdate += OnClockUpdate;
        TimeManager.onDayTimeChanged += OnDayTimeChanged;
        TimeManager.onWeekDayChanged += OnWeekDayChanged;

    }

    private void OnDestroy()
    {
        // DayTime
        TimeManager.onMorning -= OnMorning;
        TimeManager.onSunrise -= OnSunrise;
        TimeManager.onNoon -= OnNoon;   
        TimeManager.onAfternoon -= OnAfternoon;
        TimeManager.onEvening -= OnEvening;
        TimeManager.onNight -= OnNight;
        // Weekday
        TimeManager.onClockUpdate -= OnClockUpdate;
        TimeManager.onDayTimeChanged -= OnDayTimeChanged;
        TimeManager.onWeekDayChanged -= OnWeekDayChanged;
    }

    private void OnEnable()
    {
        // DayTime
        TimeManager.onMorning += OnMorning;
        TimeManager.onSunrise += OnSunrise;
        TimeManager.onNoon += OnNoon;
        TimeManager.onAfternoon += OnAfternoon;
        TimeManager.onEvening += OnEvening;
        TimeManager.onNight += OnNight;

        // Weekday
        TimeManager.onClockUpdate += OnClockUpdate;
        TimeManager.onDayTimeChanged += OnDayTimeChanged;
        TimeManager.onWeekDayChanged += OnWeekDayChanged;
    }

    private void OnDisable()
    {
        // DayTime
        TimeManager.onMorning -= OnMorning;
        TimeManager.onSunrise -= OnSunrise;
        TimeManager.onNoon -= OnNoon;   
        TimeManager.onAfternoon -= OnAfternoon;
        TimeManager.onEvening -= OnEvening;
        TimeManager.onNight -= OnNight;
        // Weekday
        TimeManager.onClockUpdate -= OnClockUpdate;
        TimeManager.onDayTimeChanged -= OnDayTimeChanged;
        TimeManager.onWeekDayChanged -= OnWeekDayChanged;
    }

    void OnClockUpdate(ClockChangedArgs args)
    {
        clockText.text = $"{args.Hours:D2}:{args.Minutes:D2}";
    }

    void OnWeekDayChanged(WeekDayPhaseChangedArgs args)
    {
        weekDayText.text = TimeUtils.WeekDayToString(args.WeekDay);
    }

    void OnDayTimeChanged(DayPhaseChangedArgs args)
    {
        dayTimeText.text = TimeUtils.DayTimeToString(args.DayTime);
        //Debug.Log($"DayTime changed to {args.DayTime}, updating text to {dayTimeText.text}");
    }

    void OnMorning(DayPhaseChangedArgs args)
    {
        DayIcon.sprite = morningIcon;
    }

    void OnSunrise(DayPhaseChangedArgs args)
    {
        DayIcon.sprite = sunriseIcon;
    }

    void OnNoon(DayPhaseChangedArgs args)
    {
        DayIcon.sprite = noonIcon;
    }

    void OnAfternoon(DayPhaseChangedArgs args)
    {
        DayIcon.sprite = afternoonIcon;
    }

    void OnEvening(DayPhaseChangedArgs args)
    {
        DayIcon.sprite = eveningIcon;
    }

    void OnNight(DayPhaseChangedArgs args)
    {
        DayIcon.sprite = nightIcon;
    }

    /*
    void UpdateIcon()
    {
        var dayTime = TimeManager.Instance.GetDayTime();
        switch (dayTime)
        {
            case DAYTIME.Morning:
                break;
            case DAYTIME.Sunrise:
                DayIcon.sprite = sunriseIcon;
                break;
            case DAYTIME.Noon:
                DayIcon.sprite = noonIcon;
                break;
            case DAYTIME.Afternoon:
                DayIcon.sprite = afternoonIcon;
                break;
            case DAYTIME.Evening:
                DayIcon.sprite = eveningIcon;
                break;
            case DAYTIME.Night:
                DayIcon.sprite = nightIcon;
                break;
        }
    }
    */
}
