using GameCreator.Runtime.Behavior;
using GameCreator.Runtime.Dialogue;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[Serializable]
[CreateAssetMenu(menuName = "Dialogue/DialogueTopicFromPlayer")]
public class DialogueTopicFromPlayer : ScriptableObject
{
    public string TopicName = "Topic Name";
    public string OptionText = "Can I ask you about this topic?"; 

    public int maxVariants = 3; // Number of dialogue variants available for this topic
    
    public string GetTopicNodeName()
    {
        string nodeName = "";

        nodeName += "{actor}_dialogue_topic_from_player_";

        nodeName += TopicName.ToLower();

        if (maxVariants > 0)
        {
            int index = Random.Range(1, maxVariants + 1);
            nodeName += $"_{index}";
        }

        return nodeName;
    }
}

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

        if(isTimeBased)
        {
            nodeName += "_{time}";
        }

        if(isWeatherBased)
        {
            nodeName += "_{weather}";
        }

        if(isLocationBased)
        {
            nodeName += "_{location}";
        }

        if(isDependentOnWhoIsNearby && specificCharactersNearby.Count > 0)
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


[Serializable]
[CreateAssetMenu(menuName = "Dialogue/DailyActivityReport")]
public class DailyActivityReport : ScriptableObject
{
    public string activityName = "Activity";

    public string GetTopicNodeName()
    {
        string nodeName = "";

        nodeName += "{actor}_activity_";

        nodeName += activityName.ToLower();

        return nodeName;
    }
}


public class CharacterDialogueBrain : MonoBehaviour
{
    [SerializeField] private string characterName = "Character";

    [Tooltip("Dialogue topics the character wants to ask the player")]
    [SerializeField] private List<DialogueTopicFromCharacter> topicsFromCharacter = new List<DialogueTopicFromCharacter>();

    [Tooltip("Dialogue topics the player wants to ask the character")]
    [SerializeField] private List<DialogueTopicFromPlayer> topicsFromPlayer = new List<DialogueTopicFromPlayer>();

    [Tooltip("Daily activities that were done throughout the day")]
    [SerializeField] private List<DailyActivityReport> dailyActivityReport = new List<DailyActivityReport>();

    [Tooltip("Dialogue topics the character talks about in the background")]
    [SerializeField] private List<BackgroundDialogueTopic> backgroundDialogueTopics = new List<BackgroundDialogueTopic>();




    private Coroutine backgroundDialogueLoop;

    [Tooltip("Minimum time between background dialogues (seconds).")]
    [SerializeField] private float minInterval = 1.0f;

    [Tooltip("Maximum time between background dialogues (seconds).")]
    [SerializeField] private float maxInterval = 2.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Player-Initiated Dialogue Functions
    public void PlayerStartDialogue()
    {
        if (DialogueManager.Instance.IsDialogueRunning())
        {
            return;
        }
        
        string nodeName = $"{characterName.ToLower()}_start";

        DialogueManager.Instance.StartDialogue(nodeName);
    }

    public string GetDialogueTopicOptionText(int index)
    {
        if(index < 0 || index >= topicsFromPlayer.Count)
        {
            return "...";
        }
        return topicsFromPlayer[index].OptionText;
    }

    public string GetDialogueTopicNodeName(int index)
    {
        if (index < 0 || index >= topicsFromPlayer.Count)
        {
            DialogueManager.Instance.StopDialogue();
            return "...";
        }
        return topicsFromPlayer[index].GetTopicNodeName().Replace("{actor}", characterName.ToLower());
    }

    public void AddDialogueTopicFromPlayer(DialogueTopicFromPlayer topic)
    {
        if (!topicsFromPlayer.Contains(topic))
        {
            topicsFromPlayer.Add(topic);
        }
    }

    // Character-Initiated Dialogue Functions
    public void CharacterStartDialogue()
    {
        if (DialogueManager.Instance.IsDialogueRunning())
        {
            return;
        }
        if (topicsFromCharacter.Count == 0)
        {
            Debug.LogWarning($"{characterName} has no topics to discuss with the player.");
            return;
        }
        DialogueManager.Instance.StartDialogue(ChooseCharacterDialogueNode());
    }

    private string ChooseCharacterDialogueNode()
    {
        string nodeName = topicsFromCharacter[Random.Range(0, topicsFromCharacter.Count)].GetTopicNodeName().Replace("{actor}", characterName.ToLower());
        return nodeName;
    }



    // Background Dialogue Functions

    public void StartBackgroundDialogueLoop()
    {
        if (DialogueManager.Instance.IsDialogueRunning() ||
            DialogueManager.Instance.IsInnerMonologueRunning() ||
            DialogueManager.Instance.IsBackgroundDialogueRunning())
        {
            return;
        }
        /*
        if (backgroundDialogueTopics.Count == 0)
        {
            return;
        }
        */
        Debug.Log($"Starting background dialogue loop for {characterName}.");

        backgroundDialogueLoop = StartCoroutine(BackgroundDialogueLoop());
    }

    private IEnumerator BackgroundDialogueLoop()
    {
        while (true)
        {
            // Wait a random amount of time between yaps
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            // Skip if a dialogue is already running
            if (DialogueManager.Instance.IsDialogueRunning() ||
                DialogueManager.Instance.IsInnerMonologueRunning() ||
                DialogueManager.Instance.IsBackgroundDialogueRunning())
            {
                continue;
            }

            if(backgroundDialogueTopics.Count == 0)
            {
                continue;
            }

            DialogueManager.Instance.StartBackgroundDialogue(ChooseBackgroundDialogueNode());
        }
    }

    public void StopBackgroundDialogueLoop()
    {
        if (backgroundDialogueLoop != null)
        {
            Debug.Log($"Stopping background dialogue loop for {characterName}.");
            StopCoroutine(backgroundDialogueLoop);
        }
    }

    
    public void AddBackgroundDialogueTopic(BackgroundDialogueTopic topic)
    {
        if (!backgroundDialogueTopics.Contains(topic))
        {
            backgroundDialogueTopics.Add(topic);
        }
    }

    public void RemoveBackgroundDialogueTopic(BackgroundDialogueTopic topic)
    {
        if (backgroundDialogueTopics.Contains(topic))
        {
            backgroundDialogueTopics.Remove(topic);
        }
    }

    
    private string ChooseBackgroundDialogueNode()
    {
        string nodeName = backgroundDialogueTopics[Random.Range(0, backgroundDialogueTopics.Count)].GetTopicNodeName().Replace("{actor}", characterName.ToLower());
        return nodeName;
    }

    // Misc Functions

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        StopBackgroundDialogueLoop();
    }
}

/*

1. Contextual Factors

These determine when a line is appropriate.

Time of Day -> morning greetings vs tired night talk.

Weather -> hot, rainy, cold comments.

Location -> inside hideout vs on the street vs mosque.

Current Activity -> scavenging, eating, resting.

Who’s Nearby -> group chatter vs private whispers.

Player Progress -> what missions/quests Prince has done.

2. Relational Factors

How Nacho (or any kid) feels about Prince / others.

Trust Level -> “Boss, I’ll do it right away” vs “I dunno about this, man…”

Mood / Emotion -> bored, cheerful, tired, scared.

History -> references past events the player has done.

3. Variation & Naturalism

Avoid repetition fatigue.

Variant Lines (you already have maxVariants).

Probability Weighting (rare “golden” lines).

Cooldowns (don’t let same topic repeat too soon).

Chained Dialogue -> background mutter -> Prince can comment -> leads into real conversation.

4. Types of Dialogue

To mix things up:

Foreground (active) -> player talks to Nacho.

Background (passive) -> Nacho mutters to himself, sings, jokes with another kid.

Reactive Comments -> player actions trigger remarks (“Whoa, boss stole that smooth!”).

Ambient Flavor -> jokes, stories, kids teasing each other.

Philosophical / Reflective -> Nacho thinking about life, streets, or Prince’s health.

5. Design Patterns for Dialogue

Topic Pools (like “food,” “weather,” “plans,” “other gang kids”).

State-Based Nodes -> e.g., nacho_dialogue_trust_high_1.

Dynamic Insertions -> e.g., {playerName} or {currentWeather}.

Escalation Over Time -> as trust/community grows, dialogue tone shifts.

*/