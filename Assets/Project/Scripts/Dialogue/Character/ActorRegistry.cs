using GameCreator.Runtime.Dialogue;
using System.Collections.Generic;
using UnityEngine;

public class ActorRegistry : Singleton<ActorRegistry>
{
    public List<Actor> actors = new List<Actor>();
    
    public Actor playerActor;
    public Actor currentSpeaker;
    
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

    public void SetCurrentSpeaker(string actorName)
    {
        if (string.IsNullOrEmpty(actorName))
        {
            currentSpeaker = playerActor;
            return;
        }

        Actor actor = GetActorByName(actorName);
        if (actor != null)
        {
            currentSpeaker = actor;
        }
        else
        {
            currentSpeaker = playerActor;
        }
    }
}
