using TMPro;
using UnityEngine;

public class TimeUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI clockText;
    [SerializeField] TextMeshProUGUI weekDayText;
    [SerializeField] TextMeshProUGUI dayTimeText;

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
}
