
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
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

#if UNITY_EDITOR
    private static bool _subscribedToPlayMode = false;
#endif

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
                        Debug.LogError($"[Singleton] Instance '{typeof(T)}' not found in the scene.");
                        return null;
                    }

                    if (Application.isPlaying && !_subscribedToSceneLoad)
                    {
                        SceneManager.sceneLoaded += OnSceneLoaded;
                        _subscribedToSceneLoad = true;
                    }

#if UNITY_EDITOR
                    if (!_subscribedToPlayMode)
                    {
                        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
                        _subscribedToPlayMode = true;
                    }
#endif
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
                MakePersistent();
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

    private static void MakePersistent()
    {
        if (_instance != null)
        {
            DontDestroyOnLoad((_instance as MonoBehaviour).gameObject);
            Debug.Log($"{typeof(T).Name} moved to DontDestroyOnLoad");
        }
    }

#if UNITY_EDITOR
    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            MakePersistent();
        }
    }
#endif

    protected virtual void OnDestroy()
    {
        if (_instance == this && Application.isPlaying)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            _subscribedToSceneLoad = false;
        }

#if UNITY_EDITOR
        if (_instance == this && _subscribedToPlayMode)
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            _subscribedToPlayMode = false;
        }
#endif
    }

    // Optional scene callback for runtime use
    protected static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Override in subclass if needed
    }

    // Optional helper to check runtime
    public static bool IsRuntime => Application.isPlaying;
}
