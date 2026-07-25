using UnityEngine;

namespace HeartOfPrince.Domain
{
    public abstract class AvailabilityRule : ScriptableObject
    {
        public abstract AvailabilityResult Evaluate(
            ActivityEvaluationContext context,
            ActivityDefinition activity,
            string targetId);
    }
}
