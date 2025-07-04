using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DayLogger : Singleton<DayLogger>
{
    public void LogDayEntry(string entryMessage)
    {
        dayLog.Add(entryMessage);
    }

    [SerializeField] List<string> dayLog;

}
