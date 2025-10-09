using GameCreator.Runtime.Quests;
using System;
using UnityEngine;


[Serializable]
public class Deadline
{
    [SerializeField] public string Time; // Expected format "HH:mm"
    [SerializeField] public WEEKDAY WeekDay;

    public bool IsPast()
    {
        if (IsDayPast()) return true;
        if (IsTimePast()) return true;
        return false;
    }

    private bool IsDayPast()
    {
        if (WeekDay < GameManager.Instance.GetSystem<TimeManager>().GetWeekDay())
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private bool IsTimePast()
    {
        // Convert Time to a float representing the fraction of the day
        string[] parts = Time.Split(':');
        if (parts.Length != 2) return false;
        if (int.TryParse(parts[0], out int hours) && int.TryParse(parts[1], out int minutes))
        {
            hours = Mathf.Clamp(hours, 0, 23);
            minutes = Mathf.Clamp(minutes, 0, 59);
            float deadlineTime = (hours + (minutes / 60f)) / 24f;
            // Compare with current time
            return GameManager.Instance.GetSystem<TimeManager>().GetTimePercentage() > deadlineTime;
        }
        else
        {
            Debug.LogWarning("Invalid time format for deadline: " + Time);
            return false;
        }
    }
}

public class TaskDeadline : MonoBehaviour
{
    [SerializeField] public PickTask m_Task = new PickTask();
    [SerializeField] public Deadline deadline = new Deadline();

    internal void Initialize(PickTask setTask, Deadline setDeadline)
    {
        m_Task = setTask;
        deadline = setDeadline;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GetSystem<QuestManager>().journal.IsTaskInactive(m_Task.Quest, m_Task.TaskId) ||
            GameManager.Instance.GetSystem<QuestManager>().journal.IsQuestInactive(m_Task.Quest) || 
            GameManager.Instance.GetSystem<QuestManager>().journal.IsTaskCompleted(m_Task.Quest, m_Task.TaskId) ||
            GameManager.Instance.GetSystem<QuestManager>().journal.IsQuestCompleted(m_Task.Quest) ||
            GameManager.Instance.GetSystem<QuestManager>().journal.IsTaskAbandoned(m_Task.Quest, m_Task.TaskId) ||
            GameManager.Instance.GetSystem<QuestManager>().journal.IsQuestAbandoned(m_Task.Quest) ||
            GameManager.Instance.GetSystem<QuestManager>().journal.IsTaskFailed(m_Task.Quest, m_Task.TaskId) || 
            GameManager.Instance.GetSystem<QuestManager>().journal.IsQuestFailed(m_Task.Quest)
            )
        {
            Destroy(this);
            return;
        }

        if(deadline.IsPast())
        {
            _ = GameManager.Instance.GetSystem<QuestManager>().journal.FailTask(m_Task.Quest, m_Task.TaskId);   
        }
    }
}
