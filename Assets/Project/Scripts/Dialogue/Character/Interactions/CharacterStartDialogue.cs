using GameCreator.Runtime.Characters;
using UnityEngine;

public class CharacterStartDialogue : MonoBehaviour
{
    [SerializeField] private Actor characterActor;

    [SerializeField] private TriggerCollider interactionCollider;
    [SerializeField] private TriggerCollider detectionCollider;

    private Character character;
    private CharacterDialogueBrain characterDialogueBrain;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        character = characterActor.gameObject.GetComponent<Character>();
        characterDialogueBrain = CharacterManager.Instance.GetCharacter(characterActor.actorName);

        interactionCollider.TriggerEntered += OnInteractInRange;
        detectionCollider.TriggerEntered += OnDetectionInRange;

        detectionCollider.TriggerExited += OnDetectionOutOfRange;
        interactionCollider.TriggerExited += OnDetectionOutOfRange;
    }

    private void OnInteractInRange(Collider other)
    {
        if (!characterDialogueBrain.HasTopicsToAskPlayer())
        {
            return;
        }

        Talk();
        StopFollow();
    }

    private void OnDetectionInRange(Collider other)
    {
        if(!characterDialogueBrain.HasTopicsToAskPlayer())
        {
            return;
        }

        Follow(other);
    }

    private void OnDetectionOutOfRange(Collider other)
    {
        StopFollow();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    void Talk()
    {
        characterDialogueBrain.CharacterStartDialogueWithPlayer();
    }

    void Follow(Collider other)
    {
        Character targetCharacter = other.gameObject.GetComponent<Character>();

        character.Motion.StartFollowingTarget(targetCharacter.transform, 0.5f, 2f);
    }

    void StopFollow()
    {
        character.Motion.StopFollowingTarget();
    }
}
