using UnityEngine;
using Yarn.Unity;

public class YarnCommands : MonoBehaviour
{



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
