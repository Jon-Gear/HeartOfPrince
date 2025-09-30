using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
[CreateAssetMenu(menuName = "Dialogue/Topic Character to Player", order = 5)]
public class TopicCharacterToPlayer : Topic
{
    public override string GetTopicNodeName()
    {
        string nodeName = "";
        nodeName += "{actor}_topic_character_to_player_";
        nodeName += TopicName.ToLower().Replace(" ", "_");
        return nodeName;
    }
}

