using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public abstract class GameSystem : MonoBehaviour
{
    public bool IsActive => gameObject.activeSelf;
    public bool IsInitialized { get; private set; }
    
    protected virtual void Awake()
    {
        DontDestroyOnLoad(gameObject);
        //gameObject.SetActive(false);
    }

    protected virtual void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    protected virtual void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    
    public abstract void Init(); 
    public abstract void Shutdown();

    public void Activate()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        enabled = true;
    }

    public void Deactivate()
    {
        enabled = false;
        gameObject.SetActive(false);
    }

    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }

    protected virtual void OnSceneUnloaded(Scene scene)
    {
    }

    protected virtual void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
    }
}
