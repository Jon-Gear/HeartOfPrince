using GameCreator.Runtime.Quests;
using UnityEngine;

public class TaskConditionalObject : MonoBehaviour
{
    [SerializeField] private PickTask m_Task = new PickTask();

    [SerializeField] private GameObject GameObjectInactiveState;
    [SerializeField] private GameObject GameObjectActiveState;
    [SerializeField] private GameObject GameObjectCompletedState;
    [SerializeField] private GameObject GameObjectAbandonedState;
    [SerializeField] private GameObject GameObjectFailedState;
    [SerializeField] private GameObject GameObjectQuestFinishedState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CheckAndSetState();
        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckAndSetState()
    {
        if (QuestManager.Instance.journal.IsTaskInactive(this.m_Task.Quest, this.m_Task.TaskId))
        {
            SetInactive(m_Task.Quest, m_Task.TaskId);
        }
        else if (QuestManager.Instance.journal.IsTaskActive(this.m_Task.Quest, this.m_Task.TaskId))
        {
            SetActive(m_Task.Quest, m_Task.TaskId);
        }
        else if (QuestManager.Instance.journal.IsQuestCompleted(this.m_Task.Quest))
        {
            SetFinished();
        }
        else if (QuestManager.Instance.journal.IsTaskCompleted(this.m_Task.Quest, this.m_Task.TaskId))
        {
            SetCompleted(m_Task.Quest, m_Task.TaskId);
        }
        else if (QuestManager.Instance.journal.IsTaskAbandoned(this.m_Task.Quest, this.m_Task.TaskId))
        {
            SetAbandoned(m_Task.Quest, m_Task.TaskId);
        }
        else if (QuestManager.Instance.journal.IsTaskFailed(this.m_Task.Quest, this.m_Task.TaskId))
        {
            SetFailed(m_Task.Quest, m_Task.TaskId);
        }
    }




    void SetInactive(Quest quest, int taskId)
    {
        if (this.m_Task.IsNot(quest, taskId)) return;

        this.GameObjectInactiveState.SetActive(true);
        this.GameObjectActiveState.SetActive(false);
        this.GameObjectCompletedState.SetActive(false);
        this.GameObjectAbandonedState.SetActive(false);
        this.GameObjectFailedState.SetActive(false);
    }


    void SetActive(Quest quest, int taskId)
    {
        if (this.m_Task.IsNot(quest, taskId)) return;


        this.GameObjectInactiveState.SetActive(false);
        this.GameObjectActiveState.SetActive(true);
        this.GameObjectCompletedState.SetActive(false);
        this.GameObjectAbandonedState.SetActive(false);
        this.GameObjectFailedState.SetActive(false);
    }

    void SetCompleted(Quest quest, int taskId)
    {
        if (this.m_Task.IsNot(quest, taskId)) return;

        this.GameObjectInactiveState.SetActive(false);
        this.GameObjectActiveState.SetActive(false);
        this.GameObjectCompletedState.SetActive(true);
        this.GameObjectAbandonedState.SetActive(false);
        this.GameObjectFailedState.SetActive(false);
    }

    void SetAbandoned(Quest quest, int taskId)
    {
        if (this.m_Task.IsNot(quest, taskId)) return;

        this.GameObjectInactiveState.SetActive(false);
        this.GameObjectActiveState.SetActive(false);
        this.GameObjectCompletedState.SetActive(false);
        this.GameObjectAbandonedState.SetActive(true);
        this.GameObjectFailedState.SetActive(false);
    }

    void SetFailed(Quest quest, int taskId)
    {
        if (this.m_Task.IsNot(quest, taskId)) return;

        this.GameObjectInactiveState.SetActive(false);
        this.GameObjectActiveState.SetActive(false);
        this.GameObjectCompletedState.SetActive(false);
        this.GameObjectAbandonedState.SetActive(false);
        this.GameObjectFailedState.SetActive(true);
    }

    void SetFinished()
    {
        this.GameObjectInactiveState.SetActive(false);
        this.GameObjectActiveState.SetActive(false);
        this.GameObjectCompletedState.SetActive(false);
        this.GameObjectAbandonedState.SetActive(false);
        this.GameObjectFailedState.SetActive(false);
        this.GameObjectQuestFinishedState.SetActive(true);
    }




    protected void Subscribe()
    {
        QuestManager.Instance.journal.EventTaskDeactivate -= this.SetInactive;
        QuestManager.Instance.journal.EventTaskDeactivate += this.SetInactive;

        QuestManager.Instance.journal.EventTaskActivate -= this.SetActive;
        QuestManager.Instance.journal.EventTaskActivate += this.SetActive;

        QuestManager.Instance.journal.EventTaskComplete -= this.SetCompleted;
        QuestManager.Instance.journal.EventTaskComplete += this.SetCompleted;

        QuestManager.Instance.journal.EventTaskAbandon -= this.SetAbandoned;
        QuestManager.Instance.journal.EventTaskAbandon += this.SetAbandoned;

        QuestManager.Instance.journal.EventTaskFail -= this.SetFailed;
        QuestManager.Instance.journal.EventTaskFail += this.SetFailed;
    }

    protected void Unsubscribe()
    {
        if (QuestManager.IsQuitting) return;

        QuestManager.Instance.journal.EventTaskDeactivate -= this.SetInactive;

        QuestManager.Instance.journal.EventTaskActivate -= this.SetActive;

        QuestManager.Instance.journal.EventTaskComplete -= this.SetCompleted;

        QuestManager.Instance.journal.EventTaskAbandon -= this.SetAbandoned;

        QuestManager.Instance.journal.EventTaskFail -= this.SetFailed;
    }
}
