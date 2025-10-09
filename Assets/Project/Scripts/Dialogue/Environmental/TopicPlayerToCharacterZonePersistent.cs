using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TriggerCollider))]
public class TopicPlayerToCharacterZonePersistent : MonoBehaviour
{
    [SerializeField] private TopicPlayerToCharacter topic;
    [SerializeField] private List<string> CharactersToAsk;

    private TriggerCollider triggerArea;

    void Start()
    {
        triggerArea = GetComponent<TriggerCollider>();
        triggerArea.TriggerEntered += OnZoneEntered;
    }

    private void OnZoneEntered(Collider other)
    {
        GameManager.Instance.GetSystem<CharacterManager>().AddTopicPlayerToCharacter(topic, CharactersToAsk);
    }
}
