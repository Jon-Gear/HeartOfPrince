using GameCreator.Runtime.Characters;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterStartMonologue : MonoBehaviour
{
    [SerializeField] private Actor characterActor;
    [SerializeField] private TriggerCollider detectionCollider;

    private ActivityCharacterMonologue activityCharacterMonologue = new ActivityCharacterMonologue();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        detectionCollider.TriggerEntered += OnPlayerInRange;
        detectionCollider.TriggerExited += OnPlayerOutOfRange;
    }


    void OnPlayerInRange(Collider other)
    {
        characterActor.Brain().Activity().AddActivity(activityCharacterMonologue);
    }

    void OnPlayerOutOfRange(Collider other)
    {
        characterActor.Brain().Activity().RemoveActivity(activityCharacterMonologue);
    }
}
