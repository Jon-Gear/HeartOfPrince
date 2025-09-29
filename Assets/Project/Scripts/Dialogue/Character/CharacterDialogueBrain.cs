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

    [Header("Topics")]

    [Tooltip("Dialogue topics the character wants to ask the player")]
    [SerializeField] private List<DialogueTopicFromCharacter> topicsFromCharacterToPlayer = new List<DialogueTopicFromCharacter>();

    [Tooltip("Dialogue topics the player wants to ask the character")]
    [SerializeField] private List<DialogueTopicFromPlayer> topicsFromPlayerToCharacter = new List<DialogueTopicFromPlayer>();

    [Tooltip("Monologue topics the character wants to ask to themselves")]
    [SerializeField] private List<MonologueTopicFromCharacter> monologueTopics = new List<MonologueTopicFromCharacter>();

    [Tooltip("Monologue topics the character wants to ask other characters")]
    [SerializeField] private List<BackgroundDialogueTopic> topicsFromCharacterToOtherCharacter = new List<BackgroundDialogueTopic>();


    [Header("Settings")]


    [Tooltip("Daily activities that were done throughout the day")]
    [SerializeField] private List<DailyActivityReport> dailyActivityReport = new List<DailyActivityReport>();

    private Coroutine backgroundDialogueLoop;

    [Tooltip("Minimum time between background dialogues (seconds).")]
    [SerializeField] private float minInterval = 1.0f;

    [Tooltip("Maximum time between background dialogues (seconds).")]
    [SerializeField] private float maxInterval = 2.0f;



    public void AddTopicFromCharacterToPlayer(DialogueTopicFromCharacter topic)
    {
        if (!topicsFromCharacterToPlayer.Contains(topic))
        {
            topicsFromCharacterToPlayer.Add(topic);
        }
    }

    public void RemoveTopicFromCharacterToPlayer(string topicName)
    {
        foreach(DialogueTopicFromCharacter topic in topicsFromCharacterToPlayer)
        {
            if(topic.TopicName == topicName)
            {
                topicsFromCharacterToPlayer.Remove(topic);
                return;
            }
        }
    }



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
    public void PlayerStartDialogueWithCharacter()
    {
        if (DialogueManager.Instance.IsDialogueRunning())
        {
            return;
        }

        Debug.Log($"Player starting dialogue with {characterName}.");

        string nodeName = $"{characterName.ToLower()}_start";

        DialogueManager.Instance.StartDialogue(nodeName);
    }

    public string GetDialogueTopicOptionText(int index)
    {
        if(index < 0 || index >= topicsFromPlayerToCharacter.Count)
        {
            return "...";
        }
        return $"Talk about {topicsFromPlayerToCharacter[index].TopicName}";
    }

    public string GetDialogueTopicNodeName(int index)
    {
        if (index < 0 || index >= topicsFromPlayerToCharacter.Count)
        {
            return "empty";
        }
        return topicsFromPlayerToCharacter[index].GetTopicNodeName().Replace("{actor}", characterName.ToLower());
    }

    public void AddDialogueTopicFromPlayerToCharacter(DialogueTopicFromPlayer topic)
    {
        if (!topicsFromPlayerToCharacter.Contains(topic))
        {
            topicsFromPlayerToCharacter.Add(topic);
        }
    }

    // Character-Initiated Dialogue Functions
    public bool HasTopicsToAskPlayer()
    {
        return topicsFromCharacterToPlayer.Count > 0;
    }

    public void CharacterStartDialogueWithPlayer()
    {
        if (DialogueManager.Instance.IsDialogueRunning())
        {
            return;
        }
        if (topicsFromCharacterToPlayer.Count == 0)
        {
            return;
        }
        DialogueManager.Instance.StartDialogue(ChooseCharacterToPlayerDialogueNode());
    }

    private string ChooseCharacterToPlayerDialogueNode()
    {
        string nodeName = topicsFromCharacterToPlayer[Random.Range(0, topicsFromCharacterToPlayer.Count)].GetTopicNodeName().Replace("{actor}", characterName.ToLower());
        return nodeName;
    }

    public void AddDialogueTopicFromCharacterToPlayer(DialogueTopicFromCharacter topic)
    {
        if (!topicsFromCharacterToPlayer.Contains(topic))
        {
            topicsFromCharacterToPlayer.Add(topic);
        }
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

            CharacterStartMonologue();
            //CharacterStartDialogueWithCharacter();
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


    private void CharacterStartMonologue()
    {
        if (DialogueManager.Instance.IsDialogueRunning() ||
            DialogueManager.Instance.IsInnerMonologueRunning() ||
            DialogueManager.Instance.IsBackgroundDialogueRunning())
        {
            return;
        }
        if (monologueTopics.Count == 0)
        {
            return;
        }
        DialogueManager.Instance.StartBackgroundDialogue(ChooseMonologueNode());
    }

    private string ChooseMonologueNode()
    {
        string nodeName = monologueTopics[Random.Range(0, monologueTopics.Count)].GetTopicNodeName().Replace("{actor}", characterName.ToLower());
        return nodeName;
    }

    public void AddMonologueTopic(MonologueTopicFromCharacter topic)
    {
        if (!monologueTopics.Contains(topic))
        {
            monologueTopics.Add(topic);
        }
    }

    private void CharacterStartDialogueWithCharacter()
    {
        if (DialogueManager.Instance.IsDialogueRunning() ||
            DialogueManager.Instance.IsInnerMonologueRunning() ||
            DialogueManager.Instance.IsBackgroundDialogueRunning())
        {
            return;
        }
        if (topicsFromCharacterToOtherCharacter.Count == 0)
        {
            return;
        }
        DialogueManager.Instance.StartBackgroundDialogue(ChooseDialogueWithOtherCharacterNode());
    }

    private string ChooseDialogueWithOtherCharacterNode()
    {
        string nodeName = topicsFromCharacterToOtherCharacter[Random.Range(0, topicsFromCharacterToOtherCharacter.Count)].GetTopicNodeName().Replace("{actor}", characterName.ToLower());
        return nodeName;
    }

    public void AddDialogueTopicFromCharacterToCharacter(BackgroundDialogueTopic topic)
    {
        if (!topicsFromCharacterToOtherCharacter.Contains(topic))
        {
            topicsFromCharacterToOtherCharacter.Add(topic);
        }
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