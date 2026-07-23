using System;
using UnityEngine;

namespace HeartOfPrince.Domain
{
    /// <summary>
    /// Read-only runtime data supplied to narrative completion rules.
    /// </summary>
    public readonly struct NarrativeProgress
    {
        public GameState State { get; }
        public int CompletedDaysInAct { get; }
        public int CompletedActsInChapter { get; }
        public int TotalActsInChapter { get; }

        public NarrativeProgress(
            GameState state,
            int completedDaysInAct,
            int completedActsInChapter,
            int totalActsInChapter)
        {
            State = state;
            CompletedDaysInAct = completedDaysInAct;
            CompletedActsInChapter = completedActsInChapter;
            TotalActsInChapter = totalActsInChapter;
        }
    }

    /// <summary>
    /// Base class for chapter and act completion rules.
    /// Add subclasses for relationship, topic, flag, or compound conditions.
    /// </summary>
    [Serializable]
    public abstract class CompletionCondition
    {
        public abstract bool IsMet(NarrativeProgress progress);
    }

    /// <summary>
    /// Completes an act after a configured number of days.
    /// </summary>
    [Serializable]
    public sealed class DaysCompletedCondition : CompletionCondition
    {
        [SerializeField, Min(1)]
        private int requiredDays = 2;

        public int RequiredDays => Math.Max(1, requiredDays);

        public DaysCompletedCondition()
        {
        }

        public DaysCompletedCondition(int requiredDays)
        {
            this.requiredDays = Math.Max(1, requiredDays);
        }

        public override bool IsMet(NarrativeProgress progress)
        {
            return progress.CompletedDaysInAct >= RequiredDays;
        }
    }

    /// <summary>
    /// Completes a chapter when every configured act has completed.
    /// </summary>
    [Serializable]
    public sealed class AllActsCompletedCondition : CompletionCondition
    {
        public override bool IsMet(NarrativeProgress progress)
        {
            return progress.TotalActsInChapter > 0 &&
                   progress.CompletedActsInChapter >= progress.TotalActsInChapter;
        }
    }
}
