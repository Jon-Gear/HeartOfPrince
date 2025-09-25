using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Dialogue/Monologue Topic From Player to Themself", order = 1)]
public class MonologueTopicFromPlayer : Topic
{
    public override string GetTopicNodeName()
    {
        string nodeName = "";
        nodeName += "monologue_topic_from_player_";
        nodeName += TopicName.ToLower().Replace(" ", "_");
        return nodeName;
    }
}
