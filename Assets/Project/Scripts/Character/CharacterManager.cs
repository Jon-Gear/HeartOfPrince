using GameCreator.Runtime.Common;
using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
}
