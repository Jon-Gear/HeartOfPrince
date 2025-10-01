using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

// Wrapper class for DialogueRunner. Must be on the same GameObject as the DialogueRunner component.
public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private DialogueRunner mainDialogueRunner;
    [SerializeField] private DialogueRunner backgroundDialogueRunner;
    

    public bool IsDialogueRunning()
    {
        return mainDialogueRunner.IsDialogueRunning;
    }

    public void StartDialogue(string startNodeName)
    {
        if (IsInnerMonologueRunning())
        {
            StopInnerMonologue();
        }
        if(IsBackgroundDialogueRunning())
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

    public bool IsInnerMonologueRunning()
    {
        return mainDialogueRunner.IsDialogueRunning;
    }
    public void StartInnerMonologue(string startNodeName)
    {
        mainDialogueRunner.StartDialogue(startNodeName);
    }

    public void StopInnerMonologue()
    {
        mainDialogueRunner.Stop();
    }

    protected override void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        StopDialogue();
        StopInnerMonologue();
        StopBackgroundDialogue();
    }
}
