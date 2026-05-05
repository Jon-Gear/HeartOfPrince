using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private List<GameSystem> systems = new List<GameSystem>();

    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        RegisterAllSystems();
    }

    private void RegisterAllSystems()
    {
        foreach(var system in systems) 
        {
            system.Init();
        }
    }


    


    /// <summary>
    /// Get a system of a specific type
    /// </summary>
    public T GetSystem<T>() where T : GameSystem
    {
        foreach (var sys in systems)
        {
            if (sys is T typedSystem)
                return typedSystem;
        }

        Debug.LogError($"System of type {typeof(T).Name} not found!");
        return null;
    }

    /// <summary>
    /// Enable a system (turns on its GameObject)
    /// </summary>
    public void ActivateSystem<T>() where T : GameSystem
    {
        T sys = GetSystem<T>();
        if (sys != null)
            sys.Activate();
    }

    /// <summary>
    /// Disable a system (turns off its GameObject)
    /// </summary>
    public void DeactivateSystem<T>() where T : GameSystem
    {
        T sys = GetSystem<T>();
        if (sys != null)
            sys.Deactivate();
    }






}
