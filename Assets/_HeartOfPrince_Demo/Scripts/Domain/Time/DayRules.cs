using UnityEngine;

namespace HeartOfPrince.Domain
{
    public enum EndOfDayActivityPolicy
    {
        AllowOvertime,
        RequireActivityToFit
    }

    [CreateAssetMenu(
        fileName = "New Day Rules",
        menuName = "Heart of Prince/Time/Day Rules")]
    public sealed class DayRules : ScriptableObject
    {
        [SerializeField, Range(0, 1439)]
        private int wakeMinute = 8 * 60;

        [SerializeField, Range(1, 2880)]
        private int endMinute = 22 * 60;

        [Tooltip("Zero removes the action-count limit.")]
        [SerializeField, Min(0)]
        private int maximumActions = 2;

        [SerializeField]
        private EndOfDayActivityPolicy endOfDayActivityPolicy =
            EndOfDayActivityPolicy.AllowOvertime;

        public int WakeMinute => wakeMinute;
        public int EndMinute => endMinute;
        public int MaximumActions => Mathf.Max(0, maximumActions);
        public EndOfDayActivityPolicy ActivityPolicy => endOfDayActivityPolicy;

        public bool ShouldEndDay(GameState state)
        {
            if (state == null)
            {
                return true;
            }

            bool timeExpired = state.Clock.MinuteOfDay >= EndMinute;
            bool actionLimitReached =
                MaximumActions > 0 &&
                state.Day.ActionsCompleted >= MaximumActions;

            return timeExpired || actionLimitReached;
        }

        public AvailabilityResult CanStartActivity(
            GameState state,
            int durationMinutes)
        {
            if (state.Day.HasRunningActivity)
            {
                return AvailabilityResult.Unavailable(
                    "Another activity is already running.");
            }

            if (ShouldEndDay(state))
            {
                return AvailabilityResult.Unavailable(
                    "There is no time left for another activity today.");
            }

            if (endOfDayActivityPolicy ==
                    EndOfDayActivityPolicy.RequireActivityToFit &&
                state.Clock.MinuteOfDay + Mathf.Max(0, durationMinutes) >
                    EndMinute)
            {
                return AvailabilityResult.Unavailable(
                    "There is not enough time remaining for this activity.");
            }

            return AvailabilityResult.Available();
        }
    }
}
