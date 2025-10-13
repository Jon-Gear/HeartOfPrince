using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterScheduleBrain : MonoBehaviour
{
    [SerializeField] private RoutineData routine;
    [SerializeField] public TimeEntry currentEntry = null;

    private CharacterBrain characterBrain;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterBrain = GetComponent<CharacterBrain>();

        SceneManager.activeSceneChanged += OnActiveSceneChanged;

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

        // If the schedule hasn't changed, do nothing
        if (newEntry == currentEntry) return;

        var previousEntry = currentEntry;
        currentEntry = newEntry;

        string currentSceneName = SceneManager.GetActiveScene().name;

        // --- LEAVE SCENE ---
        if (previousEntry != null
            && previousEntry.sceneName == currentSceneName
            && (currentEntry == null || currentEntry.sceneName != currentSceneName))
        {
            LeaveScene();
        }
        // --- ENTER SCENE ---
        else if ((previousEntry == null || previousEntry.sceneName != currentSceneName)
                 && currentEntry != null
                 && currentEntry.sceneName == currentSceneName)
        {
            EnterScene();
        }
        // --- MOVE IN SCENE ---
        else if (previousEntry != null
                 && currentEntry != null
                 && previousEntry.sceneName == currentEntry.sceneName
                 && currentEntry.sceneName == currentSceneName)
        {
            MoveInScene();
        }
    }

    private void LeaveScene()
    {
        Level level = FindFirstObjectByType<Level>();
        characterBrain.MoveCharacterToMarkerThenDespawn(level.GetRandomExit());
    }

    private void MoveInScene()
    {
        Level level = FindFirstObjectByType<Level>();
        characterBrain.MoveCharacterToMarkerID(currentEntry.markerID);
    }

    private void SpawnInScene()
    {
        characterBrain.SpawnCharacterAtMarkerID(currentEntry.markerID);
    }


    private async Task EnterScene()
    {
        Level level = FindFirstObjectByType<Level>();
        characterBrain.SpawnCharacterAtMarker(level.GetRandomEntrance());
        await Task.Yield();
        characterBrain.MoveCharacterToMarkerID(currentEntry.markerID);
    }


    protected virtual void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        TimeEntry newEntry = FindCurrentEntry();

        if(newEntry == null)
        {
            return;
        }

        currentEntry = newEntry;

        if(newEntry.sceneName == newScene.name)
        {
            SpawnInScene();
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

}
