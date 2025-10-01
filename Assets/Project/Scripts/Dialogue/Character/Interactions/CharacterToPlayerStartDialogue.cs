using GameCreator.Runtime.Characters;
using UnityEngine;

public class CharacterToPlayerStartDialogue : MonoBehaviour
{
    [SerializeField] private Actor characterActor;

    [SerializeField] private TriggerCollider interactionCollider;
    [SerializeField] private TriggerCollider detectionCollider;

    private Character character;
    private CharacterBrain characterBrain;
    
    private bool isFollowingPlayerToAskTopic = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        character = characterActor.gameObject.GetComponent<Character>();
        characterBrain = CharacterManager.Instance.GetCharacter(characterActor.actorName);

        interactionCollider.TriggerEntered += OnInteractInRange;
        detectionCollider.TriggerEntered += OnDetectionInRange;

        detectionCollider.TriggerExited += OnDetectionOutOfRange;
        interactionCollider.TriggerExited += OnDetectionOutOfRange;
    }

    private void OnDestroy()
    {
        interactionCollider.TriggerEntered -= OnInteractInRange;
        detectionCollider.TriggerEntered -= OnDetectionInRange;

        detectionCollider.TriggerExited -= OnDetectionOutOfRange;
        interactionCollider.TriggerExited -= OnDetectionOutOfRange;
    }

    private void OnInteractInRange(Collider other)
    {
        if (!characterBrain.Dialogue().HasTopicsForPlayer())
        {
            return;
        }

        Talk();
        StopFollow();
    }

    private void OnDetectionInRange(Collider other)
    {
        if(!characterBrain.Dialogue().HasTopicsForPlayer())
        {
            return;
        }

        Follow(other);
    }

    private void OnDetectionOutOfRange(Collider other)
    {
        if(!isFollowingPlayerToAskTopic)
        {
            return;
        }
        StopFollow();
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

        character.Motion.StartFollowingTarget(targetCharacter.transform, 0.5f, 2f);

        isFollowingPlayerToAskTopic = true;
    }

    void StopFollow()
    {
        character.Motion.StopFollowingTarget();
        isFollowingPlayerToAskTopic = false;
    }
}
