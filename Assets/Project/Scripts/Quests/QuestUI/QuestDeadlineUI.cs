using GameCreator.Runtime.Common.UnityUI;
using GameCreator.Runtime.Quests;
using GameCreator.Runtime.Quests.UnityUI;
using UnityEngine;

public class QuestDeadlineUI : MonoBehaviour
{

    [SerializeField] private TextReference m_WeekDay = new TextReference();
    [SerializeField] private TextReference m_Time = new TextReference();
    /**

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    private void OnEnable()
    {
        QuestUI.EventSelect -= this.OnSelectQuest;
        QuestUI.EventSelect += this.OnSelectQuest;
    }

    private void OnDisable()
    {
        QuestUI.EventSelect -= this.OnSelectQuest;
    }

    private void OnSelectQuest(Journal journal, Quest quest)
    {
        if (this.m_ActiveIfSelected != null)
        {
            bool isSelected = quest != null;
            this.m_ActiveIfSelected.SetActive(isSelected);
        }

        if (quest == null) return;

        SetDeadlineUI();
    }

    void SetDeadlineUI()
    {
        QuestManager.Instance.deadlines.
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    /**/
}
