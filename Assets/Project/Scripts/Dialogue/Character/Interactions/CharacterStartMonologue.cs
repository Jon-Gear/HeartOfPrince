using GameCreator.Runtime.Characters;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterStartMonologue : MonoBehaviour
{
    [SerializeField] private Actor characterActor;

    [SerializeField] private TriggerCollider detectionCollider;

    [Header("Settings")]
    [Tooltip("Minimum time between background dialogues (seconds).")]
    [SerializeField] private float minMonologueInterval = 1.0f;

    [Tooltip("Maximum time between background dialogues (seconds).")]
    [SerializeField] private float maxMonologueInterval = 2.0f;

    private Coroutine monologueCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        detectionCollider.TriggerEntered += OnPlayerInRange;
        detectionCollider.TriggerExited += OnPlayerOutOfRange;
    }


    void OnPlayerInRange(Collider other)
    {
        StartMonologueLoop();
    }

    void OnPlayerOutOfRange(Collider other)
    {
        StopMonologueLoop();
    }

    public void StartMonologueLoop()
    {
        monologueCoroutine = StartCoroutine(MonologueLoop());
    }

    private IEnumerator MonologueLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minMonologueInterval, maxMonologueInterval));
            if(characterActor.Brain().Dialogue().CanStartCharacterMonologue())
            {
                characterActor.Brain().Dialogue().TriggerMonologue();
                Debug.Log($"{characterActor.actorName}: Triggering monologue.");
            }
            else
            {
                Debug.Log($"{characterActor.actorName}: Cannot talk right now, skipping monologue.");
            }
        }
    }

    public void StopMonologueLoop()
    {
        if (monologueCoroutine != null)
        {
            StopCoroutine(monologueCoroutine);
        }
    }

}
