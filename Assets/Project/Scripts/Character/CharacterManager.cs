using GameCreator.Runtime.Common;
using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterManager : GameSystem
{
    private CharacterBrain[] characters;
    [SerializeField] private Actor playerActor;
    [SerializeField] private List<Actor> actors = new List<Actor>();
    
    public override void Init()
    {
        characters = GetComponentsInChildren<CharacterBrain>();
    }

    public override void Shutdown()
    {
    
    }


    public CharacterBrain GetCharacter(string characterName)
    {
        foreach (CharacterBrain character in characters)
        {
            if (character.name == characterName)
            {
                return character;
            }
        }

        return null;
    }

    public void RegisterPlayerActor(Actor actor)
    {
        if(playerActor != null)
        {
            Debug.LogError("Actor Registry Error: There cannot be two player actors");
            return;
        }
        playerActor = actor;
    }
    public Actor GetPlayerActor()
    {
        return playerActor;
    }

    public void UnregisterPlayerActor()
    {
        playerActor = null;
    }


    public void RegisterActor(Actor actor)
    {
        if (!actors.Contains(actor))
        {
            actors.Add(actor);
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

    public void UnregisterActor(Actor actor)
    {
        if (actors.Contains(actor))
        {
            actors.Remove(actor);
        }
    }



    protected override void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        playerActor = null;
        actors.Clear();
    }
}
