using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
[CreateAssetMenu(menuName = "Dialogue/General Topic Character Monologue", order = 2)]
public class GeneralTopicCharacterMonologue : TopicCharacterMonologue
{
    public int amountOfVariants = 3; // Number of dialogue variants available for this topic
    public override string GetTopicNodeName()
    {
        string nodeName = base.GetTopicNodeName();

        if (amountOfVariants > 0)
        {
            int index = Random.Range(1, amountOfVariants + 1);
            nodeName += $"_{index}";
        }
        return nodeName;
    }
}