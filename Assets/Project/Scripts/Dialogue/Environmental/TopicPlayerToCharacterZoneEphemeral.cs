using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TriggerCollider))]
public class TopicPlayerToCharacterZoneEphemeral : MonoBehaviour
{
    [SerializeField] private TopicPlayerToCharacter topic;
    [SerializeField] private List<string> CharactersToAsk;

    private TriggerCollider triggerArea;

    void Start()
    {
        triggerArea = GetComponent<TriggerCollider>();
        triggerArea.TriggerEntered += OnZoneEntered;
        triggerArea.TriggerExited += OnZoneExited;
    }

    private void OnZoneEntered(Collider other)
    {
        GameManager.Instance.GetSystem<CharacterManager>().AddTopicPlayerToCharacter(topic, CharactersToAsk);
    }
    private void OnZoneExited(Collider other)
    {
        GameManager.Instance.GetSystem<CharacterManager>().RemoveTopicPlayerToCharacter(topic, CharactersToAsk);
    }
}
