using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
[CreateAssetMenu(menuName = "Dialogue/DialogueTopicFromCharacter")]
public class DialogueTopicFromCharacter : ScriptableObject
{
    public string TopicName = "Topic Name";
    public int maxVariants = 3; // Number of dialogue variants available for this topic
    public string GetTopicNodeName()
    {
        string nodeName = "";
        nodeName += "{actor}_dialogue_topic_from_character_";
        nodeName += TopicName.ToLower();
        if (maxVariants > 0)
        {
            int index = Random.Range(1, maxVariants + 1);
            nodeName += $"_{index}";
        }
        return nodeName;
    }
}
