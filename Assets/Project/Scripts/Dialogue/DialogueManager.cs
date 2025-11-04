using GameCreator.Runtime.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Yarn.Unity;
using static Unity.Collections.Unicode;

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
        var actorRegistry = GameManager.Instance.GetSystem<ActorRegistry>();

        if (string.IsNullOrEmpty(actorName))
        {
            Debug.LogWarning("DialogueManager: Actor name is null or empty. Defaulting to player actor.");
            currentSpeaker = actorRegistry.playerActor;
            return;
        }
        Actor actor = actorRegistry.GetActorByName(actorName);
        if (actor == null)
        {
            Debug.LogWarning($"DialogueManager: Actor with name '{actorName}' not found. Defaulting to player actor.");
            currentSpeaker = actorRegistry.playerActor;
            return;
        }
        currentSpeaker = actor;
    }

    private void OnNodeStart(string nodeName)
    {
        /*
        var actorRegistry = GameManager.Instance.GetSystem<ActorRegistry>();


        IEnumerable<string> tags = dialogueRunner.GetTagsForNode(nodeName);
        foreach (var tag in tags)
        {
            if (tag.StartsWith("actor:"))
            {
                string actorName = tag.Substring("actor:".Length).Trim();
                Actor actor = actorRegistry.GetActorByName(actorName);
                if (actor == null) actor = actorRegistry.playerActor;
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

        var timeManager = GameManager.Instance.GetSystem<TimeManager>();
        timeManager.StopTime();
        */
    }

    public void RunLine()
    {
        var timeManager = GameManager.Instance.GetSystem<TimeManager>();
        timeManager.AdvanceByMinutes(1.0f);
    }

    private void OnNodeComplete(string nodeName)
    {
        /*
        var actorRegistry = GameManager.Instance.GetSystem<ActorRegistry>();

        int energy_cost = 0;

        IEnumerable<string> tags = dialogueRunner.GetTagsForNode(nodeName);
        foreach (var tag in tags)
        {
            if (tag.StartsWith("actor:"))
            {
                string actorName = tag.Substring("actor:".Length).Trim();
                Actor actor = actorRegistry.GetActorByName(actorName);
                if (actor == null) actor = actorRegistry.playerActor;
                activeActors.Add(actor);
                if (actor.Brain() != null && actor.Brain().Dialogue().CurrentIntention == DialogueIntention.None)
                {
                    actor.Brain().Dialogue().SetIntention(DialogueIntention.SpokenTo);
                }
            }

            if(tag.StartsWith("energy_cost:"))
            {
                energy_cost = int.Parse(tag.Substring("energy_cost:".Length).Trim());
            }
        }


        currentSpeaker = null;
        foreach (var actor in activeActors)
        {
            if (actor?.Brain() != null)
            {
                actor.Brain().Dialogue().ClearIntention();
            }
        }
        activeActors.Clear();

        var playerCharacter = GameManager.Instance.GetSystem<CharacterManager>().GetPlayerCharacter();

        playerCharacter.Traits().AttributeSubtract("attribute-energy", energy_cost);
        var timeManager = GameManager.Instance.GetSystem<TimeManager>();
        timeManager.StartTime();
        */

    }
}



public struct DialogueVariables
{
    string currentActor;

}


public class DialogueManager : GameSystem 
{
    [SerializeField] private DialogueRunner primary;

    private DialogueVariables currentDialogueVariables = new DialogueVariables();


    public override void Init()
    {
    }

    public DialogueRunner Primary() => primary;

    public override void Shutdown()
    {
    }



    [YarnFunction("current_actor")]
    public static string CurrentActor()
    {
        return "Munir";
        //return GameManager.Instance.GetSystem<DialogueManager>().currentActor?.actorName;
    }
}
