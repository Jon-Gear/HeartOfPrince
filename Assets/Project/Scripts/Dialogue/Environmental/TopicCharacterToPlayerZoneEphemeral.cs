using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TriggerCollider))]
public class TopicCharacterToPlayerZoneEphemeral : MonoBehaviour
{
    [SerializeField] private TopicCharacterToPlayer topic;
    [SerializeField] private List<string> AffectedCharacters;
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
        if (!AffectedCharacters.Contains(otherActor.actorName))
        {
            return;
        }
        //otherActor.Brain().Dialogue().AddCharacterToPlayerTopic(topic);
    }
    private void OnZoneExited(Collider other)
    {
        Actor otherActor = other.gameObject.GetComponent<Actor>();
        if (otherActor == null)
        {
            return;
        }
        if (!AffectedCharacters.Contains(otherActor.actorName))
        {
            return;
        }
        //otherActor.Brain().Dialogue().RemoveCharacterToPlayerTopic(topic);
    }
}
