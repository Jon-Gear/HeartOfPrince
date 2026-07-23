using System;

namespace HeartOfPrince.Domain
{
    public enum GameLoopPhase
    {
        None,
        StartingGame,
        
        StartingAct,
        
        StartingDay,

        PlayingDayOpening,
        AwaitingDecision,
        
        LoadingAction,
        PerformingTalk,
        PerformingPonder,
        ResolvingAction,
        
        EndingDay,
        
        TransitioningAct,
        
        PlayingEnding,
        
        StandaloneScene,
        StandaloneComplete,
        Completed
    }

    public enum GameLoopAction
    {
        None,
        Talk,
        Ponder
    }

    [Serializable]
    public sealed class GameLoopState
    {
        public int Chapter = 1;
        public int CurrentAct = 1;
        public int CurrentDay = 1;
        public int CurrentDecisionIndex;
        public int DecisionsAllowedPerDay = 2;
        public bool IsActionRunning;
        public bool IsDayEnding;
        public bool IsGameComplete;
        public GameLoopPhase Phase = GameLoopPhase.None;
        public GameLoopAction CurrentAction = GameLoopAction.None;
        public string CurrentTalkCharacterId;

        public void Reset(int decisionsAllowedPerDay)
        {
            CurrentAct = 1;
            CurrentDay = 1;
            CurrentDecisionIndex = 0;
            DecisionsAllowedPerDay = Math.Max(1, decisionsAllowedPerDay);
            IsActionRunning = false;
            IsDayEnding = false;
            IsGameComplete = false;
            Phase = GameLoopPhase.None;
            CurrentAction = GameLoopAction.None;
            CurrentTalkCharacterId = null;
        }
    }
}
