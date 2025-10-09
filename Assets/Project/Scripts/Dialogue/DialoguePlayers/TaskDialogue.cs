using GameCreator.Runtime.Quests;
using UnityEngine;

public class TaskDialogue : TDialogue
{
    [Header("Task Dialogue Settings")]
    [SerializeField] private Actor actor;
    public int taskID = 1;

    public override void Talk()
    {
        if (GameManager.Instance.GetSystem<DialogueManager>().main.IsRunning())
        {
            return;
        }

        string nodeName = GetDialogueNode();
        GameManager.Instance.GetSystem<DialogueManager>().StartDialogue(nodeName);
    }

    string GetDialogueNode()
    {
        State questState = GameManager.Instance.GetSystem<QuestManager>().GetQuestState(GetTaskPath());

        switch (questState)
        {
            case State.Inactive:
                return $"task_{actor.actorName.ToLower()}_{taskID}_inactive";

            case State.Active:
                return $"task_{actor.actorName.ToLower()}_{taskID}_active";
                
            case State.Completed:
                return $"task_{actor.actorName.ToLower()}_{taskID}_completed";
                
            case State.Abandoned:
                return $"task_{actor.actorName.ToLower()}_{taskID}_abandoned";
                
            case State.Failed:
                return $"task_{actor.actorName.ToLower()}_{taskID}_failed";
                
            default:
                Debug.Log("Error: TaskDialogue reached default value");
                return $"task_{actor.actorName.ToLower()}_{taskID}_inactive";
        }

    }

    string GetTaskPath()
    {
        return $"{actor.actorName}/Tasks/{actor.actorName}Task{taskID}";
    }

}
