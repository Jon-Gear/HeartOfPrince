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
        // Find the actor by name
        Actor actor = ActorRegistry.Instance.GetActorByName(actorName);
        if (actor == null)
        {
            Debug.LogWarningFormat("Cannot find actor named {0}!", actorName);
            return;
        }
        // Find the target by name
        Actor target = ActorRegistry.Instance.GetActorByName(targetName);
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
        // Find the actor by name
        Actor actor = ActorRegistry.Instance.GetActorByName(actorName);
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
        CharacterDialogueBrain characaterDialogueBrain = CharacterManager.Instance.GetCharacter(characterName);
        return characaterDialogueBrain.GetDialogueTopicOptionText(index);
    }

    [YarnFunction("GetDialogueTopicNodeName")]
    public static string GetDialogueTopicNodeName(string characterName, int index)
    {
        CharacterDialogueBrain characaterDialogueBrain = CharacterManager.Instance.GetCharacter(characterName);
        return characaterDialogueBrain.GetDialogueTopicNodeName(index);
    }

    // Adding Dialogue Topics to Characters

    [YarnCommand("AddToCharacterTopicToAskPlayer")]
    public static void AddToCharacterTopicToAskPlayer(string characterName, string resourcePathToTopic)
    {
        DialogueTopicFromCharacter topic = Resources.Load<DialogueTopicFromCharacter>("Dialogues/" + resourcePathToTopic);
        CharacterDialogueBrain characaterDialogueBrain = CharacterManager.Instance.GetCharacter(characterName);
        characaterDialogueBrain.AddDialogueTopicFromCharacterToPlayer(topic);
    }

    [YarnCommand("AddToPlayerTopicToAskCharacter")]
    public static void AddToPlayerTopicToAskCharacter(string characterName, string resourcePathToTopic)
    {
        DialogueTopicFromPlayer topic = Resources.Load<DialogueTopicFromPlayer>("Dialogues/" + resourcePathToTopic);
        CharacterDialogueBrain characaterDialogueBrain = CharacterManager.Instance.GetCharacter(characterName);
        characaterDialogueBrain.AddDialogueTopicFromPlayerToCharacter(topic);
    }

    [YarnCommand("AddToPlayerMonologueTopic")]
    public static void AddToPlayerMonologueTopic(string characterName, string resourcePathToTopic)
    {
        /*
        DialogueTopicFromPlayer topic = Resources.Load<DialogueTopicFromPlayer>("Dialogues/" + resourcePathToTopic);
        CharacterDialogueBrain characaterDialogueBrain = CharacterManager.Instance.GetCharacter(characterName);
        characaterDialogueBrain.(topic);
        */
    }

    [YarnCommand("AddToCharacterMonologueTopic")]
    public static void AddToCharacterMonologueTopic(string characterName, string resourcePathToTopic)
    {
        MonologueTopicFromCharacter topic = Resources.Load<MonologueTopicFromCharacter>("Dialogues/" + resourcePathToTopic);
        CharacterDialogueBrain characaterDialogueBrain = CharacterManager.Instance.GetCharacter(characterName);
        characaterDialogueBrain.AddMonologueTopic(topic);
    }

    [YarnCommand("AddToCharacterTopicToAskCharacter")]
    public static void AddToCharacterTopicToAskCharacter(string characterName, string resourcePathToTopic)
    {
        BackgroundDialogueTopic topic = Resources.Load<BackgroundDialogueTopic>("Dialogues/" + resourcePathToTopic);
        CharacterDialogueBrain characaterDialogueBrain = CharacterManager.Instance.GetCharacter(characterName);
        characaterDialogueBrain.AddDialogueTopicFromCharacterToCharacter(topic);
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
        // Find the actor by name
        Actor actor = ActorRegistry.Instance.GetActorByName(actorName);
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
        CinemachineManager.Instance.SetLongShot();
    }

    [YarnCommand("setCloseUpShot")]
    public static void SetCloseUpShot()
    {
        CinemachineManager.Instance.SetCloseUpShot();
    }

    [YarnCommand("addActorToShot")]
    public static void AddActorToShot(string actorName)
    {
        // Find the actor by name
        Actor actor = ActorRegistry.Instance.GetActorByName(actorName);
        if (actor == null)
        {
            Debug.LogWarningFormat("Cannot find actor named {0}!", actorName);
            return;
        }

        // Add the target to the camera manager
        CinemachineManager.Instance.targetGroup.AddMember(actor.transform, 1f, 0.5f);

        //CinemachineManager.Instance.cameraTarget.AddTarget(actor.transform);
    }

    [YarnCommand("removeActorFromShot")]
    public static void RemoveActorFromShot(string actorName)
    {
        // Find the actor by name
        Actor actor = ActorRegistry.Instance.GetActorByName(actorName);
        if (actor == null)
        {
            Debug.LogWarningFormat("Cannot find actor named {0}!", actorName);
            return;
        }

        CinemachineManager.Instance.targetGroup.RemoveMember(actor.transform);


        // Add the target to the camera manager
        //CinemachineManager.Instance.cameraTarget.RemoveTarget(actor.transform);
    }





    // Quest Management

    [YarnCommand("activateQuest")]
    public static void ActivateQuest(string questName)
    {
        QuestManager.Instance.ActivateQuest(questName);
    }

    [YarnCommand("trackQuest")]
    public static void TrackQuest(string questName)
    {
        QuestManager.Instance.TrackQuest(questName);
    }

    [YarnCommand("completeTask")]
    public static void CompleteTask(string questName, string taskName)
    {
        QuestManager.Instance.CompleteTask(questName, taskName);
    }

    [YarnCommand("updateTaskProgressBy")]
    public static void UpdateTaskProgressBy(string questName, string taskName, float progress)
    {
        QuestManager.Instance.UpdateTaskProgressBy(questName, taskName, progress);
    }

    // Actor Expressions

    [YarnCommand("gesture")]
    public static void Gesture(string actorName, string emoteName)
    {
        // Find the actor by name
        Actor actor = ActorRegistry.Instance.GetActorByName(actorName);
        if (actor == null)
        {
            Debug.LogWarningFormat("Cannot find actor named {0}!", actorName);
            return;
        }

        // Trigger the emote on the actor
        actor.Gesture(emoteName);
    }

    [YarnCommand("enterState")]
    public static void EnterState(string actorName, string stateName)
    {
        // Find the actor by name
        Actor actor = ActorRegistry.Instance.GetActorByName(actorName);
        if (actor == null)
        {
            Debug.LogWarningFormat("Cannot find actor named {0}!", actorName);
            return;
        }

        // Enter the specified state on the actor
        actor.EnterState(stateName);
    }

    [YarnCommand("exitState")]
    public static void ExitState(string actorName, string stateName)
    {
        // Find the actor by name
        Actor actor = ActorRegistry.Instance.GetActorByName(actorName);
        if (actor == null)
        {
            Debug.LogWarningFormat("Cannot find actor named {0}!", actorName);
            return;
        }
        // Exit the specified state on the actor
        actor.ExitState(stateName);
    }
}
