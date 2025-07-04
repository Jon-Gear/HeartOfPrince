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

        actor.AddThought(thoughtName, maxVariants);
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
        CinemachineManager.Instance.cameraTarget.AddTarget(actor.transform);
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

        // Add the target to the camera manager
        CinemachineManager.Instance.cameraTarget.RemoveTarget(actor.transform);
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
