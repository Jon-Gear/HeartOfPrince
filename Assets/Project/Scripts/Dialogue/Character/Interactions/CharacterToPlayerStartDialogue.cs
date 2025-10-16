using System.Collections;
using UnityEngine;
using GameCreator.Runtime.Characters;


public class CharacterToPlayerStartDialogue : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Actor characterActor;
    [SerializeField] private TriggerCollider detectionCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        detectionCollider.TriggerEntered += OnPlayerInRange;
        detectionCollider.TriggerExited += OnPlayerOutOfRange;
    }


    void OnPlayerInRange(Collider other)
    {
        characterActor.Brain().Activity().AddActivity<ActivityCharacterToPlayerDialogue>();
    }

    void OnPlayerOutOfRange(Collider other)
    {
        characterActor.Brain().Activity().RemoveActivity<ActivityCharacterToPlayerDialogue>();
    }
}
