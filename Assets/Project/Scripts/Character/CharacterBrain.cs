using GameCreator.Runtime.Characters;
using UnityEngine;


[RequireComponent(typeof(CharacterDialogueBrain))]
public class CharacterBrain : MonoBehaviour
{
    private CharacterDialogueBrain characterDialogueBrain;

    public CharacterDialogueBrain Dialogue() => characterDialogueBrain;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterDialogueBrain = GetComponent<CharacterDialogueBrain>();
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
