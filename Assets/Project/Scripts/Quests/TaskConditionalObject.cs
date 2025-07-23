using GameCreator.Runtime.Quests;
using System;
using UnityEngine;

public class TaskConditionalObject : MonoBehaviour
{
    [SerializeField] private PickTask m_Task = new PickTask();

    [Header("General States")]
    [SerializeField] private GameObject GameObjectInactiveState;
    [SerializeField] private GameObject GameObjectActiveState;

    [Header("Task States")]
    [SerializeField] private GameObject GameObjectTaskCompletedState;
    [SerializeField] private GameObject GameObjectTaskAbandonedState;
    [SerializeField] private GameObject GameObjectTaskFailedState;

    [Header("Quest States")]
    [SerializeField] private GameObject GameObjectQuestCompletedState;
    [SerializeField] private GameObject GameObjectQuestAbandonedState;
    [SerializeField] private GameObject GameObjectQuestFailedState;



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
        // Check Quest State
        if (QuestManager.Instance.journal.IsQuestInactive(this.m_Task.Quest))
        {
            SetInactive(m_Task.Quest, m_Task.TaskId);
            return;
        }
        else if (QuestManager.Instance.journal.IsQuestActive(this.m_Task.Quest))
        {
            if (QuestManager.Instance.journal.IsTaskInactive(this.m_Task.Quest, this.m_Task.TaskId))
            {
                SetInactive(m_Task.Quest, m_Task.TaskId);
            }
            else if (QuestManager.Instance.journal.IsTaskActive(this.m_Task.Quest, this.m_Task.TaskId))
            {
                SetActive(m_Task.Quest, m_Task.TaskId);
            }
            else if (QuestManager.Instance.journal.IsTaskCompleted(this.m_Task.Quest, this.m_Task.TaskId))
            {
                SetTaskCompleted(m_Task.Quest, m_Task.TaskId);
            }
            else if (QuestManager.Instance.journal.IsTaskAbandoned(this.m_Task.Quest, this.m_Task.TaskId))
            {
                SetTaskAbandoned(m_Task.Quest, m_Task.TaskId);
            }
            else if (QuestManager.Instance.journal.IsTaskFailed(this.m_Task.Quest, this.m_Task.TaskId))
            {
                SetTaskFailed(m_Task.Quest, m_Task.TaskId);
            }
        }

        if(QuestManager.Instance.journal.IsQuestCompleted(this.m_Task.Quest))
        {
            SetQuestCompleted(m_Task.Quest);
        }
        else if(QuestManager.Instance.journal.IsQuestAbandoned(this.m_Task.Quest))
        {
            SetQuestAbandoned(m_Task.Quest);
        }
        else if(QuestManager.Instance.journal.IsQuestFailed(this.m_Task.Quest))
        {
            SetQuestFailed(m_Task.Quest);
        }
    }

    
    void SetInactive(Quest quest, int taskId)
    {
        if (this.m_Task.IsNot(quest, taskId)) return;

        this.GameObjectActiveState.SetActive(false);

        this.GameObjectTaskCompletedState.SetActive(false);
        this.GameObjectTaskAbandonedState.SetActive(false);
        this.GameObjectTaskFailedState.SetActive(false);

        this.GameObjectQuestCompletedState.SetActive(false);
        this.GameObjectQuestAbandonedState.SetActive(false);
        this.GameObjectQuestFailedState.SetActive(false);

        this.GameObjectInactiveState.SetActive(true);
    }


    void SetActive(Quest quest, int taskId)
    {
        if (this.m_Task.IsNot(quest, taskId)) return;


        this.GameObjectInactiveState.SetActive(false);

        this.GameObjectTaskCompletedState.SetActive(false);
        this.GameObjectTaskAbandonedState.SetActive(false);
        this.GameObjectTaskFailedState.SetActive(false);

        this.GameObjectQuestCompletedState.SetActive(false);
        this.GameObjectQuestAbandonedState.SetActive(false);
        this.GameObjectQuestFailedState.SetActive(false);

        this.GameObjectActiveState.SetActive(true);
    }

    void SetTaskCompleted(Quest quest, int taskId)
    {
        if (this.m_Task.IsNot(quest, taskId)) return;

        this.GameObjectActiveState.SetActive(false);
        this.GameObjectInactiveState.SetActive(false);

        this.GameObjectTaskAbandonedState.SetActive(false);
        this.GameObjectTaskFailedState.SetActive(false);

        this.GameObjectQuestCompletedState.SetActive(false);
        this.GameObjectQuestAbandonedState.SetActive(false);
        this.GameObjectQuestFailedState.SetActive(false);

        this.GameObjectTaskCompletedState.SetActive(true);

    }

    void SetTaskAbandoned(Quest quest, int taskId)
    {
        if (this.m_Task.IsNot(quest, taskId)) return;

        this.GameObjectActiveState.SetActive(false);
        this.GameObjectInactiveState.SetActive(false);

        this.GameObjectTaskCompletedState.SetActive(false);
        this.GameObjectTaskFailedState.SetActive(false);

        this.GameObjectQuestCompletedState.SetActive(false);
        this.GameObjectQuestAbandonedState.SetActive(false);
        this.GameObjectQuestFailedState.SetActive(false);

        this.GameObjectTaskAbandonedState.SetActive(true);

    }

    void SetTaskFailed(Quest quest, int taskId)
    {
        if (this.m_Task.IsNot(quest, taskId)) return;

        this.GameObjectActiveState.SetActive(false);
        this.GameObjectInactiveState.SetActive(false);

        this.GameObjectTaskCompletedState.SetActive(false);
        this.GameObjectTaskAbandonedState.SetActive(false);
        
        this.GameObjectQuestCompletedState.SetActive(false);
        this.GameObjectQuestAbandonedState.SetActive(false);
        this.GameObjectQuestFailedState.SetActive(false);

        this.GameObjectTaskFailedState.SetActive(true);
    }

    private void SetQuestCompleted(Quest quest)
    {
        if (this.m_Task.Quest != quest) return;

        this.GameObjectActiveState.SetActive(false);
        this.GameObjectInactiveState.SetActive(false);
        this.GameObjectTaskCompletedState.SetActive(false);
        this.GameObjectTaskAbandonedState.SetActive(false);
        this.GameObjectTaskFailedState.SetActive(false);
        this.GameObjectQuestAbandonedState.SetActive(false);
        this.GameObjectQuestFailedState.SetActive(false);
        this.GameObjectQuestCompletedState.SetActive(true);
    }

    private void SetQuestAbandoned(Quest quest)
    {
        if (this.m_Task.Quest != quest) return;
        this.GameObjectActiveState.SetActive(false);
        this.GameObjectInactiveState.SetActive(false);
        this.GameObjectTaskCompletedState.SetActive(false);
        this.GameObjectTaskAbandonedState.SetActive(false);
        this.GameObjectTaskFailedState.SetActive(false);
        this.GameObjectQuestCompletedState.SetActive(false);
        this.GameObjectQuestFailedState.SetActive(false);
        this.GameObjectQuestAbandonedState.SetActive(true);
    }

    private void SetQuestFailed(Quest quest)
    {
        if (this.m_Task.Quest != quest) return;
        this.GameObjectActiveState.SetActive(false);
        this.GameObjectInactiveState.SetActive(false);
        this.GameObjectTaskCompletedState.SetActive(false);
        this.GameObjectTaskAbandonedState.SetActive(false);
        this.GameObjectTaskFailedState.SetActive(false);
        this.GameObjectQuestCompletedState.SetActive(false);
        this.GameObjectQuestAbandonedState.SetActive(false);
        this.GameObjectQuestFailedState.SetActive(true);
    }




    protected void Subscribe()
    {
        // Tasks

        QuestManager.Instance.journal.EventTaskDeactivate -= this.SetInactive;
        QuestManager.Instance.journal.EventTaskDeactivate += this.SetInactive;

        QuestManager.Instance.journal.EventTaskActivate -= this.SetActive;
        QuestManager.Instance.journal.EventTaskActivate += this.SetActive;

        QuestManager.Instance.journal.EventTaskComplete -= this.SetTaskCompleted;
        QuestManager.Instance.journal.EventTaskComplete += this.SetTaskCompleted;

        QuestManager.Instance.journal.EventTaskAbandon -= this.SetTaskAbandoned;
        QuestManager.Instance.journal.EventTaskAbandon += this.SetTaskAbandoned;

        QuestManager.Instance.journal.EventTaskFail -= this.SetTaskFailed;
        QuestManager.Instance.journal.EventTaskFail += this.SetTaskFailed;

        // Quests


    }

    protected void Unsubscribe()
    {
        if (QuestManager.IsQuitting) return;

        QuestManager.Instance.journal.EventTaskDeactivate -= this.SetInactive;

        QuestManager.Instance.journal.EventTaskActivate -= this.SetActive;

        QuestManager.Instance.journal.EventTaskComplete -= this.SetTaskCompleted;

        QuestManager.Instance.journal.EventTaskAbandon -= this.SetTaskAbandoned;

        QuestManager.Instance.journal.EventTaskFail -= this.SetTaskFailed;
    }
}
