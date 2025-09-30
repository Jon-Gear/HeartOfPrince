using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
[CreateAssetMenu(menuName = "Dialogue/Topic From Player to Character", order = 3)]
public class TopicPlayerToCharacter : Topic
{
    public override string GetTopicNodeName()
    {
        string nodeName = "";
        nodeName += "{actor}_topic_player_to_character_";
        nodeName += TopicName.ToLower().Replace(" ", "_");
        return nodeName;
    }
}
