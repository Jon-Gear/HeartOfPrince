using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
[CreateAssetMenu(menuName = "Dialogue/Dialogue Topic From Character to Player", order = 5)]
public class DialogueTopicFromCharacter : Topic
{
    public override string GetTopicNodeName()
    {
        string nodeName = "";
        nodeName += "{actor}_dialogue_topic_from_character_";
        nodeName += TopicName.ToLower().Replace(" ", "_");
        return nodeName;
    }
}
