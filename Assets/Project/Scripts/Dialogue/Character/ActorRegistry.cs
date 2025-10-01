using GameCreator.Runtime.Dialogue;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActorRegistry : Singleton<ActorRegistry>
{
    public List<Actor> actors = new List<Actor>();
    
    public Actor playerActor;

    public Actor mainDialogueCurrentSpeaker;
    public Actor backgroundDialogueCurrentSpeaker;

    public void RegisterActor(Actor actor)
    {
        if (!actors.Contains(actor))
        {
            actors.Add(actor);
        }
    }

    public void UnregisterActor(Actor actor)
    {
        if (actors.Contains(actor))
        {
            actors.Remove(actor);
        }
    }

    public Actor GetActorByName(string actorName)
    {
        foreach (var actor in actors)
        {
            if (actor.actorName == actorName)
            {
                return actor;
            }
        }
        Debug.LogWarningFormat("Cannot find actor named {0}!", actorName);
        return null;
    }

    public void SetMainDialogueCurrentSpeaker(string actorName)
    {
        if (string.IsNullOrEmpty(actorName))
        {
            mainDialogueCurrentSpeaker = playerActor;
            return;
        }

        Actor actor = GetActorByName(actorName);
        if (actor != null)
        {
            mainDialogueCurrentSpeaker = actor;
        }
        else
        {
            mainDialogueCurrentSpeaker = playerActor;
        }
    }

    public void SetMainDialogueCurrentSpeaker()
    {
        mainDialogueCurrentSpeaker = playerActor;
    }

    public void SetBackgroundDialogueCurrentSpeaker(string actorName)
    {
        if (string.IsNullOrEmpty(actorName))
        {
            backgroundDialogueCurrentSpeaker = playerActor;
            return;
        }
        Actor actor = GetActorByName(actorName);
        if (actor != null)
        {
            backgroundDialogueCurrentSpeaker = actor;
        }
        else
        {
            backgroundDialogueCurrentSpeaker = playerActor;
        }
    }
    protected override void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        playerActor = null;
        actors.Clear();
    }
}
