using GameCreator.Runtime.Behavior;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Quests;
using UnityEngine;

public class QuestConditionalObject : MonoBehaviour
{
    [SerializeField] private Quest myQuest;
    [SerializeField] private GameCreator.Runtime.Quests.State requiredQuestState;
    
    /**/
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Check();
        //Subscribe();
    }
    private void OnDestroy()
    {
        //Unsubscribe();
    }

    void Check()
    {
        if (QuestManager.Instance.GetQuestState(myQuest) == requiredQuestState)
        {

            Debug.Log("Enable");
            Toggle(true);
        }
        else
        {

            Debug.Log("Disable");
            Toggle(false);
        }
    }


    protected void OnChange(Quest quest)
    {
        if (quest != myQuest) return;
        Check();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Toggle(bool toggle)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(toggle);
        }
    }


    protected void Subscribe()
    {
        QuestManager.Instance.journal.EventQuestActivate -= this.OnChange;
        QuestManager.Instance.journal.EventQuestActivate += this.OnChange;
    }

    protected void Unsubscribe()
    {
        QuestManager.Instance.journal.EventQuestActivate -= this.OnChange;
    }
}
