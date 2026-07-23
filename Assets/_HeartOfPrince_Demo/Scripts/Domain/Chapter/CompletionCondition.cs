using System;
using UnityEngine;

namespace HeartOfPrince.Domain
{
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

    public abstract class CompletionCondition : ScriptableObject
    {
        public abstract bool IsMet(NarrativeProgress progress);
    }

    

    
}