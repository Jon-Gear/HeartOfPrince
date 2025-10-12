using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class TimeEntry
{
    [SerializeField] private string TimePeriod = "00:00-24:00";

    [Header("Location")]
    public string sceneName = string.Empty;
    public string markerID = string.Empty;

    public string PeriodStart()
    {
        var (start, _) = ParsePeriod();
        return start;
    }

    public string PeriodEnd()
    {
        var (_, end) = ParsePeriod();
        return end;
    }

    private (string, string) ParsePeriod()
    {
        if (string.IsNullOrEmpty(TimePeriod))
            return ("00:00", "24:00");

        string[] parts = TimePeriod.Split('-');
        if (parts.Length != 2)
            return ("00:00", "24:00");

        return (parts[0].Trim(), parts[1].Trim());
    }
}

[Serializable]
public class DayEntry
{
    public WEEKDAY Weekday;
    public List<TimeEntry> Entries;
}


[CreateAssetMenu(menuName = "NPC Routine")]
public class RoutineData : ScriptableObject
{
    public List<DayEntry> Schedule;
}

