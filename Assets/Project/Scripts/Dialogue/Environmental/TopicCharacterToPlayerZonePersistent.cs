using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TriggerCollider))]
public class TopicCharacterToPlayerZonePersistent : MonoBehaviour
{
    [SerializeField] private TopicCharacterToPlayer topic;
    [SerializeField] private List<string> AffectedCharacters;
    private TriggerCollider triggerArea;

    void Start()
    {
        triggerArea = GetComponent<TriggerCollider>();
        triggerArea.TriggerEntered += OnZoneEntered;
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
}
