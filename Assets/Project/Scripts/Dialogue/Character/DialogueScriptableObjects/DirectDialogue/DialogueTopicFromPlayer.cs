using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
[CreateAssetMenu(menuName = "Dialogue/Dialogue Topic From Player to Character", order = 3)]
public class DialogueTopicFromPlayer : Topic
{
    public override string GetTopicNodeName()
    {
        string nodeName = "";
        nodeName += "{actor}_dialogue_topic_from_player_";
        nodeName += TopicName.ToLower().Replace(" ", "_");
        return nodeName;
    }
}
