using UnityEngine;

namespace HeartOfPrince.Domain
{
    [CreateAssetMenu(
        fileName = "All Acts Completed Condition",
        menuName = "Heart of Prince/Narrative/Conditions/All Acts Completed")]
    public sealed class AllActsCompletedCondition : CompletionCondition
    {
        public override bool IsMet(NarrativeProgress progress)
        {
            return progress.TotalActsInChapter > 0 &&
                   progress.CompletedActsInChapter >=
                   progress.TotalActsInChapter;
        }
    }
}
