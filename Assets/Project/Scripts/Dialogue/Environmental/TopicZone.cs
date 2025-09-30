using UnityEngine;
using UnityEngine.SceneManagement;

public class TopicZone : MonoBehaviour
{
    [SerializeField] private TopicCharacterMonologue topic;
    private TriggerCollider triggerArea;

    void Start()
    {
        triggerArea = GetComponent<TriggerCollider>();
        triggerArea.TriggerEntered += OnZoneEntered;
        triggerArea.TriggerExited += OnZoneExited;
    }

    private void OnZoneEntered(Collider other)
    {
        Actor otherActor = other.gameObject.GetComponent<Actor>();
        if (otherActor == null)
        {
            return;
        }
        CharacterDialogueBrain dialogueBrain = CharacterManager.Instance.GetCharacter(otherActor.actorName);

        dialogueBrain.AddCharacterMonologueTopic(topic);


    }
    private void OnZoneExited(Collider other)
    {
        Actor otherActor = other.gameObject.GetComponent<Actor>();
        if (otherActor == null)
        {
            return;
        }
        CharacterDialogueBrain dialogueBrain = CharacterManager.Instance.GetCharacter(otherActor.actorName);

        dialogueBrain.RemoveCharacterMonologueTopic(topic);
    }
}
