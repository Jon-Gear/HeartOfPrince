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


    public CharacterDialogueBrain GetCharacter(string characterName)
    {
        CharacterDialogueBrain[] characters = FindObjectsByType<CharacterDialogueBrain>(FindObjectsSortMode.None);
        
        foreach (CharacterDialogueBrain character in characters)
        {
            if (character.name == characterName)
            {
                return character;
            }
        }

        Debug.LogError($"Character '{characterName}' not found in the scene.");
        return null;
    }
}
