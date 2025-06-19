using UnityEngine;
using Yarn.Unity;

// Wrapper class for DialogueRunner. Must be on the same GameObject as the DialogueRunner component.
public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private DialogueRunner mainDialogueRunner;
    [SerializeField] private DialogueRunner backgroundDialogueRunner;
    [SerializeField] private DialogueRunner innerMonologueRunner;


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
        if (IsInnerMonologueRunning())
        {
            StopInnerMonologue();
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

    public bool IsInnerMonologueRunning()
    {
        return innerMonologueRunner.IsDialogueRunning;
    }
    public void StartInnerMonologue(string startNodeName)
    {
        if (IsBackgroundDialogueRunning())
        {
            StopBackgroundDialogue();
        }
        innerMonologueRunner.StartDialogue(startNodeName);
    }

    public void StopInnerMonologue()
    {
        innerMonologueRunner.Stop();
    }
}
