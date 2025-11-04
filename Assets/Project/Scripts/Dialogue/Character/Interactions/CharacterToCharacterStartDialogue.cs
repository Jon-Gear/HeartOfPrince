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
    private CharacterBrain characterBrain;

    private Coroutine dialogueCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        character = characterActor.gameObject.GetComponent<Character>();
        characterBrain = GameManager.Instance.GetSystem<CharacterManager>().GetCharacter(characterActor.actorName);

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
        //if (!GameManager.Instance.GetSystem<DialogueManager>().IsAnyBackgroundDialogueAvailable())
            return;

        dialogueCoroutine = StartCoroutine(DialogueLoop());
    }

    private IEnumerator DialogueLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minDialogueInterval, maxDialogueInterval));

            if(!characterBrain.Dialogue().CanStartCharacterToCharacterDialogue() || nearbyActors.Count == 0)
            {
                continue;
            }

            characterBrain.Dialogue().TriggerCharacterToCharacterDialogue(nearbyActors);
            
        }
    }

    public void StopDialogueLoop()
    {
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
        }
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        //StopDialogueLoop();
    }
}
