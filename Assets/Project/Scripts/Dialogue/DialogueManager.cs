using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

[System.Serializable]
public class Dialogue
{
    [SerializeField] public DialogueRunner dialogueRunner;
    [SerializeField] private Actor currentSpeaker;
    [SerializeField] private List<Actor> activeActors = new List<Actor>();

    private Actor dialogueInitiator;

    public void Start()
    {
        if (dialogueRunner == null)
        {
            Debug.LogError("DialogueRunner is not assigned in Dialogue class instance.");
            return;
        }
        dialogueRunner.onNodeStart.AddListener(OnNodeStart);
        dialogueRunner.onNodeComplete.AddListener(OnNodeComplete);
        
    }

    public bool IsRunning()
    {
        return dialogueRunner != null && dialogueRunner.IsDialogueRunning;
    }
    public void Stop()
    {
        if (dialogueRunner != null && dialogueRunner.IsDialogueRunning)
            dialogueRunner.Stop();
        currentSpeaker = null;
        foreach (var actor in activeActors)
            actor?.Brain()?.Dialogue().ClearIntention();
        activeActors.Clear();
    }

    public Actor GetSpeaker() => currentSpeaker;
    public void SetSpeaker(string actorName)
    {
        if (string.IsNullOrEmpty(actorName))
        {
            Debug.LogWarning("DialogueManager: Actor name is null or empty. Defaulting to player actor.");
            currentSpeaker = ActorRegistry.Instance.playerActor;
            return;
        }
        Actor actor = ActorRegistry.Instance.GetActorByName(actorName);
        if (actor == null)
        {
            Debug.LogWarning($"DialogueManager: Actor with name '{actorName}' not found. Defaulting to player actor.");
            currentSpeaker = ActorRegistry.Instance.playerActor;
            return;
        }
        currentSpeaker = actor;
    }

    private void OnNodeStart(string nodeName)
    {
        IEnumerable<string> tags = dialogueRunner.GetTagsForNode(nodeName);
        foreach (var tag in tags)
        {
            if (tag.StartsWith("actor:"))
            {
                string actorName = tag.Substring("actor:".Length).Trim();
                Actor actor = ActorRegistry.Instance.GetActorByName(actorName);
                if (actor == null) actor = ActorRegistry.Instance.playerActor;
                activeActors.Add(actor);
                if (actor.Brain() != null && actor.Brain().Dialogue().CurrentIntention == DialogueIntention.None)
                {
                    actor.Brain().Dialogue().SetIntention(DialogueIntention.SpokenTo);
                    
                }
            }
        }
        if (tags.GetHashCode() == 0)
        {
            // No tags found, default to current speaker if set
            Debug.LogWarning($"No actor tags found in node {nodeName}");
        }
    }

    private void OnNodeComplete(string nodeName)
    {
        currentSpeaker = null;
        foreach (var actor in activeActors)
        {
            if (actor?.Brain() != null)
                actor.Brain().Dialogue().ClearIntention();
        }
        activeActors.Clear();
    }
}



public class DialogueManager : Singleton<DialogueManager>
{
    [Header("Dialogue")]

    [SerializeField] public Dialogue main;
    [SerializeField] public Dialogue background_1;
    [SerializeField] public Dialogue background_2;
    [SerializeField] public Dialogue background_3;


    private void Start()
    {
        main.Start();
        background_1.Start();
        background_2.Start();
        background_3.Start();
    }

    // --- Main Dialogue ---
    public void StartDialogue(string startNodeName)
    {
        StopAllBackgroundDialogue();
        main.dialogueRunner.StartDialogue(startNodeName);
    }


    
    // --- Background Dialogue ---
    public void StartBackgroundDialogue(string startNodeName)
    {
        var runner = GetAvailableBackgroundRunner();
        Debug.Log($"Starting background dialogue '{startNodeName}' on runner: {runner?.dialogueRunner?.name ?? "None"}");
        if (runner != null)
        {
            runner.dialogueRunner.StartDialogue(startNodeName);
        }
        else
        {
            Debug.LogWarning("All background dialogue runners are busy!");
        }
    }

    public bool IsAnyBackgroundDialogueAvailable() => GetAvailableBackgroundRunner() != null;

    private Dialogue GetAvailableBackgroundRunner()
    {
        return !background_1.IsRunning() ? background_1 :
               !background_2.IsRunning() ? background_2 :
               !background_3.IsRunning() ? background_3 : null;
    }

    public void StopAllBackgroundDialogue()
    {
        background_1.Stop();
        background_2.Stop();
        background_3.Stop();
    }

    // --- Cleanup on Scene Change ---
    protected override void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        main.Stop();
        StopAllBackgroundDialogue();
    }

}
