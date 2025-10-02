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


public enum DialogueIntention
{
    None,                // free to start something
    ToPlayer,            // character intends to talk to the player
    ToCharacter,         // character intends to talk to another NPC
    Monologue,           // character intends to monologue
    SpokenTo,            // character has been spoken to and is responding
    ApproachingPlayer,   // character is approaching the player to start dialogue
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

    
    private DialogueIntention currentIntention = DialogueIntention.None;
    public DialogueIntention CurrentIntention => currentIntention;
    public bool IsFree => currentIntention == DialogueIntention.None;
    

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

    public void SetIntention(DialogueIntention intention)
    {
        currentIntention = intention;
    }

    public void ClearIntention()
    {
        currentIntention = DialogueIntention.None;
    }

    public bool CanStartPlayerToCharacterDialogue()
    {
        return IsFree && !DialogueManager.Instance.main.IsRunning();
    }

    public bool CanStartCharacterToPlayerDialogue()
    {
        return (IsFree || CurrentIntention == DialogueIntention.ApproachingPlayer) && !DialogueManager.Instance.main.IsRunning() && characterToPlayerTopics.Count > 0;
    }

    public bool CanStartCharacterMonologue()
    {
        return IsFree && !DialogueManager.Instance.main.IsRunning() && DialogueManager.Instance.IsAnyBackgroundDialogueAvailable() && monologueTopics.Count > 0;
    }

    public bool CanStartCharacterToCharacterDialogue()
    {
        return IsFree && !DialogueManager.Instance.main.IsRunning() && DialogueManager.Instance.IsAnyBackgroundDialogueAvailable() && characterToCharacterTopics.Count > 0;
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

    public void TriggerPlayerDialogueWithCharacter()
    {
        if (!CanStartPlayerToCharacterDialogue())
        {
            return;
        }
        string nodeName = $"{characterName.ToLower()}_start";
        SetIntention(DialogueIntention.SpokenTo);
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
        characterToPlayerTopics.RemoveAll(t => t.TopicName == topicName);
    }

    public bool HasTopicsForPlayer() => characterToPlayerTopics.Count > 0;

    public void TriggerCharacterDialogueWithPlayer()
    {
        if(!CanStartCharacterToPlayerDialogue())
        {
            return;
        }
        SetIntention(DialogueIntention.ToPlayer);
        DialogueManager.Instance.StartDialogue(GetRandomCharacterToPlayerNode());
    }

    private string GetRandomCharacterToPlayerNode()
    {
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
        if (!CanStartCharacterMonologue())
            return;
        SetIntention(DialogueIntention.Monologue);
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
        if (!CanStartCharacterToCharacterDialogue())
            return false;

        string bestTopicNodeName = GetBestAvailableTopic(nearbyActors);

        if(bestTopicNodeName == "")
        {
            return false;
        }
        SetIntention(DialogueIntention.ToCharacter);
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
            bool allRequiredPresent = topic.OtherActors.All(req =>
            {
                var actor = nearbyActors.FirstOrDefault(c => c.actorName == req);
                return actor != null && actor.Brain().Dialogue().IsFree;
            });

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