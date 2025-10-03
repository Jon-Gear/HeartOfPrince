using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using UnityEngine;

public class CharacterManager : Singleton<CharacterManager>
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public CharacterBrain GetCharacter(string characterName)
    {
        CharacterBrain[] characters = FindObjectsByType<CharacterBrain>(FindObjectsSortMode.None);
        
        foreach (CharacterBrain character in characters)
        {
            if (character.name == characterName)
            {
                return character;
            }
        }

        //Debug.LogError($"Character '{characterName}' not found in the scene.");
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
