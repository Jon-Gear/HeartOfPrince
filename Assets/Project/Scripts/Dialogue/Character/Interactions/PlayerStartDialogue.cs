using GameCreator.Runtime.Dialogue;
using GameCreator.Runtime.Stats;
using UnityEngine;

public class PlayerStartDialogue : MonoBehaviour
{
    [SerializeField] private Actor characterActor;
    public void StartDialogue()
    {
        var dialogueManager = GameManager.Instance.GetSystem<DialogueManager>();
        dialogueManager.Primary().StartDialogue("Start");
    }
}
