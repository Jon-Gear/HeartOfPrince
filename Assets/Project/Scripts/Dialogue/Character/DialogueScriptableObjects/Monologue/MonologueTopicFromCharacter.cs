using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Dialogue/Monologue Topic From Character to Themself", order = 2)]
public class MonologueTopicFromCharacter : Topic
{
    public override string GetTopicNodeName()
    {
        string nodeName = "";
        nodeName += "{actor}_monologue_topic_from_character_";
        nodeName += TopicName.ToLower().Replace(" ", "_");
        return nodeName;
    }
}