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
        if (GameManager.Instance.GetSystem<QuestManager>().journal.IsQuestInactive(this.m_Task.Quest))
        {
            SetInactive(m_Task.Quest, m_Task.TaskId);
            return;
        }
        else if (GameManager.Instance.GetSystem<QuestManager>().journal.IsQuestActive(this.m_Task.Quest))
        {
            if (GameManager.Instance.GetSystem<QuestManager>().journal.IsTaskInactive(this.m_Task.Quest, this.m_Task.TaskId))
            {
                SetInactive(m_Task.Quest, m_Task.TaskId);
            }
            else if (GameManager.Instance.GetSystem<QuestManager>().journal.IsTaskActive(this.m_Task.Quest, this.m_Task.TaskId))
            {
                SetActive(m_Task.Quest, m_Task.TaskId);
            }
            else if (GameManager.Instance.GetSystem<QuestManager>().journal.IsTaskCompleted(this.m_Task.Quest, this.m_Task.TaskId))
            {
                SetTaskCompleted(m_Task.Quest, m_Task.TaskId);
            }
            else if (GameManager.Instance.GetSystem<QuestManager>().journal.IsTaskAbandoned(this.m_Task.Quest, this.m_Task.TaskId))
            {
                SetTaskAbandoned(m_Task.Quest, m_Task.TaskId);
            }
            else if (GameManager.Instance.GetSystem<QuestManager>().journal.IsTaskFailed(this.m_Task.Quest, this.m_Task.TaskId))
            {
                SetTaskFailed(m_Task.Quest, m_Task.TaskId);
            }
        }

        if(GameManager.Instance.GetSystem<QuestManager>().journal.IsQuestCompleted(this.m_Task.Quest))
        {
            SetQuestCompleted(m_Task.Quest);
        }
        else if(GameManager.Instance.GetSystem<QuestManager>().journal.IsQuestAbandoned(this.m_Task.Quest))
        {
            SetQuestAbandoned(m_Task.Quest);
        }
        else if(GameManager.Instance.GetSystem<QuestManager>().journal.IsQuestFailed(this.m_Task.Quest))
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

        GameManager.Instance.GetSystem<QuestManager>().journal.EventTaskDeactivate -= this.SetInactive;
        GameManager.Instance.GetSystem<QuestManager>().journal.EventTaskDeactivate += this.SetInactive;

        GameManager.Instance.GetSystem<QuestManager>().journal.EventTaskActivate -= this.SetActive;
        GameManager.Instance.GetSystem<QuestManager>().journal.EventTaskActivate += this.SetActive;

        GameManager.Instance.GetSystem<QuestManager>().journal.EventTaskComplete -= this.SetTaskCompleted;
        GameManager.Instance.GetSystem<QuestManager>().journal.EventTaskComplete += this.SetTaskCompleted;

        GameManager.Instance.GetSystem<QuestManager>().journal.EventTaskAbandon -= this.SetTaskAbandoned;
        GameManager.Instance.GetSystem<QuestManager>().journal.EventTaskAbandon += this.SetTaskAbandoned;

        GameManager.Instance.GetSystem<QuestManager>().journal.EventTaskFail -= this.SetTaskFailed;
        GameManager.Instance.GetSystem<QuestManager>().journal.EventTaskFail += this.SetTaskFailed;

        // Quests


    }

    protected void Unsubscribe()
    {
        //if (QuestManager.IsQuitting) return;

        GameManager.Instance.GetSystem<QuestManager>().journal.EventTaskDeactivate -= this.SetInactive;

        GameManager.Instance.GetSystem<QuestManager>().journal.EventTaskActivate -= this.SetActive;

        GameManager.Instance.GetSystem<QuestManager>().journal.EventTaskComplete -= this.SetTaskCompleted;

        GameManager.Instance.GetSystem<QuestManager>().journal.EventTaskAbandon -= this.SetTaskAbandoned;

        GameManager.Instance.GetSystem<QuestManager>().journal.EventTaskFail -= this.SetTaskFailed;
    }
}
