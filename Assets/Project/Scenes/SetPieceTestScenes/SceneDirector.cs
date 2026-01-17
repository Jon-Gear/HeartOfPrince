using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using Yarn.Unity;

public class SceneDirector : MonoBehaviour
{
    public enum Shot
    {
        Wide,
        Master,
        OTS_A,
        OTS_B,
    }

    [Header("Core Camera Shots")]
    [SerializeField] CinemachineCamera WideShot;
    [SerializeField] CinemachineCamera MasterShot;
    [SerializeField] CinemachineCamera OverTheShoulderShotA;
    [SerializeField] CinemachineCamera OverTheShoulderShotB;

    [Header("Dialogue Runner")]
    [SerializeField] DialogueRunner SetPieceDialogueRunner;

    [Header("Actors")]
    [SerializeField] private List<SceneActor> sceneActors = new List<SceneActor>();



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
