using GameCreator.Runtime.Common;
using GameCreator.Runtime.Quests;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    [SerializeField] Journal journal;
    private void Awake()
    {
        journal = FindFirstObjectByType<Journal>();
    }

    public void ActivateQuest(string questName)
    {
        if (journal == null) return;
        Quest quest = Resources.Load<Quest>("Quests/" + questName);
        if (quest == null) return;
        _ = journal.ActivateQuest(quest);
    }

    public void TrackQuest(string questName)
    {
        if (journal == null) return;
        Quest quest = Resources.Load<Quest>("Quests/" + questName);
        if (quest == null) return;
        _ = journal.TrackQuest(quest);
    }

    public void CompleteTask(string questName, string taskName)
    {
        if (journal == null) return;
        Quest quest = Resources.Load<Quest>("Quests/" + questName);
        if (quest == null) return;
        
        int taskId = FindTaskIdByName(quest.Tasks, taskName);
        _ = journal.CompleteTask(quest, taskId);
    }

    public void UpdateTaskProgressBy(string questName, string taskName, float value)
    {
        if (journal == null) return;
        Quest quest = Resources.Load<Quest>("Quests/" + questName);
        if (quest == null) return;
        int taskId = FindTaskIdByName(quest.Tasks, taskName);

        double newValue = journal.GetTaskValue(quest, taskId) + value;

        _ = journal.SetTaskValue(quest, taskId, newValue);
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private static int FindTaskIdByName(TasksTree tasksTree, string name)
    {
        foreach (int rootId in tasksTree.RootIds)
        {
            int resultId = FindTaskIdRecursive(tasksTree, rootId, name);
            if (resultId != TreeNode.INVALID) return resultId;
        }

        return TreeNode.INVALID; // Task not found
    }

    private static int FindTaskIdRecursive(TasksTree tasksTree, int nodeId, string name)
    {
        Task currentTask = tasksTree.Get(nodeId);
        if (currentTask == null) return TreeNode.INVALID;

        // Use either Name or Title.GetText() depending on how you're naming tasks
        if (currentTask.GetName(null) == name)
        {
            return nodeId;
        }

        List<int> children = tasksTree.Children(nodeId);
        foreach (int childId in children)
        {
            int resultId = FindTaskIdRecursive(tasksTree, childId, name);
            if (resultId != TreeNode.INVALID) return resultId;
        }

        return TreeNode.INVALID;
    }
}
