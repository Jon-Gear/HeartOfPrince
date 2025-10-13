using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;
using Yarn.Unity;

public class YarnCommands : MonoBehaviour
{

    /*
    
    Ideas for commands:
    - Checking a value of something


    Other ideas:
    - Background dialogue between other characters (like in NITW)
    */


    [YarnCommand("Follow")]
    public static void Follow(string actorName, string targetName)
    {
        var actorRegistry = GameManager.Instance.GetSystem<ActorRegistry>();
        // Find the actor by name
        Actor actor = actorRegistry.GetActorByName(actorName);
        if (actor == null)
        {
            Debug.LogWarningFormat("Cannot find actor named {0}!", actorName);
            return;
        }
        // Find the target by name
        Actor target = actorRegistry.GetActorByName(targetName);
        if (target == null)
        {
            Debug.LogWarningFormat("Cannot find target named {0}!", targetName);
            return;
        }

        Character actorCharacter = actor.gameObject.GetComponent<Character>();
        Character targetCharacter = target.gameObject.GetComponent<Character>();

        actorCharacter.Motion.StartFollowingTarget(targetCharacter.transform, 0.5f, 2f);
    }

    [YarnCommand("StopFollow")]
    public static void StopFollow(string actorName)
    {
        var actorRegistry = GameManager.Instance.GetSystem<ActorRegistry>();
        // Find the actor by name
        Actor actor = actorRegistry.GetActorByName(actorName);
        if (actor == null)
        {
            Debug.LogWarningFormat("Cannot find actor named {0}!", actorName);
            return;
        }
        
        Character actorCharacter = actor.gameObject.GetComponent<Character>();
        
        actorCharacter.Motion.StopFollowingTarget();
    }



    // Dialogue Options Management
    [YarnFunction("GetDialogueTopicOptionText")]
    public static string GetDialogueTopicOptionText(string characterName, int index)
    {
        CharacterBrain characterBrain = GameManager.Instance.GetSystem<CharacterManager>().GetCharacter(characterName);
        return characterBrain.Dialogue().GetPlayerTopicOptionText(index);
    }

    [YarnFunction("GetDialogueTopicNodeName")]
    public static string GetDialogueTopicNodeName(string characterName, int index)
    {
        CharacterBrain characterBrain = GameManager.Instance.GetSystem<CharacterManager>().GetCharacter(characterName);
        return characterBrain.Dialogue().GetPlayerTopicNodeName(index);
    }



    [YarnFunction("GetPlayerStat")]
    public static float GetPlayerStat(string statID)
    {
        var characterManager = GameManager.Instance.GetSystem<CharacterManager>();
        var playerCharacter = characterManager.GetPlayerCharacter();
        return playerCharacter.Traits().GetStat(statID);
    }

    [YarnFunction("GetPlayerAttribute")]
    public static float GetPlayerAttribute(string attributeID)
    {
        var characterManager = GameManager.Instance.GetSystem<CharacterManager>();
        var playerCharacter = characterManager.GetPlayerCharacter();
        return playerCharacter.Traits().GetAttribute(attributeID);
    }

    [YarnCommand("PlayerAttributeAdd")]
    public static void PlayerAttributeAdd(string attributeID, float value)
    {
        var characterManager = GameManager.Instance.GetSystem<CharacterManager>();
        var playerCharacter = characterManager.GetPlayerCharacter();
        playerCharacter.Traits().AttributeAdd(attributeID, value);
    }

    [YarnCommand("PlayerAttributeSubtract")]
    public static void PlayerAttributeSubtract(string attributeID, float value)
    {
        var characterManager = GameManager.Instance.GetSystem<CharacterManager>();
        var playerCharacter = characterManager.GetPlayerCharacter();
        playerCharacter.Traits().AttributeSubtract(attributeID, value);
    }



    // -------------------
    // Player -> Character
    // -------------------

    [YarnCommand("AddPlayerToCharacterTopic")]
    public static void AddToPlayerTopicToAskCharacter(string characterToAsk, string resourcePathToTopic)
    {
        TopicPlayerToCharacter topic = Resources.Load<TopicPlayerToCharacter>("TopicPlayerToCharacter/" + resourcePathToTopic);

        if (topic == null)
        {
            Debug.LogWarning($"Cannot add topic {resourcePathToTopic} to {characterToAsk}");
            return;
        }

        CharacterBrain characterBrain = GameManager.Instance.GetSystem<CharacterManager>().GetCharacter(characterToAsk);
        characterBrain.Dialogue().AddPlayerToCharacterTopic(topic);
    }

    [YarnCommand("RemovePlayerToCharacterTopic")]
    public static void RemovePlayerTopicToAskCharacter(string characterToAsk, string topicName)
    {
        CharacterBrain characterBrain = GameManager.Instance.GetSystem<CharacterManager>().GetCharacter(characterToAsk);
        characterBrain.Dialogue().RemovePlayerToCharacterTopic(topicName);
    }

    // -------------------
    // Character -> Player
    // -------------------

    [YarnCommand("AddCharacterToPlayerTopic")]
    public static void AddToCharacterTopicToAskPlayer(string characterName, string resourcePathToTopic)
    {
        TopicCharacterToPlayer topic = Resources.Load<TopicCharacterToPlayer>("Dialogues/" + resourcePathToTopic);
        CharacterBrain characterBrain = GameManager.Instance.GetSystem<CharacterManager>().GetCharacter(characterName);
        characterBrain.Dialogue().AddCharacterToPlayerTopic(topic);
    }

    [YarnCommand("RemoveCharacterToPlayerTopic")]
    public static void RemoveCharacterTopicToAskPlayer(string characterName, string topicName)
    {
        CharacterBrain characterBrain = GameManager.Instance.GetSystem<CharacterManager>().GetCharacter(characterName);
        characterBrain.Dialogue().RemoveCharacterToPlayerTopic(topicName);
    }

    // -------------------
    // Character Monologue
    // -------------------

    [YarnCommand("AddToCharacterMonologueTopic")]
    public static void AddToCharacterMonologueTopic(string characterName, string resourcePathToTopic)
    {
        TopicCharacterMonologue topic = Resources.Load<TopicCharacterMonologue>("Dialogues/" + resourcePathToTopic);
        CharacterBrain characterBrain = GameManager.Instance.GetSystem<CharacterManager>().GetCharacter(characterName);
        characterBrain.Dialogue().AddCharacterMonologueTopic(topic);
    }

    [YarnCommand("RemoveCharacterMonologueTopic")]
    public static void RemoveCharacterMonologueTopic(string characterName, string topicName)
    {
        CharacterBrain characterBrain = GameManager.Instance.GetSystem<CharacterManager>().GetCharacter(characterName);
        characterBrain.Dialogue().RemoveCharacterMonologueTopic(topicName);
    }

    // -------------------
    // Player Monologue
    // -------------------

    //[YarnCommand("AddToPlayerMonologueTopic")]
    //public static void AddToPlayerMonologueTopic(string characterName, string resourcePathToTopic)
    //{
    //    TopicPlayerMonologue topic = Resources.Load<TopicPlayerMonologue>("Dialogues/" + resourcePathToTopic);
    //    CharacterBrain characterBrain = GameManager.Instance.GetSystem<CharacterManager>().GetCharacter(characterName);
    //    characterBrain.Dialogue().AddPlayerMonologueTopic(topic);
    //}

    //[YarnCommand("RemovePlayerMonologueTopic")]
    //public static void RemovePlayerMonologueTopic(string characterName, string topicName)
    //{
    //    CharacterBrain characterBrain = GameManager.Instance.GetSystem<CharacterManager>().GetCharacter(characterName);
    //    characterBrain.Dialogue().RemovePlayerMonologueTopic(topicName);
    //}

    // -------------------
    // Character -> Character
    // -------------------

    [YarnCommand("AddCharacterToCharacterTopic")]
    public static void AddToCharacterTopicToAskCharacter(string characterName, string resourcePathToTopic)
    {
        TopicCharacterToCharacter topic = Resources.Load<TopicCharacterToCharacter>("Dialogues/" + resourcePathToTopic);
        CharacterBrain characterBrain = GameManager.Instance.GetSystem<CharacterManager>().GetCharacter(characterName);
        characterBrain.Dialogue().AddCharacterToCharacterTopic(topic);
    }

    [YarnCommand("RemoveCharacterToCharacterTopic")]
    public static void RemoveCharacterTopicToAskCharacter(string characterName, string topicName)
    {
        CharacterBrain characterBrain = GameManager.Instance.GetSystem<CharacterManager>().GetCharacter(characterName);
        characterBrain.Dialogue().RemoveCharacterToCharacterTopic(topicName);
    }


    



    [YarnCommand("logDayEntry")]
    public static void LogDayEntry(string entryMessage)
    {
        DayLogger.Instance.LogDayEntry(entryMessage);
    }


    // Background Dialogue (Thought Management)
    [YarnCommand("addThought")]
    public static void AddThought(string actorName, string thoughtName, int maxVariants)
    {
        var actorRegistry = GameManager.Instance.GetSystem<ActorRegistry>();
        // Find the actor by name
        Actor actor = actorRegistry.GetActorByName(actorName);
        if (actor == null)
        {
            Debug.LogWarningFormat("Cannot find actor named {0}!", actorName);
            return;
        }

        //actor.AddThought(thoughtName, maxVariants);
    }




    // Camera Control

    [YarnCommand("setLongShot")]
    public static void SetLongShot()
    {
        GameManager.Instance.GetSystem<CinemachineManager>().SetLongShot();
    }

    [YarnCommand("setCloseUpShot")]
    public static void SetCloseUpShot()
    {
        GameManager.Instance.GetSystem<CinemachineManager>().SetCloseUpShot();
    }

    [YarnCommand("addActorToShot")]
    public static void AddActorToShot(string actorName)
    {
        // Find the actor by name
        var actorRegistry = GameManager.Instance.GetSystem<ActorRegistry>();
        Actor actor = actorRegistry.GetActorByName(actorName);
        if (actor == null)
        {
            Debug.LogWarningFormat("Cannot find actor named {0}!", actorName);
            return;
        }

        // Add the target to the camera manager
        GameManager.Instance.GetSystem<CinemachineManager>().targetGroup.AddMember(actor.transform, 1f, 0.5f);

        //GameManager.Instance.GetSystem<CinemachineManager>().cameraTarget.AddTarget(actor.transform);
    }

    [YarnCommand("removeActorFromShot")]
    public static void RemoveActorFromShot(string actorName)
    {
        // Find the actor by name
        Actor actor = GameManager.Instance.GetSystem<ActorRegistry>().GetActorByName(actorName);
        if (actor == null)
        {
            Debug.LogWarningFormat("Cannot find actor named {0}!", actorName);
            return;
        }

        GameManager.Instance.GetSystem<CinemachineManager>().targetGroup.RemoveMember(actor.transform);


        // Add the target to the camera manager
        //GameManager.Instance.GetSystem<CinemachineManager>().cameraTarget.RemoveTarget(actor.transform);
    }





    // Quest Management

    [YarnCommand("activateQuest")]
    public static void ActivateQuest(string questName)
    {
        GameManager.Instance.GetSystem<QuestManager>().ActivateQuest(questName);
    }

    [YarnCommand("trackQuest")]
    public static void TrackQuest(string questName)
    {
        GameManager.Instance.GetSystem<QuestManager>().TrackQuest(questName);
    }

    [YarnCommand("completeTask")]
    public static void CompleteTask(string questName, string taskName)
    {
        GameManager.Instance.GetSystem<QuestManager>().CompleteTask(questName, taskName);
    }

    [YarnCommand("updateTaskProgressBy")]
    public static void UpdateTaskProgressBy(string questName, string taskName, float progress)
    {
        GameManager.Instance.GetSystem<QuestManager>().UpdateTaskProgressBy(questName, taskName, progress);
    }

    // Actor Expressions

    [YarnCommand("gesture")]
    public static void Gesture(string actorName, string emoteName)
    {
        // Find the actor by name
        Actor actor = GameManager.Instance.GetSystem<ActorRegistry>().GetActorByName(actorName);
        if (actor == null)
        {
            Debug.LogWarningFormat("Cannot find actor named {0}!", actorName);
            return;
        }

        // Trigger the emote on the actor
        //actor.Gesture(emoteName);
    }

    [YarnCommand("enterState")]
    public static void EnterState(string actorName, string stateName)
    {
        // Find the actor by name
        Actor actor = GameManager.Instance.GetSystem<ActorRegistry>().GetActorByName(actorName);
        if (actor == null)
        {
            Debug.LogWarningFormat("Cannot find actor named {0}!", actorName);
            return;
        }

        // Enter the specified state on the actor
        //actor.EnterState(stateName);
    }

    [YarnCommand("exitState")]
    public static void ExitState(string actorName, string stateName)
    {
        // Find the actor by name
        Actor actor = GameManager.Instance.GetSystem<ActorRegistry>().GetActorByName(actorName);
        if (actor == null)
        {
            Debug.LogWarningFormat("Cannot find actor named {0}!", actorName);
            return;
        }
        // Exit the specified state on the actor
        //actor.ExitState(stateName);
    }
}
