using System;
using UnityEngine;

namespace HeartOfPrince.Domain
{
    [CreateAssetMenu(
        fileName = "New Act",
        menuName = "Heart of Prince/Narrative/Act")]
    public sealed class Act : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField]
        private string id;

        [SerializeField]
        private string displayName;

        [TextArea]
        [SerializeField]
        private string description;

        [Header("Act Scenes")]
        [SerializeField]
        private string startScene;

        [SerializeField]
        private string endScene;

        [Header("Day Loop Scenes")]
        [SerializeField]
        private string dayStartScene;

        [SerializeField]
        private string dayEndScene;

        [Header("Decisions")]
        [SerializeField, Min(1)]
        private int decisionsPerDay = 2;

        [Tooltip("Decision scenes in chronological order.")]
        [SerializeField]
        private string[] decisionScenes = Array.Empty<string>();

        [Header("Completion")]
        [SerializeField]
        private CompletionCondition completionCondition;

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;

        public string StartScene => startScene;
        public string EndScene => endScene;
        public string DayStartScene => dayStartScene;
        public string DayEndScene => dayEndScene;

        public int DecisionsPerDay => Mathf.Max(1, decisionsPerDay);
        public CompletionCondition CompletionCondition =>
            completionCondition;

        public string GetDecisionScene(int zeroBasedDecisionIndex)
        {
            if (decisionScenes == null || decisionScenes.Length == 0)
            {
                throw new InvalidOperationException(
                    $"Act '{id}' has no decision scenes configured.");
            }

            if (decisionScenes.Length == 1)
            {
                return decisionScenes[0];
            }

            int sceneIndex = Mathf.Clamp(
                zeroBasedDecisionIndex,
                0,
                decisionScenes.Length - 1);

            return decisionScenes[sceneIndex];
        }

        public bool IsComplete(NarrativeProgress progress)
        {
            if (completionCondition == null)
            {
                Debug.LogError(
                    $"Act '{name}' has no completion condition.",
                    this);

                return false;
            }

            return completionCondition.IsMet(progress);
        }

        private void OnValidate()
        {
            decisionsPerDay = Mathf.Max(1, decisionsPerDay);

            if (string.IsNullOrWhiteSpace(id))
            {
                id = name
                    .Trim()
                    .ToLowerInvariant()
                    .Replace(" ", "-");
            }
        }
    }
}