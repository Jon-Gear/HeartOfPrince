using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeartOfPrince.Domain
{
    public interface IActivityRunData
    {
        string Summary { get; }
    }

    [Serializable]
    public sealed class EmptyActivityRunData : IActivityRunData
    {
        public string Summary => string.Empty;
    }

    [Serializable]
    public sealed class TalkActivityRunData : IActivityRunData
    {
        [SerializeField] private string characterId;

        public string CharacterId => characterId;
        public string Summary => characterId;

        public TalkActivityRunData(string characterId)
        {
            this.characterId = characterId?.Trim();
        }
    }

    [Serializable]
    public sealed class ActivityRunState
    {
        public string ActivityId;
        public string DisplayName;
        public string SceneName;
        public int StartMinute;
        public int PlannedDurationMinutes;

        [SerializeReference]
        public IActivityRunData Data;

        public TData GetData<TData>() where TData : class, IActivityRunData
        {
            return Data as TData;
        }
    }

    [Serializable]
    public sealed class ActivityHistoryEntry
    {
        public int Day;
        public string ActivityId;
        public string DisplayName;
        public string DataSummary;
        public int StartMinute;
        public int EndMinute;
    }

    [Serializable]
    public sealed class DayActivityState
    {
        public int ActionsCompleted;
        public ActivityRunState CurrentActivity;
        public List<ActivityHistoryEntry> History = new();

        public bool HasRunningActivity => CurrentActivity != null;

        public void BeginDay()
        {
            ActionsCompleted = 0;
            CurrentActivity = null;
        }

        public bool WasPerformedToday(string activityId, int day)
        {
            if (string.IsNullOrWhiteSpace(activityId))
            {
                return false;
            }

            for (int i = History.Count - 1; i >= 0; i--)
            {
                ActivityHistoryEntry entry = History[i];

                if (entry.Day < day)
                {
                    break;
                }

                if (entry.Day == day &&
                    string.Equals(
                        entry.ActivityId,
                        activityId,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
