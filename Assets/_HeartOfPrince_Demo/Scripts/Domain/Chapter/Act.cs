using System;
using UnityEngine;

namespace HeartOfPrince.Domain
{
    /// <summary>
    /// Immutable-at-runtime narrative configuration for one act.
    /// Mutable progress remains in GameLoopState.
    /// </summary>
    [Serializable]
    public sealed class Act
    {
        [Header("Identity")]
        [SerializeField] private string id;
        [SerializeField] private string displayName;

        [Header("Act Scenes")]
        [SerializeField] private string startScene;
        [SerializeField] private string endScene;

        [Header("Day Loop Scenes")]
        [SerializeField] private string dayStartScene;
        [SerializeField] private string dayEndScene;

        [Header("Decisions")]
        [SerializeField, Min(1)] private int decisionsPerDay = 2;
        [SerializeField] private string[] decisionScenes = Array.Empty<string>();

        [Header("Completion")]
        [SerializeReference]
        private CompletionCondition completionCondition =
            new DaysCompletedCondition(2);

        public string Id => id;
        public string DisplayName => displayName;
        public string StartScene => startScene;
        public string EndScene => endScene;
        public string DayStartScene => dayStartScene;
        public string DayEndScene => dayEndScene;
        public int DecisionsPerDay => Math.Max(1, decisionsPerDay);
        public CompletionCondition CompletionCondition => completionCondition;

        public Act()
        {
        }

        public Act(
            string id,
            string displayName,
            string startScene,
            string endScene,
            string dayStartScene,
            string dayEndScene,
            int decisionsPerDay,
            string[] decisionScenes,
            CompletionCondition completionCondition)
        {
            this.id = id;
            this.displayName = displayName;
            this.startScene = startScene;
            this.endScene = endScene;
            this.dayStartScene = dayStartScene;
            this.dayEndScene = dayEndScene;
            this.decisionsPerDay = Math.Max(1, decisionsPerDay);
            this.decisionScenes = decisionScenes ?? Array.Empty<string>();
            this.completionCondition = completionCondition ??
                throw new ArgumentNullException(nameof(completionCondition));
        }

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

            int sceneIndex = Math.Max(
                0,
                Math.Min(zeroBasedDecisionIndex, decisionScenes.Length - 1));

            return decisionScenes[sceneIndex];
        }

        public bool IsComplete(NarrativeProgress progress)
        {
            return completionCondition != null &&
                   completionCondition.IsMet(progress);
        }
    }
}
