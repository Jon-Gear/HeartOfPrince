using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

[System.Serializable]
public class BackgroundRunner
{
    public DialogueRunner runner;
    public Actor currentSpeaker;
    public List<Actor> activeActors = new List<Actor>();

    public void Initialize(UnityEngine.Events.UnityAction<string> onStart, UnityEngine.Events.UnityAction<string> onComplete)
    {
        if (runner == null) return;
        runner.onNodeStart.AddListener(onStart);
        runner.onNodeComplete.AddListener(onComplete);
    }

    public bool IsRunning => runner != null && runner.IsDialogueRunning;

    public void Stop()
    {
        if (runner != null && runner.IsDialogueRunning)
            runner.Stop();
        currentSpeaker = null;
        foreach (var actor in activeActors)
            actor?.Brain()?.Dialogue().SetFree();
        activeActors.Clear();
    }
}


public class DialogueManager : Singleton<DialogueManager>
{
    [Header("Dialogue Runners")]
    [SerializeField] private DialogueRunner mainDialogueRunner;

    [Header("Background Dialogue Runners")]
    [SerializeField] private BackgroundRunner backgroundRunner_1;
    [SerializeField] private BackgroundRunner backgroundRunner_2;
    [SerializeField] private BackgroundRunner backgroundRunner_3;

    private BackgroundRunner[] backgroundRunners;

    [Header("Main Dialogue Speakers")]
    [SerializeField] public Actor mainDialogueCurrentSpeaker;
    [SerializeField] private List<Actor> mainDialogueActiveActors = new List<Actor>();

    private void Start()
    {
        // Main Dialogue
        mainDialogueRunner.onNodeStart.AddListener(OnMainNodeStart);
        mainDialogueRunner.onNodeComplete.AddListener(OnMainNodeComplete);

        // Background Runners
        backgroundRunners = new[] { backgroundRunner_1, backgroundRunner_2, backgroundRunner_3 };

        foreach (var br in backgroundRunners)
        {
            br.Initialize(OnBackgroundNodeStart, OnBackgroundNodeComplete);
        }
    }

    // --- Main Dialogue ---
    public bool IsDialogueRunning() => mainDialogueRunner.IsDialogueRunning;

    public void StartDialogue(string startNodeName)
    {
        StopInnerMonologue();
        StopAllBackgroundDialogues();
        mainDialogueRunner.StartDialogue(startNodeName);
    }

    public void SetDialogueSpeaker(string actorName) => SetCurrentSpeaker(actorName, ref mainDialogueCurrentSpeaker);
    private void OnMainNodeStart(string nodeName) => ProcessNodeStart(nodeName, mainDialogueActiveActors);
    private void OnMainNodeComplete(string nodeName) => ProcessNodeComplete(mainDialogueActiveActors, ref mainDialogueCurrentSpeaker);

    // --- Background Dialogue ---
    private void OnBackgroundNodeStart(string nodeName)
    {
        var runner = GetBackgroundRunnerFromCaller();
        if (runner != null)
        {
            ProcessNodeStart(nodeName, runner.activeActors);
        }
    }

    private void OnBackgroundNodeComplete(string nodeName)
    {
        var runner = GetBackgroundRunnerFromCaller();
        if (runner != null)
        {
            ProcessNodeComplete(runner.activeActors, ref runner.currentSpeaker);
        }
    }

    private BackgroundRunner GetBackgroundRunnerFromCaller()
    {
        // Identify which background runner called this event
        foreach (var br in backgroundRunners)
        {
            if (br.runner != null && br.runner.IsDialogueRunning)
                return br;
        }
        return null;
    }

    public void StartBackgroundDialogue(string startNodeName)
    {
        var runner = GetAvailableBackgroundRunner();
        Debug.Log($"Starting background dialogue '{startNodeName}' on runner: {runner?.runner?.name ?? "None"}");
        if (runner != null)
        {
            runner.runner.StartDialogue(startNodeName);
        }
        else
        {
            Debug.LogWarning("All background dialogue runners are busy!");
        }
    }

    private BackgroundRunner GetAvailableBackgroundRunner()
    {
        foreach (var br in backgroundRunners)
        {
            if (!br.IsRunning)
                return br;
        }
        return null;
    }

    public void StopAllBackgroundDialogues()
    {
        foreach (var br in backgroundRunners)
            br.Stop();
    }

    public bool IsAnyBackgroundDialogueRunning()
    {
        foreach (var br in backgroundRunners)
            if (br.IsRunning) return true;
        return false;
    }

    public void SetBackgroundDialogueSpeaker_1(string actorName) => SetCurrentSpeaker(actorName, ref backgroundRunner_1.currentSpeaker);
    public void SetBackgroundDialogueSpeaker_2(string actorName) => SetCurrentSpeaker(actorName, ref backgroundRunner_2.currentSpeaker);
    public void SetBackgroundDialogueSpeaker_3(string actorName) => SetCurrentSpeaker(actorName, ref backgroundRunner_3.currentSpeaker);

    public bool IsAnyBackgroundDialogueAvailable() => GetAvailableBackgroundRunner() != null;

    // --- Inner Monologue ---
    public bool IsInnerMonologueRunning() => mainDialogueRunner.IsDialogueRunning;
    public void StartInnerMonologue(string startNodeName) => mainDialogueRunner.StartDialogue(startNodeName);
    public void StopInnerMonologue() => mainDialogueRunner.Stop();

    // --- Node Processing Helper ---

    private void SetCurrentSpeaker(string actorName, ref Actor currentSpeaker)
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
    private void ProcessNodeStart(string nodeName, List<Actor> activeActors)
    {
        IEnumerable<string> tags = mainDialogueRunner.GetTagsForNode(nodeName); // or runner.GetTagsForNode(nodeName)
        foreach (var tag in tags)
        {
            if (tag.StartsWith("actor:"))
            {
                string actorName = tag.Substring("actor:".Length).Trim();
                Actor actor = ActorRegistry.Instance.GetActorByName(actorName);
                if (actor == null) actor = ActorRegistry.Instance.playerActor;

                activeActors.Add(actor);
                
                if (actor.Brain() != null)
                    actor.Brain().Dialogue().SetBusy();
            }
        }

        if(tags.GetHashCode() == 0)
        {
            // No tags found, default to current speaker if set
            Debug.LogWarning($"No actor tags found in node {nodeName}");
        }


    }

    private void ProcessNodeComplete(List<Actor> activeActors, ref Actor currentSpeaker)
    {
        currentSpeaker = null;
        foreach (var actor in activeActors)
        {
            if (actor?.Brain() != null)
                actor.Brain().Dialogue().SetFree();
        }
        activeActors.Clear();
    }

    // --- Cleanup on Scene Change ---
    protected override void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        StopDialogue();
        StopInnerMonologue();
        StopAllBackgroundDialogues();
    }

    public void StopDialogue() => mainDialogueRunner.Stop();
}
