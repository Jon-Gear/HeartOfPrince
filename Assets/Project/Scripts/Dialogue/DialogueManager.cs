using UnityEngine;
using Yarn.Unity;

// Wrapper class for DialogueRunner. Must be on the same GameObject as the DialogueRunner component.
public class DialogueManager : Singleton<DialogueManager>
{
    private DialogueRunner dialogueRunner;
    private void Awake()
    {
        dialogueRunner = GetComponent<Yarn.Unity.DialogueRunner>();
    }

    public bool IsDialogueRunning()
    {
        return dialogueRunner.IsDialogueRunning;
    }

    public void StartDialogue(string startNodeName)
    {
        dialogueRunner.StartDialogue(startNodeName);
    }

    public void StopDialogue()
    {
        dialogueRunner.Stop();
    }
}
