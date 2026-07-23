using UnityEngine;

namespace HeartOfPrince.Domain
{
    [CreateAssetMenu(
        fileName = "Days Completed Condition",
        menuName = "Heart of Prince/Narrative/Conditions/Days Completed")]
    public sealed class DaysCompletedCondition : CompletionCondition
    {
        [SerializeField, Min(1)]
        private int requiredDays = 2;

        public int RequiredDays => Mathf.Max(1, requiredDays);

        public override bool IsMet(NarrativeProgress progress)
        {
            return progress.CompletedDaysInAct >= RequiredDays;
        }

        private void OnValidate()
        {
            requiredDays = Mathf.Max(1, requiredDays);
        }
    }
}
