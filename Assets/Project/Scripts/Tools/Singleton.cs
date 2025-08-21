using UnityEngine;
using UnityEngine.SceneManagement;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static object _lock = new object();
    private static bool _quitting = false;

    public static bool IsQuitting
    {
        get { return _quitting; }
    }
    public static T Instance
    {
        get
        {
            if (_quitting)
            {
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed on application quit.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        Debug.LogError($"[Singleton] Instance '{typeof(T)}' not found in the scene.");
                    }
                }


                return _instance;
            }
        }
    }
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // Kill the duplicate
        }
    }

    protected virtual void OnApplicationQuit()
    {
        _quitting = true;
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }
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


[ExecuteAlways]
public class EditorSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static object _lock = new object();
    private static bool _subscribedToSceneLoad = false;

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        //GameObject singletonObject = new GameObject(typeof(T).Name);
                        //_instance = singletonObject.AddComponent<T>();

                        //if (Application.isPlaying)
                            //DontDestroyOnLoad(singletonObject);
                    }

                    if (Application.isPlaying && !_subscribedToSceneLoad)
                    {
                        SceneManager.sceneLoaded += OnSceneLoaded;
                        _subscribedToSceneLoad = true;
                    }
                }

                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;

            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
                if (!_subscribedToSceneLoad)
                {
                    SceneManager.sceneLoaded += OnSceneLoaded;
                    _subscribedToSceneLoad = true;
                }
            }
        }
        else if (_instance != this)
        {
            if (Application.isPlaying)
                Destroy(gameObject);
            else
                DestroyImmediate(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this && Application.isPlaying)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            _subscribedToSceneLoad = false;
        }
    }

    // Optional scene callback for runtime use
    protected static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Override in subclass if needed
    }

    // Optional helper to check runtime
    public static bool IsRuntime => Application.isPlaying;
}
