using UnityEngine;
using Yarn.Unity;

// Wrapper class for DialogueRunner. Must be on the same GameObject as the DialogueRunner component.
public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private DialogueRunner mainDialogueRunner;
    [SerializeField] private DialogueRunner backgroundDialogueRunner;
    private void Awake()
    {
    }

    public bool IsDialogueRunning()
    {
        return mainDialogueRunner.IsDialogueRunning;
    }

    public void StartDialogue(string startNodeName)
    {
        if (IsBackgroundDialogueRunning())
        {
            StopBackgroundDialogue();
        }
        mainDialogueRunner.StartDialogue(startNodeName);
    }

    public void StopDialogue()
    {
        mainDialogueRunner.Stop();
    }

    public bool IsBackgroundDialogueRunning()
    {
        return backgroundDialogueRunner.IsDialogueRunning;
    }
    public void StartBackgroundDialogue(string startNodeName)
    {
        backgroundDialogueRunner.StartDialogue(startNodeName);
    }

    public void StopBackgroundDialogue()
    {
        backgroundDialogueRunner.Stop();
    }
}
