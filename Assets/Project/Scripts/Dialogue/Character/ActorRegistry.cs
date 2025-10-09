using GameCreator.Runtime.Dialogue;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActorRegistry : Singleton<ActorRegistry>
{
    public List<Actor> actors = new List<Actor>();
    
    public Actor playerActor;

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

    protected override void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        playerActor = null;
        actors.Clear();
    }
}
