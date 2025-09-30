using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Dialogue/Topic Character Monologue", order = 2)]
public class TopicCharacterMonologue : Topic
{
    public override string GetTopicNodeName()
    {
        string nodeName = "";
        nodeName += "{actor}_topic_character_monologue_";
        nodeName += TopicName.ToLower().Replace(" ", "_");
        return nodeName;
    }
}