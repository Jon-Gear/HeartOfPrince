using UnityEngine;

namespace HeartOfPrince.Domain
{
    [CreateAssetMenu(
        fileName = "Once Per Day Rule",
        menuName = "Heart of Prince/Activities/Rules/Once Per Day")]
    public sealed class ActivityNotPerformedTodayRule : AvailabilityRule
    {
        [SerializeField]
        private string unavailableReason = "This activity has already been completed today.";

        public override AvailabilityResult Evaluate(
            ActivityEvaluationContext context,
            ActivityDefinition activity,
            string targetId)
        {
            bool alreadyPerformed = context.State.Day.WasPerformedToday(
                activity.Id,
                context.State.Clock.Day);

            return alreadyPerformed
                ? AvailabilityResult.Unavailable(unavailableReason)
                : AvailabilityResult.Available();
        }
    }
}
