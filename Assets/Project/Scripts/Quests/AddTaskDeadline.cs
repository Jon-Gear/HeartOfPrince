using GameCreator.Runtime.Common;
using GameCreator.Runtime.Quests;
using GameCreator.Runtime.VisualScripting;
using System;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class AddTaskDeadline : Instruction
{
    [SerializeField] private PickTask m_Task = new PickTask();
    [SerializeField] private Deadline m_Deadline = new Deadline();
    protected override System.Threading.Tasks.Task Run(Args args)
    {
        GameManager.Instance.GetSystem<QuestManager>().AddTaskDeadline(m_Task, m_Deadline);
        return DefaultResult;
    }
}

public class AddTaskDeadlineToday : Instruction
{
    [SerializeField] private PickTask m_Task = new PickTask();
    [SerializeField] private string m_Time = "12:00"; // Default time for the deadline
    protected override System.Threading.Tasks.Task Run(Args args)
    {
        Deadline deadline = new Deadline();

        deadline.Time = m_Time;
        deadline.WeekDay = GameManager.Instance.GetSystem<TimeManager>().GetWeekDay(); // Set to today

        GameManager.Instance.GetSystem<QuestManager>().AddTaskDeadline(m_Task, deadline);
        return DefaultResult;
    }
}

public class AddTaskDeadlineInHoursFromNow : Instruction
{
    [SerializeField] private PickTask m_Task = new PickTask();
    [SerializeField] private string m_HoursFromNow = "01:30"; // Default to 60 minutes from now
    protected override System.Threading.Tasks.Task Run(Args args)
    {
        Deadline deadline = new Deadline();
        deadline.Time = GameManager.Instance.GetSystem<TimeManager>().GetTimeFromHoursAndMinutesFromNowString(m_HoursFromNow);
        deadline.WeekDay = GameManager.Instance.GetSystem<TimeManager>().GetWeekDay(); // Set to today
        GameManager.Instance.GetSystem<QuestManager>().AddTaskDeadline(m_Task, deadline);
        return DefaultResult;
    }
}


public class AddTaskDeadlineInDays : Instruction
{
    [SerializeField] private PickTask m_Task = new PickTask();
    [SerializeField] private int m_DaysFromToday = 1; // Default to 1 day from now
    [SerializeField] private string m_Time = "12:00"; // Default time for the deadline
    protected override System.Threading.Tasks.Task Run(Args args)
    {
        Deadline deadline = new Deadline();
        deadline.Time = m_Time;
        deadline.WeekDay = GameManager.Instance.GetSystem<TimeManager>().GetWeekDay() + m_DaysFromToday; // Set to today + specified days
        GameManager.Instance.GetSystem<QuestManager>().AddTaskDeadline(m_Task, deadline);
        return DefaultResult;
    }
}