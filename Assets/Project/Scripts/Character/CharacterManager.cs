using GameCreator.Runtime.Common;
using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterManager : GameSystem
{
    private PlayerCharacterBrain playerCharacter;
    private CharacterBrain[] characters;


    public override void Init()
    {
        characters = GetComponentsInChildren<CharacterBrain>();
        playerCharacter = GetComponentInChildren<PlayerCharacterBrain>();
    }

    public override void Shutdown()
    {
    
    }

    public PlayerCharacterBrain GetPlayerCharacter()
    {
        return playerCharacter;
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


    //WE STOPPED HERE, SPAWNING CHACHARCTER.

    public void SpawnCharacter(CharacterBrain character, TimeEntry entry)
    {
        Marker marker =

        Character character = target.Get<Character>();


        Instantiate(character.Prefab(), entry.TargetLocation, Quaternion.identity);
    }


    public void AddTopicPlayerToCharacter(TopicPlayerToCharacter topic, List<string> characterNames)
    {
        foreach (string characterName in characterNames)
        {
            CharacterBrain character = GetCharacter(characterName);
            if (character != null)
            {
                character.Dialogue().AddPlayerToCharacterTopic(topic);
            }
            else
            {
                Debug.LogWarning($"Character '{characterName}' not found. Cannot add topic.");
            }
        }
    }

    public void RemoveTopicPlayerToCharacter(TopicPlayerToCharacter topic, List<string> characterNames)
    {
        foreach (string characterName in characterNames)
        {
            CharacterBrain character = GetCharacter(characterName);
            if (character != null)
            {
                character.Dialogue().RemovePlayerToCharacterTopic(topic);
            }
            else
            {
                Debug.LogWarning($"Character '{characterName}' not found. Cannot remove topic.");
            }
        }
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (CharacterBrain character in characters)
        {
            TimeEntry currentEntry = character.Schedule().FindCurrentEntry();
            if(currentEntry == null)
            {
                continue;
            }

            if(currentEntry.sceneName == scene.name)
            {
                Debug.Log($"Spawning {character.name}");
            }
        }
    }
}
