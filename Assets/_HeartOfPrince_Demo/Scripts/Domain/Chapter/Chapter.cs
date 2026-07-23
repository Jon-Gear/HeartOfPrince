using System;
using UnityEngine;

namespace HeartOfPrince.Domain
{
    /// <summary>
    /// Immutable-at-runtime narrative configuration for one chapter.
    /// </summary>
    [Serializable]
    public sealed class Chapter
    {
        [Header("Identity")]
        [SerializeField] private string id;
        [SerializeField] private string displayName;

        [Header("Chapter Scenes")]
        [SerializeField] private string startScene;
        [SerializeField] private string endScene;

        [Header("Acts")]
        [SerializeField] private Act[] acts = Array.Empty<Act>();

        [Header("Completion")]
        [SerializeReference]
        private CompletionCondition completionCondition =
            new AllActsCompletedCondition();

        public string Id => id;
        public string DisplayName => displayName;
        public string StartScene => startScene;
        public string EndScene => endScene;
        public int ActCount => acts?.Length ?? 0;
        public CompletionCondition CompletionCondition => completionCondition;

        public Chapter()
        {
        }

        public Chapter(
            string id,
            string displayName,
            string startScene,
            string endScene,
            Act[] acts,
            CompletionCondition completionCondition)
        {
            this.id = id;
            this.displayName = displayName;
            this.startScene = startScene;
            this.endScene = endScene;
            this.acts = acts ?? Array.Empty<Act>();
            this.completionCondition = completionCondition ??
                throw new ArgumentNullException(nameof(completionCondition));
        }

        public Act GetAct(int zeroBasedActIndex)
        {
            if (acts == null ||
                zeroBasedActIndex < 0 ||
                zeroBasedActIndex >= acts.Length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(zeroBasedActIndex),
                    $"Chapter '{id}' does not contain act index {zeroBasedActIndex}.");
            }

            return acts[zeroBasedActIndex];
        }

        public bool IsComplete(NarrativeProgress progress)
        {
            return completionCondition != null &&
                   completionCondition.IsMet(progress);
        }
    }
}
