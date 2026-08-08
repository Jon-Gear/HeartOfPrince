using UnityEngine;

namespace HeartOfPrince.Domain
{
    [CreateAssetMenu(
        fileName = "Forbidden Flag Rule",
        menuName = "Heart of Prince/Activities/Rules/Forbidden Flag")]
    public sealed class ForbiddenFlagAvailabilityRule : AvailabilityRule
    {
        [SerializeField] private string forbiddenFlag;
        [SerializeField] private string unavailableReason =
            "A conflicting story condition is currently active.";

        public override AvailabilityResult Evaluate(
            ActivityEvaluationContext context,
            ActivityDefinition activity,
            string targetId)
        {
            return context.State.HasFlag(forbiddenFlag)
                ? AvailabilityResult.Unavailable(unavailableReason)
                : AvailabilityResult.Available();
        }
    }
}
