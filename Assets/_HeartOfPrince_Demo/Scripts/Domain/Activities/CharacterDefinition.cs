using System;
using UnityEngine;

namespace HeartOfPrince.Domain
{
    [CreateAssetMenu(
        fileName = "New Character",
        menuName = "Heart of Prince/Characters/Character")]
    public sealed class CharacterDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;

        [TextArea, SerializeField]
        private string talkDecisionDescription;

        [SerializeField]
        private AvailabilityRule[] talkAvailabilityRules =
            Array.Empty<AvailabilityRule>();

        public string Id => id;
        public string DisplayName =>
            string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public string TalkDecisionDescription => talkDecisionDescription;

        public AvailabilityResult EvaluateTalkAvailability(
            ActivityEvaluationContext context,
            ActivityDefinition talkActivity)
        {
            if (talkAvailabilityRules == null)
            {
                return AvailabilityResult.Available();
            }

            foreach (AvailabilityRule rule in talkAvailabilityRules)
            {
                if (rule == null)
                {
                    continue;
                }

                AvailabilityResult result =
                    rule.Evaluate(context, talkActivity, id);

                if (!result.IsAvailable)
                {
                    return result;
                }
            }

            return AvailabilityResult.Available();
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                id = name.Trim().ToLowerInvariant().Replace(" ", "-");
            }
        }
    }
}
