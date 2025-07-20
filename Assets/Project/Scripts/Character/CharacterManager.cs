using System.Drawing;
using System.Numerics;
using UnityEngine;

public class CharacterManager : Singleton<CharacterManager>
{
    [SerializeField] private CharacterEntry[] characterEntries;

    /*
    Vector<CharacterBrain> characterBrains;
    Vector<Character> characters
    
    void AddCharacterToScene()
    {

    }

    void RemoveCharacterFromScene()
    {

    }


    void Start()
    {

    }

    */

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetCharacterPrefab(string spawnedActorName)
    {
        foreach (CharacterEntry characterEntry in characterEntries)
        {
            string actorName = characterEntry.CharacterPrefab.GetComponent<Actor>().actorName;
            if (actorName == spawnedActorName)
            {
                return characterEntry.CharacterPrefab;
            }   
        }

        return null;
    }
        

    void DespawnCharacters()
    {

    }
    
}
