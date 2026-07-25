using UnityEngine;

namespace HeartOfPrince.Domain
{
    [CreateAssetMenu(
        fileName = "Time Window Rule",
        menuName = "Heart of Prince/Activities/Rules/Time Window")]
    public sealed class TimeWindowAvailabilityRule : AvailabilityRule
    {
        [SerializeField, Range(0, 1439)]
        private int earliestMinute;

        [SerializeField, Range(0, 1439)]
        private int latestMinute = 1439;

        [SerializeField]
        private string unavailableReason = "This activity is not available at this time.";

        public override AvailabilityResult Evaluate(
            ActivityEvaluationContext context,
            ActivityDefinition activity,
            string targetId)
        {
            int time = context.State.Clock.NormalizedMinuteOfDay;
            bool available = earliestMinute <= latestMinute
                ? time >= earliestMinute && time <= latestMinute
                : time >= earliestMinute || time <= latestMinute;

            return available
                ? AvailabilityResult.Available()
                : AvailabilityResult.Unavailable(unavailableReason);
        }
    }
}
