using System;
using UnityEngine;

namespace HeartOfPrince.Domain
{
    [CreateAssetMenu(
        fileName = "New Chapter",
        menuName = "Heart of Prince/Narrative/Chapter")]
    public sealed class Chapter : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField]
        private string id;

        [SerializeField]
        private string displayName;

        [TextArea]
        [SerializeField]
        private string description;

        [Header("Chapter Scenes")]
        [SerializeField]
        private string startScene;

        [SerializeField]
        private string endScene;

        [Header("Acts")]
        [SerializeField]
        private Act[] acts = Array.Empty<Act>();

        [Header("Completion")]
        [SerializeField]
        private CompletionCondition completionCondition;

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;

        public string StartScene => startScene;
        public string EndScene => endScene;

        public int ActCount => acts?.Length ?? 0;

        public CompletionCondition CompletionCondition =>
            completionCondition;

        public Act GetAct(int zeroBasedActIndex)
        {
            if (acts == null ||
                zeroBasedActIndex < 0 ||
                zeroBasedActIndex >= acts.Length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(zeroBasedActIndex),
                    $"Chapter '{id}' does not contain act index " +
                    $"{zeroBasedActIndex}.");
            }

            Act act = acts[zeroBasedActIndex];

            if (act == null)
            {
                throw new InvalidOperationException(
                    $"Chapter '{id}' has an empty act reference at " +
                    $"index {zeroBasedActIndex}.");
            }

            return act;
        }

        public int FindActIndexForScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName) ||
                acts == null)
            {
                return -1;
            }

            for (int i = 0; i < acts.Length; i++)
            {
                if (acts[i] != null &&
                    acts[i].ContainsScene(sceneName))
                {
                    return i;
                }
            }

            return -1;
        }

        public bool ContainsScene(string sceneName)
        {
            return string.Equals(
                       sceneName,
                       startScene,
                       StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(
                       sceneName,
                       endScene,
                       StringComparison.OrdinalIgnoreCase) ||
                   FindActIndexForScene(sceneName) >= 0;
        }

        public bool IsComplete(NarrativeProgress progress)
        {
            if (completionCondition == null)
            {
                Debug.LogError(
                    $"Chapter '{name}' has no completion condition.",
                    this);

                return false;
            }

            return completionCondition.IsMet(progress);
        }

        private void OnValidate()
        {
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