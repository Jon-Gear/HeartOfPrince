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
[CreateAssetMenu(menuName = "Dialogue/DialogueTopic")]
public class DialogueTopic : ScriptableObject
{
    public string TopicName;
    public int maxVariants = 3; // Number of dialogue variants available for this topic
    
    public string GetTopicNodeName()
    {
        string nodeName = "";

        nodeName += "{actor}_dialogue_";

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



public class CharacterDialogueBrain : MonoBehaviour
{
    [SerializeField] private string characterName = "Character";

    [SerializeField] private List<DialogueTopic> dialogueTopics = new List<DialogueTopic>();
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


    public void StartDialogue()
    {
        if (DialogueManager.Instance.IsDialogueRunning())
        {
            return;
        }

        if (dialogueTopics.Count == 0)
        {
            return;
        }

        DialogueManager.Instance.StartDialogue(ChooseDialogueNode());
    }

    public void StartBackgroundDialogueLoop()
    {
        if (DialogueManager.Instance.IsDialogueRunning() ||
            DialogueManager.Instance.IsInnerMonologueRunning() ||
            DialogueManager.Instance.IsBackgroundDialogueRunning())
        {
            return;
        }
        if (backgroundDialogueTopics.Count == 0)
        {
            return;
        }

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


    private string ChooseDialogueNode()
    {
        
        string nodeName = dialogueTopics[Random.Range(0, dialogueTopics.Count)].GetTopicNodeName().Replace("{actor}", characterName.ToLower());
        return nodeName;
    }

    private string ChooseBackgroundDialogueNode()
    {
        string nodeName = backgroundDialogueTopics[Random.Range(0, backgroundDialogueTopics.Count)].GetTopicNodeName().Replace("{actor}", characterName.ToLower());
        return nodeName;
    }


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