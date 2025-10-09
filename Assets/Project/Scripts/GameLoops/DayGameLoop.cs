using System;
using System.Threading.Tasks;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum DAYLOOPSTATE {  };

public class DayGameLoop : Singleton<DayGameLoop>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TimeManager.onWeekDayChanged += OnWeekDayChanged;
    }

    // Update is called once per frame
    void Update()
    {

    }

    async void OnWeekDayChanged(WeekDayPhaseChangedArgs args)
    {
        Debug.Log("Fading");
        await GameManager.Instance.GetSystem<ScreenEffectsManager>().BasicFadeOut(0.5f);

        GameManager.Instance.GetSystem<TimeManager>().StopTime();
        GameManager.Instance.GetSystem<TimeManager>().SetTime("6:00");
        Debug.Log("Loading");
        
        
        await LoadSceneAsyncAwait("Hideout_Prince_Room", LoadSceneMode.Single);
        GameManager.Instance.GetSystem<TimeManager>().StartTime();

        Debug.Log("Unfading");
        await GameManager.Instance.GetSystem<ScreenEffectsManager>().BasicFadeIn(0.5f);
    }

    public async Task LoadSceneAsyncAwait(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, mode);

        while (!op.isDone)
            await Task.Yield();
    }
}

