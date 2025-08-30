using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
[CreateAssetMenu(menuName = "Dialogue/BackgroundDialogueTopic")]
public class BackgroundDialogueTopic : ScriptableObject
{
    public string TopicName;
    public int maxVariants = 3; // Number of dialogue variants available for this topic

    [Space]
    [Header("Contextual Factors")]
    public bool isTimeBased = false; // Whether this topic is time-based (e.g., morning, afternoon, night)
    public bool isWeatherBased = false; // Whether this topic is weather-based
    public bool isLocationBased = false; // Whether this topic is location-based
    public bool isDependentOnWhoIsNearby = false; // Whether this topic depends on who is nearby
    public List<String> specificCharactersNearby = new List<string>(); // Specific characters that trigger this topic

    public string GetTopicNodeName()
    {
        string nodeName = "";

        nodeName += "{actor}_background_dialogue_";

        nodeName += TopicName.ToLower();

        if (isTimeBased)
        {
            nodeName += "_{time}";
        }

        if (isWeatherBased)
        {
            nodeName += "_{weather}";
        }

        if (isLocationBased)
        {
            nodeName += "_{location}";
        }

        if (isDependentOnWhoIsNearby && specificCharactersNearby.Count > 0)
        {
            // Randomly choose one of the specific characters to include in the node name
            string character = specificCharactersNearby[Random.Range(0, specificCharactersNearby.Count)];
            nodeName += $"_with_{character.ToLower()}";
        }

        if (maxVariants > 0)
        {
            int index = Random.Range(1, maxVariants + 1);
            nodeName += $"_{index}";
        }

        return nodeName;
    }
}