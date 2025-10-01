using UnityEngine;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(TriggerCollider))]
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
        CharacterBrain brain = CharacterManager.Instance.GetCharacter(otherActor.actorName);

        brain.Dialogue().AddCharacterMonologueTopic(topic);


    }
    private void OnZoneExited(Collider other)
    {
        Actor otherActor = other.gameObject.GetComponent<Actor>();
        if (otherActor == null)
        {
            return;
        }
        CharacterBrain brain = CharacterManager.Instance.GetCharacter(otherActor.actorName);

        brain.Dialogue().RemoveCharacterMonologueTopic(topic);
    }
}
