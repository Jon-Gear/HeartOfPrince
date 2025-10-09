using System.Collections;
using UnityEngine;
using GameCreator.Runtime.Characters;

/// <summary>
/// This component allows an NPC to periodically check for a nearby player,
/// approach them, and initiate dialogue if conditions are met.
/// </summary>
public class CharacterToPlayerStartDialogue : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Actor characterActor;
    [SerializeField] private TriggerCollider interactionCollider;
    [SerializeField] private TriggerCollider detectionCollider;

    [Header("Dialogue Settings")]
    [Tooltip("Minimum delay between dialogue attempts (seconds).")]
    [SerializeField] private float minAskInterval = 1.0f;

    [Tooltip("Maximum delay between dialogue attempts (seconds).")]
    [SerializeField] private float maxAskInterval = 2.0f;

    private Character character;
    private CharacterBrain characterBrain;
    private Collider detectedPlayer;

    private bool isFollowingPlayer;
    private bool isPlayerInInteractionRange;

    private Coroutine dialogueLoopCoroutine;

    // ----------------------------------------------------------------------

    private void Start()
    {
        if (characterActor == null)
        {
            Debug.LogError($"[{nameof(CharacterToPlayerStartDialogue)}] Missing characterActor reference.");
            enabled = false;
            return;
        }

        character = characterActor.GetComponent<Character>();
        characterBrain = GameManager.Instance.GetSystem<CharacterManager>().GetCharacter(characterActor.actorName);

        if (character == null || characterBrain == null)
        {
            Debug.LogError($"[{nameof(CharacterToPlayerStartDialogue)}] Could not initialize character or brain.");
            enabled = false;
            return;
        }

        // Subscribe to trigger events
        interactionCollider.TriggerEntered += OnInteractionEnter;
        interactionCollider.TriggerStayed += OnInteractionStay;
        interactionCollider.TriggerExited += OnInteractionExit;

        detectionCollider.TriggerEntered += OnDetectionEnter;
        detectionCollider.TriggerStayed += OnDetectionStay;
        detectionCollider.TriggerExited += OnDetectionExit;
    }

    // ----------------------------------------------------------------------
    #region Interaction Colliders

    private void OnInteractionEnter(Collider other)
    {
        // Optionally handle visual cues or prompts here
    }

    private void OnInteractionStay(Collider other)
    {
        if (characterBrain.Dialogue().CanStartCharacterToPlayerDialogue())
        {
            StartDialogue();
            StopFollowing();
        }
        
    }

    private void OnInteractionExit(Collider other)
    {
        isPlayerInInteractionRange = false;
    }

    #endregion

    // ----------------------------------------------------------------------
    #region Detection Colliders

    private void OnDetectionEnter(Collider other)
    {
        detectedPlayer = other;
        StartDialogueLoop();
    }

    private void OnDetectionStay(Collider other)
    {
        detectedPlayer = other;
    }

    private void OnDetectionExit(Collider other)
    {
        StopDialogueLoop();
        StopFollowing();
        detectedPlayer = null;
    }

    #endregion

    // ----------------------------------------------------------------------
    #region Dialogue Loop

    private void StartDialogueLoop()
    {
        if (dialogueLoopCoroutine == null)
        {
            dialogueLoopCoroutine = StartCoroutine(DialogueLoop());
        }
    }

    private void StopDialogueLoop()
    {
        if (dialogueLoopCoroutine != null)
        {
            StopCoroutine(dialogueLoopCoroutine);
            dialogueLoopCoroutine = null;
        }
    }

    private IEnumerator DialogueLoop()
    {
        while (detectedPlayer != null)
        {
            if (detectedPlayer == null) yield break;

            bool canTalkNow = characterActor.Brain().Dialogue().CanStartCharacterToPlayerDialogue();

            if (canTalkNow && !isFollowingPlayer)
            {
                FollowPlayer(detectedPlayer);
            }

            if (!canTalkNow && isFollowingPlayer)
            {
                StopFollowing();
            }

            float waitTime = Random.Range(minAskInterval, maxAskInterval);
            yield return new WaitForSeconds(waitTime);
        }
    }

    #endregion

    // ----------------------------------------------------------------------
    #region Character Actions

    private void FollowPlayer(Collider playerCollider)
    {
        Character playerCharacter = playerCollider.GetComponent<Character>();
        if (playerCharacter == null) return;

        character.Motion.StartFollowingTarget(playerCharacter.transform, 1f, 2f);
        characterActor.Brain().Dialogue().SetIntention(DialogueIntention.ApproachingPlayer);
        isFollowingPlayer = true;
    }

    private void StopFollowing()
    {
        if (isFollowingPlayer)
        {
            character.Motion.StopFollowingTarget();
            characterActor.Brain().Dialogue().ClearIntention();
            isFollowingPlayer = false;
        }
    }

    private void StartDialogue()
    {
        characterBrain.Dialogue().TriggerCharacterDialogueWithPlayer();
    }

    #endregion
}
