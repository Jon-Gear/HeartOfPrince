using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
[CreateAssetMenu(menuName = "Dialogue/Dialogue Topic From Character to Character", order = 7)]
public class BackgroundDialogueTopic : Topic
{
    public List<String> OtherActors = new List<string>();
    public override string GetTopicNodeName()
    {
        string nodeName = "";

        nodeName += "{actor}_background_dialogue_";

        nodeName += TopicName.ToLower().Replace(" ", "_");

        nodeName += "_with_";

        for(int i = 0; i < OtherActors.Count; i++)
        {
            nodeName += OtherActors[i].ToLower();
            if (i < OtherActors.Count - 1)
            {
                nodeName += "_and_";
            }
        }

        return nodeName;
    }
}