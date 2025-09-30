using GameCreator.Runtime.Behavior;
using GameCreator.Runtime.Dialogue;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [Header("Dialogue Topics")]

    [Tooltip("Dialogue topics the character can ask the player.")]
    [SerializeField] private List<TopicCharacterToPlayer> characterToPlayerTopics = new();

    [Tooltip("Dialogue topics the player can ask the character.")]
    [SerializeField] private List<TopicPlayerToCharacter> playerToCharacterTopics = new();

    [Tooltip("Monologue topics the character thinks about themselves.")]
    [SerializeField] private List<TopicCharacterMonologue> monologueTopics = new();

    [Tooltip("Background dialogue topics between this character and others.")]
    [SerializeField] private List<TopicCharacterToCharacter> characterToCharacterTopics = new();

    [Header("Settings")]

    [Tooltip("Daily activities done throughout the day.")]
    [SerializeField] private List<DailyActivityReport> dailyActivityReports = new();


    // -------------------
    // Unity Events
    // -------------------

    private void Start()
    {
        
    }

    private void Update()
    {
        // Reserved for runtime checks if needed
    }

    // -------------------
    // Player -> Character
    // -------------------

    public void AddPlayerToCharacterTopic(TopicPlayerToCharacter topic)
    {
        if (!playerToCharacterTopics.Contains(topic))
            playerToCharacterTopics.Add(topic);
    }

    public void RemovePlayerToCharacterTopic(string topicName)
    {
        playerToCharacterTopics.RemoveAll(t => t.TopicName == topicName);
    }

    public string GetPlayerTopicOptionText(int index)
    {
        if (index < 0 || index >= playerToCharacterTopics.Count)
            return "...";

        return $"Talk about {playerToCharacterTopics[index].TopicName}";
    }

    public string GetPlayerTopicNodeName(int index)
    {
        if (index < 0 || index >= playerToCharacterTopics.Count)
            return "empty";

        return playerToCharacterTopics[index]
            .GetTopicNodeName()
            .Replace("{actor}", characterName.ToLower());
    }

    public void PlayerStartDialogue()
    {
        if (DialogueManager.Instance.IsDialogueRunning())
            return;

        string nodeName = $"{characterName.ToLower()}_start";
        DialogueManager.Instance.StartDialogue(nodeName);
    }

    // -------------------
    // Character -> Player
    // -------------------

    public void AddCharacterToPlayerTopic(TopicCharacterToPlayer topic)
    {
        if (!characterToPlayerTopics.Contains(topic))
            characterToPlayerTopics.Add(topic);
    }

    public void RemoveCharacterToPlayerTopic(string topicName)
    {
        Debug.Log("Before");
        for (int i = 0; i < characterToPlayerTopics.Count; i++)
        {
            Debug.Log($" {i}: Character to player topic: {characterToPlayerTopics[i].TopicName}");
        }
        characterToPlayerTopics.RemoveAll(t => t.TopicName == topicName);

        Debug.Log("After");
        for (int i = 0; i < characterToPlayerTopics.Count; i++)
        {
            Debug.Log($" {i}: Character to player topic: {characterToPlayerTopics[i].TopicName}");
        }
        Debug.Log("End Removal");
    }

    public bool HasTopicsForPlayer() => characterToPlayerTopics.Count > 0;

    public void CharacterStartDialogueWithPlayer()
    {
        if (DialogueManager.Instance.IsDialogueRunning() || characterToPlayerTopics.Count == 0)
            return;

        string nodeName = GetRandomCharacterToPlayerNode();

        Debug.Log($"Starting character to player dialogue with node: {nodeName}");

        DialogueManager.Instance.StartDialogue(nodeName);
    }

    private string GetRandomCharacterToPlayerNode()
    {
        Debug.Log($"Character to player topics count: {characterToPlayerTopics.Count}");

        for (int i = 0; i < characterToPlayerTopics.Count; i++)
        {
            Debug.Log($" {i}: Character to player topic: {characterToPlayerTopics[i].TopicName}");
        }

        return characterToPlayerTopics[Random.Range(0, characterToPlayerTopics.Count)]
            .GetTopicNodeName()
            .Replace("{actor}", characterName.ToLower());
    }

    // -------------------
    // Character Monologue
    // -------------------

    public void AddCharacterMonologueTopic(TopicCharacterMonologue topic)
    {
        if (!monologueTopics.Contains(topic))
            monologueTopics.Add(topic);
    }

    public void RemoveCharacterMonologueTopic(string topicName)
    {
        monologueTopics.RemoveAll(t => t.TopicName == topicName);
    }

    public void RemoveCharacterMonologueTopic(TopicCharacterMonologue topic)
    {
        if (monologueTopics.Contains(topic))
            monologueTopics.Remove(topic);
    }

    public void TriggerMonologue()
    {
        if (DialogueManager.Instance.IsDialogueRunning() ||
            DialogueManager.Instance.IsInnerMonologueRunning() ||
            DialogueManager.Instance.IsBackgroundDialogueRunning() ||
            monologueTopics.Count == 0)
            return;

        DialogueManager.Instance.StartBackgroundDialogue(GetRandomMonologueNode());
    }

    private string GetRandomMonologueNode()
    {
        return monologueTopics[Random.Range(0, monologueTopics.Count)]
            .GetTopicNodeName()
            .Replace("{actor}", characterName.ToLower());
    }

    // -------------------
    // Character -> Character
    // -------------------

    public void AddCharacterToCharacterTopic(TopicCharacterToCharacter topic)
    {
        if (!characterToCharacterTopics.Contains(topic))
            characterToCharacterTopics.Add(topic);
    }

    public void RemoveCharacterToCharacterTopic(string topicName)
    {
        characterToCharacterTopics.RemoveAll(t => t.TopicName == topicName);
    }

    public bool TriggerCharacterToCharacterDialogue(List<Actor> nearbyActors)
    {
        if (DialogueManager.Instance.IsDialogueRunning() ||
            DialogueManager.Instance.IsInnerMonologueRunning() ||
            DialogueManager.Instance.IsBackgroundDialogueRunning() ||
            characterToCharacterTopics.Count == 0)
            return false;

        string bestTopicNodeName = GetBestAvailableTopic(nearbyActors);

        if(bestTopicNodeName == "")
        {
            return false;
        }

        DialogueManager.Instance.StartBackgroundDialogue(bestTopicNodeName);
        return true;
    }

    private String GetBestAvailableTopic(List<Actor> nearbyActors)
    {
        TopicCharacterToCharacter bestTopic = null;
        int bestScore = -1;

        foreach (var topic in characterToCharacterTopics)
        {
            // 1. Check if all required chars are present
            bool allRequiredPresent = topic.OtherActors
                .All(req => nearbyActors.Exists(c => c.actorName == req));
            if (!allRequiredPresent) continue;

            int score = topic.OtherActors.Count;

            // 3. Keep highest scoring topic
            if (score > bestScore)
            {
                bestScore = score;
                bestTopic = topic;
            }
        }

        if(bestTopic == null)
        {
            return "";
        }
        else
        {
            return bestTopic.GetTopicNodeName().Replace("{actor}", characterName.ToLower());
        }

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