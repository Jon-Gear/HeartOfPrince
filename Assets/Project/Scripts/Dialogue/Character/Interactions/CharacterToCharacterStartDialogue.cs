using GameCreator.Runtime.Characters;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterToCharacterStartDialogue : MonoBehaviour
{
    [SerializeField] private Actor characterActor;
    [SerializeField] private TriggerCollider detectionCollider;
    [SerializeField] private List<Actor> nearbyActors;

    [Header("Settings")]
    [Tooltip("Minimum time between background dialogues (seconds).")]
    [SerializeField] private float minDialogueInterval = 1.0f;

    [Tooltip("Maximum time between background dialogues (seconds).")]
    [SerializeField] private float maxDialogueInterval = 2.0f;



    private Character character;
    private CharacterDialogueBrain characterDialogueBrain;

    private Coroutine dialogueCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        character = characterActor.gameObject.GetComponent<Character>();
        characterDialogueBrain = CharacterManager.Instance.GetCharacter(characterActor.actorName);

        detectionCollider.TriggerEntered += OnDetectionInRange;
        detectionCollider.TriggerExited += OnDetectionOutOfRange;

        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void OnDetectionInRange(Collider other)
    {
        Actor otherActor = other.gameObject.GetComponent<Actor>();
        if(otherActor == null || otherActor == characterActor || nearbyActors.Contains(otherActor))
        {
            return;
        }
        
        nearbyActors.Add(otherActor);
        if(nearbyActors.Count == 1)
        {
            StartDialogueLoop();
        }
    }

    private void OnDetectionOutOfRange(Collider other)
    {
        Actor otherActor = other.gameObject.GetComponent<Actor>();
        if (otherActor == null || otherActor == characterActor || !nearbyActors.Contains(otherActor))
        {
            return;
        }
        nearbyActors.Remove(otherActor);
        if(nearbyActors.Count == 0)
        {
            StopDialogueLoop();
        }
    }

    public void StartDialogueLoop()
    {
        if (DialogueManager.Instance.IsDialogueRunning() ||
            DialogueManager.Instance.IsInnerMonologueRunning() ||
            DialogueManager.Instance.IsBackgroundDialogueRunning())
            return;

        dialogueCoroutine = StartCoroutine(DialogueLoop());
        Debug.Log($"Started dialogue loop for {characterActor.actorName} with {nearbyActors.Count} nearby actors.");
    }

    private IEnumerator DialogueLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minDialogueInterval, maxDialogueInterval));
            if(characterDialogueBrain.TriggerCharacterToCharacterDialogue(nearbyActors))
            {
                Debug.Log($"{characterActor.actorName} started a dialogue with another character.");
            }
        }
    }

    public void StopDialogueLoop()
    {
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
            Debug.Log($"Stopped dialogue loop for {characterActor.actorName}.");
        }
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        //StopDialogueLoop();
    }
}
