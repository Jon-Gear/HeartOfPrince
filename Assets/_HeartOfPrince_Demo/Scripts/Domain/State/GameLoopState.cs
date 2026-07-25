using System;

namespace HeartOfPrince.Domain
{
    public enum GameLoopPhase
    {
        None,
        StartingGame,
        StartingAct,
        PlayingDayOpening,
        AwaitingDecision,
        LoadingActivity,
        PerformingActivity,
        ResolvingActivity,
        EndingDay,
        TransitioningAct,
        PlayingEnding,
        StandaloneScene,
        StandaloneComplete,
        Completed
    }

    [Serializable]
    public sealed class GameLoopState
    {
        public int Chapter = 1;
        public int CurrentAct = 1;
        public bool IsDayEnding;
        public bool IsGameComplete;
        public GameLoopPhase Phase = GameLoopPhase.None;

        public void Reset()
        {
            Chapter = 1;
            CurrentAct = 1;
            IsDayEnding = false;
            IsGameComplete = false;
            Phase = GameLoopPhase.None;
        }
    }
}
