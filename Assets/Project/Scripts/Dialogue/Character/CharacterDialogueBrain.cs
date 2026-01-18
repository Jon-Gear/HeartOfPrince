using GameCreator.Runtime.Behavior;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Dialogue;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Yarn.Unity;
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
    [SerializeField] public string characterName = "Character";

    [Header("Dialogue Topics")]

    [Tooltip("Dialogue topics the character can ask the player.")]
    [SerializeField] public List<string> characterToPlayerTopics = new();

    [Tooltip("Dialogue topics the player can ask the character.")]
    [SerializeField] public List<string> playerToCharacterTopics = new();

    [Tooltip("Monologue topics the character thinks about themselves.")]
    [SerializeField] public List<string> monologueTopics = new();

    [Tooltip("Background dialogue topics between this character and others.")]
    [SerializeField] public List<string> characterToCharacterTopics = new();


    public List<string> PlayerToCharacterTopics() => playerToCharacterTopics;
    public List<string> CharacterToPlayerTopics() => characterToPlayerTopics;
    public List<string> CharacterToCharacterTopics() => characterToCharacterTopics;
    public List<string> MonologueTopics() => monologueTopics;


    private DialogueIntention currentIntention = DialogueIntention.None;
    public DialogueIntention CurrentIntention => currentIntention;
    public bool IsFree => currentIntention == DialogueIntention.None;


    // -------------------
    // Choosing Topics
    // -------------------

    public string ChooseMonologueTopic()
    {
        return monologueTopics[Random.Range(0, monologueTopics.Count)];
    }


    public string ChooseCharacterToPlayerTopic()
    {
        return characterToPlayerTopics[Random.Range(0, characterToPlayerTopics.Count)];
    }



    [YarnFunction("GetCharacterToPlayerTopic")]
    public static string ChooseCharacterToPlayerTopic(string characterName)
    {
        CharacterBrain character = GameManager.Instance.GetSystem<CharacterManager>().GetCharacter(characterName);
        return character.Dialogue().ChooseCharacterToPlayerTopic();
    }

    [YarnFunction("GetCharacterToPlayerTopicAmount")]
    public static int GetCharacterToPlayerTopicAmount(string characterName)
    {
        CharacterBrain character = GameManager.Instance.GetSystem<CharacterManager>().GetCharacter(characterName);
        return character.Dialogue().characterToPlayerTopics.Count;
    }






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
        return (IsFree); //&& !GameManager.Instance.GetSystem<DialogueManager>().main.IsRunning();
    }

    public bool CanStartCharacterToPlayerDialogue()
    {
        return (IsFree || CurrentIntention == DialogueIntention.ApproachingPlayer);// && !GameManager.Instance.GetSystem<DialogueManager>().main.IsRunning() && characterToPlayerTopics.Count > 0;
    }

    public bool CanStartCharacterMonologue()
    {
        var dialogueManager = GameManager.Instance.GetSystem<DialogueManager>();
        return 
            IsFree && 
            !dialogueManager.Primary().IsDialogueRunning && 
            !dialogueManager.Secondary().IsDialogueRunning && 
            monologueTopics.Count > 0;
    }

    public bool CanStartCharacterToCharacterDialogue()
    {
        return IsFree; // && !GameManager.Instance.GetSystem<DialogueManager>().main.IsRunning() && GameManager.Instance.GetSystem<DialogueManager>().IsAnyBackgroundDialogueAvailable() && characterToCharacterTopics.Count > 0;
    }


    // -------------------
    // Player -> Character
    // -------------------

    public void StartPlayerToCharacterDialogue()
    {
        var dialogueManager = GameManager.Instance.GetSystem<DialogueManager>();
        dialogueManager.Primary().StartDialogue("Start");
    }

    [YarnFunction("GetPlayerToCharacterTopicAmount")]
    public static int PlayerToCharacterTopicAmount(string characterName)
    {
        CharacterBrain character = GameManager.Instance.GetSystem<CharacterManager>().GetCharacter(characterName);
        return character.Dialogue().playerToCharacterTopics.Count;
    }

    [YarnCommand("ShufflePlayerToCharacterTopics")]
    public static void ShufflePlayerToCharacterTopics(string characterName)
    {
        Debug.Log($"Shuffling Player to Character topics for {characterName}");
        CharacterBrain character = GameManager.Instance.GetSystem<CharacterManager>().GetCharacter(characterName);

        character.Dialogue().PlayerToCharacterTopics().Shuffle();
    }

    [YarnFunction("GetPlayerToCharacterTopic")]
    public static string GetPlayerToCharacterTopic(string characterName, int index)
    {
        var character = GameManager.Instance.GetSystem<CharacterManager>().GetCharacter(characterName);

        if (character.Dialogue().playerToCharacterTopics.Count == 0 || 
            index >= character.Dialogue().playerToCharacterTopics.Count || 
            index < 0)
        {
            return "...";
        }
        return character.Dialogue().playerToCharacterTopics[index];
    
    }

    // -------------------
    // Player -> Character
    // -------------------

    [YarnCommand("AddPlayerToCharacterTopic")]
    public static void AddPlayerToCharacterTopic(string characterToAsk, string topicName)
    {
        CharacterBrain characterBrain = GameManager.Instance.GetSystem<CharacterManager>().GetCharacter(characterToAsk);

        if (!characterBrain.Dialogue().PlayerToCharacterTopics().Contains(topicName))
        {
            characterBrain.Dialogue().PlayerToCharacterTopics().Add(topicName);
        }
    }

    [YarnCommand("RemovePlayerToCharacterTopic")]
    public static void RemovePlayerToCharacterTopic(string characterToAsk, string topicName)
    {
        CharacterBrain characterBrain = GameManager.Instance.GetSystem<CharacterManager>().GetCharacter(characterToAsk);

        if (characterBrain.Dialogue().PlayerToCharacterTopics().Contains(topicName))
        {
            characterBrain.Dialogue().PlayerToCharacterTopics().Remove(topicName);
        }
    }

    // -------------------
    // Character -> Player
    // -------------------

    [YarnCommand("AddCharacterToPlayerTopic")]
    public static void AddToCharacterTopicToAskPlayer(string characterName, string topicName)
    {
        CharacterBrain characterBrain = GameManager.Instance.GetSystem<CharacterManager>().GetCharacter(characterName);

        if (!characterBrain.Dialogue().CharacterToPlayerTopics().Contains(topicName))
        {
            characterBrain.Dialogue().CharacterToPlayerTopics().Add(topicName);
        }
    }

    [YarnCommand("RemoveCharacterToPlayerTopic")]
    public static void RemoveCharacterTopicToAskPlayer(string characterName, string topicName)
    {
        CharacterBrain characterBrain = GameManager.Instance.GetSystem<CharacterManager>().GetCharacter(characterName);

        if (characterBrain.Dialogue().CharacterToPlayerTopics().Contains(topicName))
        {
            characterBrain.Dialogue().CharacterToPlayerTopics().Remove(topicName);
        }
    }

    // -------------------
    // Character -> Character
    // -------------------


    public bool TriggerCharacterToCharacterDialogue(List<Actor> nearbyActors)
    {
        return false;
        //if (!CanStartCharacterToCharacterDialogue())
        //    return false;

        //string bestTopicNodeName = GetBestAvailableTopic(nearbyActors);

        //if(bestTopicNodeName == "")
        //{
        //    return false;
        //}
        //SetIntention(DialogueIntention.ToCharacter);
        ////GameManager.Instance.GetSystem<DialogueManager>().StartBackgroundDialogue(bestTopicNodeName);
        //return true;
    }

    //private String GetBestAvailableTopic(List<Actor> nearbyActors)
    //{
    //    TopicCharacterToCharacter bestTopic = null;
    //    int bestScore = -1;

    //    foreach (var topic in characterToCharacterTopics)
    //    {
    //        // 1. Check if all required chars are present
    //        bool allRequiredPresent = topic.OtherActors.All(req =>
    //        {
    //            var actor = nearbyActors.FirstOrDefault(c => c.actorName == req);
    //            return actor != null && actor.Brain().Dialogue().IsFree;
    //        });

    //        if (!allRequiredPresent) continue;

    //        int score = topic.OtherActors.Count;

    //        // 3. Keep highest scoring topic
    //        if (score > bestScore)
    //        {
    //            bestScore = score;
    //            bestTopic = topic;
    //        }
    //    }

    //    if(bestTopic == null)
    //    {
    //        return "";
    //    }
    //    else
    //    {
    //        return bestTopic.GetTopicNodeName().Replace("{actor}", characterName.ToLower());
    //    }

    //}
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