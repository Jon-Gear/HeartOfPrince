using UnityEngine;
using UnityEngine.Events;

public enum GameState
{
    MainMenu,
    Game
}

public class GameManager : Singleton<GameManager>
{
    public static event UnityAction onMainMenu;


    GameState state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = GameState.MainMenu;
        onMainMenu += OnMainMenu;
        onMainMenu.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMainMenu()
    {
        TimeManager.Instance.enabled = false;
        CinemachineManager.Instance.enabled = false;
    }

}
