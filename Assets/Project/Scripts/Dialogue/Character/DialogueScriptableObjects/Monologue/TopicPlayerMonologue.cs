using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Dialogue/Topic Player Monologue", order = 1)]
public class TopicPlayerMonologue : Topic
{
    public override string GetTopicNodeName()
    {
        string nodeName = "";
        nodeName += "topic_player_monologue_";
        nodeName += TopicName.ToLower().Replace(" ", "_");
        return nodeName;
    }
}
