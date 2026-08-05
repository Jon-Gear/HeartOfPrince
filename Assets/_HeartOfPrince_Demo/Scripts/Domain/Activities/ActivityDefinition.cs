using System;
using UnityEngine;

namespace HeartOfPrince.Domain
{
    [Serializable]
    public sealed class ActivitySceneVariant
    {
        [SerializeField] private string sceneName;

        [Tooltip("Leave empty for an activity-wide scene.")]
        [SerializeField] private string targetId;

        [SerializeField, Range(0, 1439)]
        private int earliestMinute;

        [SerializeField, Range(0, 1439)]
        private int latestMinute = 1439;

        public string SceneName => sceneName;
        public string TargetId => targetId;

        public bool Matches(
            ActivityEvaluationContext context,
            string requestedTargetId)
        {
            bool targetMatches =
                string.IsNullOrWhiteSpace(targetId) ||
                string.Equals(
                    targetId,
                    requestedTargetId,
                    StringComparison.OrdinalIgnoreCase);

            if (!targetMatches)
            {
                return false;
            }

            int time = context.State.Clock.NormalizedMinuteOfDay;

            return earliestMinute <= latestMinute
                ? time >= earliestMinute && time <= latestMinute
                : time >= earliestMinute || time <= latestMinute;
        }

        public int ResolveStandaloneMinute(int fallbackMinute)
        {
            int normalized =
                ((fallbackMinute % WorldClockState.MinutesPerDay) +
                 WorldClockState.MinutesPerDay) %
                WorldClockState.MinutesPerDay;

            bool fallbackMatches =
                earliestMinute <= latestMinute
                    ? normalized >= earliestMinute &&
                      normalized <= latestMinute
                    : normalized >= earliestMinute ||
                      normalized <= latestMinute;

            return fallbackMatches
                ? fallbackMinute
                : earliestMinute;
        }
    }

    [CreateAssetMenu(
        fileName = "New Activity",
        menuName = "Heart of Prince/Activities/Activity")]
    public sealed class ActivityDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;

        [TextArea, SerializeField]
        private string decisionDescription;

        [Tooltip("Selects the runtime module that owns this activity's typed input.")]
        [SerializeField] private string runtimeModuleId;

        [Min(0)]
        [SerializeField] private int durationMinutes = 60;

        [SerializeField]
        private ActivitySceneVariant[] sceneVariants =
            Array.Empty<ActivitySceneVariant>();

        [SerializeField]
        private AvailabilityRule[] availabilityRules =
            Array.Empty<AvailabilityRule>();

        public string Id => id;
        public string RuntimeModuleId => runtimeModuleId;
        public string DisplayName =>
            string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public string DecisionDescription => decisionDescription;
        public int DurationMinutes => Mathf.Max(0, durationMinutes);

        public AvailabilityResult EvaluateAvailability(
            ActivityEvaluationContext context,
            string targetId)
        {
            if (availabilityRules == null)
            {
                return AvailabilityResult.Available();
            }

            foreach (AvailabilityRule rule in availabilityRules)
            {
                if (rule == null)
                {
                    continue;
                }

                AvailabilityResult result =
                    rule.Evaluate(context, this, targetId);

                if (!result.IsAvailable)
                {
                    return result;
                }
            }

            return AvailabilityResult.Available();
        }

        public string ResolveScene(
            ActivityEvaluationContext context,
            string targetId = null)
        {
            if (sceneVariants == null)
            {
                return null;
            }

            foreach (ActivitySceneVariant variant in sceneVariants)
            {
                if (variant != null && variant.Matches(context, targetId))
                {
                    return variant.SceneName;
                }
            }

            return null;
        }

        public bool TryGetStandaloneMinuteForScene(
            string sceneName,
            int fallbackMinute,
            out int minute)
        {
            minute = fallbackMinute;

            if (string.IsNullOrWhiteSpace(sceneName) ||
                sceneVariants == null)
            {
                return false;
            }

            foreach (ActivitySceneVariant variant in sceneVariants)
            {
                if (variant != null &&
                    string.Equals(
                        variant.SceneName,
                        sceneName,
                        StringComparison.OrdinalIgnoreCase))
                {
                    minute =
                        variant.ResolveStandaloneMinute(
                            fallbackMinute);
                    return true;
                }
            }

            return false;
        }

        public bool TryGetTargetForScene(
            string sceneName,
            out string targetId)
        {
            targetId = null;

            if (string.IsNullOrWhiteSpace(sceneName) ||
                sceneVariants == null)
            {
                return false;
            }

            foreach (ActivitySceneVariant variant in sceneVariants)
            {
                if (variant != null &&
                    string.Equals(
                        variant.SceneName,
                        sceneName,
                        StringComparison.OrdinalIgnoreCase))
                {
                    targetId = variant.TargetId;
                    return true;
                }
            }

            return false;
        }

        public bool ContainsScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName) || sceneVariants == null)
            {
                return false;
            }

            foreach (ActivitySceneVariant variant in sceneVariants)
            {
                if (variant != null &&
                    string.Equals(
                        variant.SceneName,
                        sceneName,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                id = name.Trim().ToLowerInvariant().Replace(" ", "-");
            }

            durationMinutes = Mathf.Max(0, durationMinutes);
        }
    }
}
