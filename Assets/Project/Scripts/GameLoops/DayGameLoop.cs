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
        await ScreenEffectsManager.Instance.BasicFadeOut(0.5f);

        TimeManager.Instance.StopTime();
        TimeManager.Instance.SetTime("6:00");
        Debug.Log("Loading");
        
        
        await LoadSceneAsyncAwait("Hideout_Prince_Room", LoadSceneMode.Single);
        TimeManager.Instance.StartTime();

        Debug.Log("Unfading");
        await ScreenEffectsManager.Instance.BasicFadeIn(0.5f);
    }

    public async Task LoadSceneAsyncAwait(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, mode);

        while (!op.isDone)
            await Task.Yield();
    }
}

