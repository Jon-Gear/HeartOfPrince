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
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateClock();
        UpdateWeekDay();
        UpdateDayTime();
        UpdateIcon();
    }

    void UpdateClock()
    {
        var (hours, minutes) = TimeManager.Instance.GetTime();
        clockText.text = $"{hours:D2}:{minutes:D2}";
    }

    void UpdateDayTime()
    {
        dayTimeText.text = TimeManager.Instance.ToString(TimeManager.Instance.GetDayTime());
    }

    void UpdateWeekDay()
    {
        weekDayText.text = TimeManager.Instance.ToString(TimeManager.Instance.GetWeekDay());
    }

    void UpdateIcon()
    {
        var dayTime = TimeManager.Instance.GetDayTime();
        /**/
        switch (dayTime)
        {
            case DAYTIME.Morning:
                DayIcon.sprite = morningIcon;
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
        /**/
    }
}
