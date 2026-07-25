using UnityEngine;

namespace HeartOfPrince.Domain
{
    [CreateAssetMenu(
        fileName = "New Act",
        menuName = "Heart of Prince/Narrative/Act")]
    public sealed class Act : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [TextArea, SerializeField] private string description;

        [Header("Act Scenes")]
        [SerializeField] private string startScene;
        [SerializeField] private string endScene;

        [Header("Day Loop")]
        [SerializeField] private string dayStartScene;
        [SerializeField] private string decisionScene;
        [SerializeField] private string dayEndScene;
        [SerializeField] private DayRules dayRules;

        [Header("Completion")]
        [SerializeField] private CompletionCondition completionCondition;

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public string StartScene => startScene;
        public string EndScene => endScene;
        public string DayStartScene => dayStartScene;
        public string DecisionScene => decisionScene;
        public string DayEndScene => dayEndScene;
        public DayRules DayRules => dayRules;
        public CompletionCondition CompletionCondition => completionCondition;

        public bool ContainsScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                return false;
            }

            return string.Equals(
                       sceneName,
                       startScene,
                       System.StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(
                       sceneName,
                       endScene,
                       System.StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(
                       sceneName,
                       dayStartScene,
                       System.StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(
                       sceneName,
                       decisionScene,
                       System.StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(
                       sceneName,
                       dayEndScene,
                       System.StringComparison.OrdinalIgnoreCase);
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
            if (string.IsNullOrWhiteSpace(id))
            {
                id = name.Trim().ToLowerInvariant().Replace(" ", "-");
            }
        }
    }
}
