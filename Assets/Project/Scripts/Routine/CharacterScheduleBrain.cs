using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using System.Linq;
using UnityEngine;

public class CharacterScheduleBrain : MonoBehaviour
{
    [SerializeField] private PropertyGetInstantiate characterPrefab = new PropertyGetInstantiate();
    [SerializeField] private RoutineData routine;
    [SerializeField] public TimeEntry currentEntry = null;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var timeManager = GameManager.Instance.GetSystem<TimeManager>();
        TimeManager.onClockUpdate += OnClockUpdate;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnClockUpdate(ClockChangedArgs args)
    {
        if (routine == null) return;

        EvaluateSchedule();
    }

    
    private void EvaluateSchedule()
    {
        var newEntry = FindCurrentEntry();
        if (newEntry == currentEntry) return;
        
        currentEntry = newEntry;

        if(newEntry != null)
        {
            Debug.Log($"New entry: {gameObject.name} at {newEntry.PeriodStart()} - {newEntry.PeriodEnd()} in {newEntry.sceneName} at {newEntry.markerID}");
        }
    }

    public TimeEntry FindCurrentEntry()
    {
        if(routine == null) return null;

        var timeManager = GameManager.Instance.GetSystem<TimeManager>();

        WEEKDAY weekday = timeManager.GetWeekDay();
        float now = timeManager.GetTimePercentage();

        DayEntry dayEntry = routine.Schedule.FirstOrDefault(e => e.Weekday == weekday);

        if (dayEntry == null)
        {
            return null;
        }

        TimeEntry timeEntry = dayEntry.Entries.FirstOrDefault(e => now >= TimeUtils.GetTimePercentageFromString(e.PeriodStart()) && now < TimeUtils.GetTimePercentageFromString(e.PeriodEnd()));

        if(timeEntry == null)
        {
            return null;
        }


        return timeEntry;
    }


    private void SpawnCharacterInScene()
    {
        
    }


}
