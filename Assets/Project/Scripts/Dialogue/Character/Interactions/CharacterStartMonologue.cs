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

    private Character character;
    private CharacterDialogueBrain characterDialogueBrain;

    private Coroutine monologueCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        character = characterActor.gameObject.GetComponent<Character>();
        characterDialogueBrain = CharacterManager.Instance.GetCharacter(characterActor.actorName);

        detectionCollider.TriggerEntered += OnPlayerInRange;
        detectionCollider.TriggerExited += OnPlayerOutOfRange;

        SceneManager.activeSceneChanged += OnActiveSceneChanged;
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
        if (DialogueManager.Instance.IsDialogueRunning() ||
            DialogueManager.Instance.IsInnerMonologueRunning() ||
            DialogueManager.Instance.IsBackgroundDialogueRunning())
            return;

        monologueCoroutine = StartCoroutine(MonologueLoop());
    }

    private IEnumerator MonologueLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minMonologueInterval, maxMonologueInterval));
            characterDialogueBrain.TriggerMonologue();
        }
    }

    public void StopMonologueLoop()
    {
        if (monologueCoroutine != null)
        {
            StopCoroutine(monologueCoroutine);
        }
    }


    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        StopMonologueLoop();
    }


}
