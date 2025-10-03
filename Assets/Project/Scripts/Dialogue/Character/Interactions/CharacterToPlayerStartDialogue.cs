using GameCreator.Runtime.Characters;
using System.Collections;
using UnityEngine;

public class CharacterToPlayerStartDialogue : MonoBehaviour
{
    [SerializeField] private Actor characterActor;

    [SerializeField] private TriggerCollider interactionCollider;
    [SerializeField] private TriggerCollider detectionCollider;

    [Header("Settings")]
    [Tooltip("Minimum time between background dialogues (seconds).")]
    [SerializeField] private float minAskInterval = 1.0f;

    [Tooltip("Maximum time between background dialogues (seconds).")]
    [SerializeField] private float maxAskInterval = 2.0f;

    private Character character;
    private CharacterBrain characterBrain;
    
    private bool isFollowingPlayerToAskTopic = false;
    private bool isInInteractionRange = false;

    private Coroutine dialogueCoroutine;

    private Collider otherCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        character = characterActor.gameObject.GetComponent<Character>();
        characterBrain = CharacterManager.Instance.GetCharacter(characterActor.actorName);

        interactionCollider.TriggerEntered += OnInteractInRange;
        interactionCollider.TriggerStayed += OnInteractStayInRange;
        interactionCollider.TriggerExited += OnDetectionOutOfRange;

        detectionCollider.TriggerEntered += OnDetectionInRange;
        detectionCollider.TriggerStayed += OnDetectionStayInRange;
        detectionCollider.TriggerExited += OnDetectionOutOfRange;
    }


    private void OnInteractInRange(Collider other)
    {
        
    }

    private void OnInteractStayInRange(Collider other)
    {
        if (!characterBrain.Dialogue().CanStartCharacterToPlayerDialogue())
        {
            isInInteractionRange = false;
            return;
        }

        isInInteractionRange = true;
    }

    private void OnInteractionOutOfRange(Collider other)
    {

    }


    private void OnDetectionInRange(Collider other)
    {
        dialogueCoroutine = StartCoroutine(Loop());
    }

    private void OnDetectionStayInRange(Collider other)
    {
        otherCollider = other;
    }

    private void OnDetectionOutOfRange(Collider other)
    {
        if(dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
        }

        otherCollider = null;

        StopFollow();
    }

    private IEnumerator Loop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minAskInterval, maxAskInterval));

            if(characterActor.Brain().Dialogue().CanStartPlayerToCharacterDialogue())
            {
                Follow(otherCollider);
            }


            if (characterActor.Brain().Dialogue().CanStartCharacterToPlayerDialogue() && isInInteractionRange)
            {
                Talk();
                StopFollow();
            }
            else
            {
                Debug.Log($"{characterActor.actorName}: Cannot talk right now, skipping monologue.");
            }
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    void Talk()
    {
        characterBrain.Dialogue().TriggerCharacterDialogueWithPlayer();
    }

    void Follow(Collider other)
    {
        Character targetCharacter = other.gameObject.GetComponent<Character>();

        character.Motion.StartFollowingTarget(targetCharacter.transform, 1f, 2f);

        characterActor.Brain().Dialogue().SetIntention(DialogueIntention.ApproachingPlayer);

        isFollowingPlayerToAskTopic = true;
    }

    void StopFollow()
    {
        character.Motion.StopFollowingTarget();
        isFollowingPlayerToAskTopic = false;
        characterActor.Brain().Dialogue().ClearIntention();
    }
}
