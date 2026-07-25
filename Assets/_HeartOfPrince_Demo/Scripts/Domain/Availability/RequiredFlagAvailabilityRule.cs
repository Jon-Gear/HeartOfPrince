using UnityEngine;

namespace HeartOfPrince.Domain
{
    [CreateAssetMenu(
        fileName = "Required Flag Rule",
        menuName = "Heart of Prince/Activities/Rules/Required Flag")]
    public sealed class RequiredFlagAvailabilityRule : AvailabilityRule
    {
        [SerializeField] private string requiredFlag;
        [SerializeField] private string unavailableReason =
            "The required story condition has not been met.";

        public override AvailabilityResult Evaluate(
            ActivityEvaluationContext context,
            ActivityDefinition activity,
            string targetId)
        {
            return context.State.HasFlag(requiredFlag)
                ? AvailabilityResult.Available()
                : AvailabilityResult.Unavailable(unavailableReason);
        }
    }
}
