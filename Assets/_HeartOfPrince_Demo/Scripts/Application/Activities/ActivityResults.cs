using System;
using HeartOfPrince.Domain;

namespace HeartOfPrince.Application
{
    public sealed class ActivityStartResult
    {
        public bool CanStart { get; }
        public ActivityRunState Run { get; }
        public string FailureReason { get; }

        private ActivityStartResult(
            bool canStart,
            ActivityRunState run,
            string failureReason)
        {
            CanStart = canStart;
            Run = run;
            FailureReason = failureReason;
        }

        public static ActivityStartResult Success(ActivityRunState run)
        {
            return new ActivityStartResult(
                true,
                run ?? throw new ArgumentNullException(nameof(run)),
                null);
        }

        public static ActivityStartResult Failed(string reason)
        {
            return new ActivityStartResult(
                false,
                null,
                string.IsNullOrWhiteSpace(reason)
                    ? "The activity could not be started."
                    : reason);
        }
    }

    public sealed class ActivityResult
    {
        private readonly Action<GameState> applyEffects;

        public int? DurationOverrideMinutes { get; }

        private ActivityResult(
            int? durationOverrideMinutes,
            Action<GameState> applyEffects)
        {
            DurationOverrideMinutes = durationOverrideMinutes;
            this.applyEffects = applyEffects;
        }

        public static ActivityResult Success(
            int? durationOverrideMinutes = null,
            Action<GameState> applyEffects = null)
        {
            return new ActivityResult(
                durationOverrideMinutes,
                applyEffects);
        }

        public void ApplyTo(GameState state)
        {
            applyEffects?.Invoke(state);
        }
    }

    public readonly struct ActivityCompletion
    {
        public ActivityRunState CompletedRun { get; }
        public int DurationMinutes { get; }
        public bool ShouldEndDay { get; }

        public ActivityCompletion(
            ActivityRunState completedRun,
            int durationMinutes,
            bool shouldEndDay)
        {
            CompletedRun = completedRun;
            DurationMinutes = durationMinutes;
            ShouldEndDay = shouldEndDay;
        }
    }
}
