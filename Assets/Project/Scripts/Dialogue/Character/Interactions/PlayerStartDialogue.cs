using GameCreator.Runtime.Dialogue;
using GameCreator.Runtime.Stats;
using UnityEngine;

public class PlayerStartDialogue : MonoBehaviour
{
    [SerializeField] private Actor characterActor;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void StartDialogue()
    {
        characterActor.Brain().Dialogue().TriggerPlayerDialogueWithCharacter();
    }
}
