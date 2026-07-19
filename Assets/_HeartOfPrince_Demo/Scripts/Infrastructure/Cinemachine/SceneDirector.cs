using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using Yarn.Unity;

public class SceneDirector : MonoBehaviour
{
    public static SceneDirector Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public enum Shot
    {
        Wide,
        TwoShot,
        OTS_A,
        OTS_B,
        CloseUp_A,
        CloseUp_B
    }

    [Header("Core Camera Shots")]
    [SerializeField] CinemachineCamera WideShot;
    [SerializeField] CinemachineCamera TwoShot;
    [SerializeField] CinemachineCamera OverTheShoulderShotA;
    [SerializeField] CinemachineCamera OverTheShoulderShotB;
    [SerializeField] CinemachineCamera CloseUpShotA;
    [SerializeField] CinemachineCamera CloseUpShotB;

    [Header("Dialogue Runner")]
    [SerializeField] DialogueRunner SetPieceDialogueRunner;

    
    private List<SceneActor> sceneActors = new List<SceneActor>();



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RegisterActor(SceneActor actor)
    {
        if (!sceneActors.Contains(actor))
        {
            sceneActors.Add(actor);
        }
    }

    public void UnregisterActor(SceneActor actor)
    {
        if (sceneActors.Contains(actor))
        {
            sceneActors.Remove(actor);
        }
    }

    [YarnCommand("SetShot")]
    public static void YarnSetShot(string shotName)
    {
        if (!Enum.TryParse(shotName, true, out Shot shot))
        {
            Debug.LogError($"Unknown shot '{shotName}'");
            return;
        }

        if (SceneDirector.Instance == null)
        {
            Debug.LogWarning($"Cannot set shot '{shotName}': no SceneDirector is active.");
            return;
        }

        SceneDirector.Instance.SetShot(shot);
    }

    public void SetShot(Shot shot)
    {
        if (WideShot == null || TwoShot == null ||
            OverTheShoulderShotA == null || OverTheShoulderShotB == null ||
            CloseUpShotA == null || CloseUpShotB == null)
        {
            Debug.LogWarning($"SceneDirector '{name}' is missing one or more camera bindings.");
            return;
        }

        WideShot.Priority = (shot == Shot.Wide) ? 1 : 0;
        TwoShot.Priority = (shot == Shot.TwoShot) ? 1 : 0;
        OverTheShoulderShotA.Priority = (shot == Shot.OTS_A) ? 1 : 0;
        OverTheShoulderShotB.Priority = (shot == Shot.OTS_B) ? 1 : 0;
        CloseUpShotA.Priority = (shot == Shot.CloseUp_A) ? 1 : 0;
        CloseUpShotB.Priority = (shot == Shot.CloseUp_B) ? 1 : 0;
    }



    private void OnDestroy()
    {
        sceneActors.Clear();
        // Important: clear the reference when the scene unloads
        if (Instance == this)
            Instance = null;
    }
}
