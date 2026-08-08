using System;
using System.Collections.Generic;
using HeartOfPrince.Domain;

namespace HeartOfPrince.Application
{
    public sealed class ActivityOptionDraft
    {
        public string DisplayName { get; }
        public IActivityRequest Request { get; }

        public ActivityOptionDraft(
            string displayName,
            IActivityRequest request)
        {
            DisplayName = displayName;
            Request = request
                ?? throw new ArgumentNullException(nameof(request));
        }
    }

    public sealed class ActivityOption
    {
        public string DisplayName { get; }
        public IActivityRequest Request { get; }
        public bool IsAvailable { get; }
        public string UnavailableReason { get; }

        public ActivityOption(
            string displayName,
            IActivityRequest request,
            bool isAvailable,
            string unavailableReason)
        {
            DisplayName = displayName;
            Request = request;
            IsAvailable = isAvailable;
            UnavailableReason = unavailableReason;
        }
    }

    public interface IActivityOptionProvider
    {
        IEnumerable<ActivityOptionDraft> GetOptions(
            ActivityEvaluationContext context);
    }

    public sealed class NoInputActivityOptionProvider :
        IActivityOptionProvider
    {
        private readonly ActivityDefinition activity;

        public NoInputActivityOptionProvider(
            ActivityDefinition activity)
        {
            this.activity = activity != null
                ? activity
                : throw new ArgumentNullException(nameof(activity));
        }

        public IEnumerable<ActivityOptionDraft> GetOptions(
            ActivityEvaluationContext context)
        {
            if (!activity.EvaluateAvailability(context, null).IsAvailable)
            {
                yield break;
            }

            yield return new ActivityOptionDraft(
                activity.DisplayName,
                new ActivityRequest<NoActivityInput>(
                    activity,
                    NoActivityInput.Instance));
        }
    }

    public sealed class TalkActivityOptionProvider :
        IActivityOptionProvider
    {
        private readonly ActivityDefinition activity;
        private readonly ActivityCatalog catalog;

        public TalkActivityOptionProvider(
            ActivityDefinition activity,
            ActivityCatalog catalog)
        {
            this.activity = activity != null
                ? activity
                : throw new ArgumentNullException(nameof(activity));

            this.catalog = catalog != null
                ? catalog
                : throw new ArgumentNullException(nameof(catalog));
        }

        public IEnumerable<ActivityOptionDraft> GetOptions(
            ActivityEvaluationContext context)
        {
            CharacterDefinition[] characters = catalog.Characters;

            if (characters == null)
            {
                yield break;
            }

            foreach (CharacterDefinition character in characters)
            {
                if (character == null)
                {
                    continue;
                }

                if (!character
                        .EvaluateTalkAvailability(context, activity)
                        .IsAvailable ||
                    !activity
                        .EvaluateAvailability(context, character.Id)
                        .IsAvailable)
                {
                    continue;
                }

                yield return new ActivityOptionDraft(
                    $"Talk to {character.DisplayName}",
                    new ActivityRequest<TalkActivityInput>(
                        activity,
                        new TalkActivityInput(character.Id)));
            }
        }
    }

    public sealed class ActivityQueryService
    {
        private readonly ActivityService activityService;
        private readonly List<IActivityOptionProvider> providers = new();

        public ActivityQueryService(ActivityService activityService)
        {
            this.activityService = activityService
                ?? throw new ArgumentNullException(nameof(activityService));
        }

        public void Register(IActivityOptionProvider provider)
        {
            providers.Add(
                provider ?? throw new ArgumentNullException(nameof(provider)));
        }

        public IReadOnlyList<ActivityOption> GetOptions()
        {
            var options = new List<ActivityOption>();
            ActivityEvaluationContext context =
                activityService.CreateContext();

            foreach (IActivityOptionProvider provider in providers)
            {
                foreach (ActivityOptionDraft draft in
                         provider.GetOptions(context))
                {
                    ActivityStartResult evaluation =
                        activityService.Evaluate(draft.Request);

                    options.Add(
                        new ActivityOption(
                            draft.DisplayName,
                            draft.Request,
                            evaluation.CanStart,
                            evaluation.FailureReason));
                }
            }

            return options;
        }

        public ActivityOption FindOption(
            string activityId,
            IActivityInput input)
        {
            if (string.IsNullOrWhiteSpace(activityId) || input == null)
            {
                return null;
            }

            IReadOnlyList<ActivityOption> options = GetOptions();

            foreach (ActivityOption option in options)
            {
                if (string.Equals(
                        option.Request.Activity.Id,
                        activityId,
                        StringComparison.OrdinalIgnoreCase) &&
                    option.Request.Input.Matches(input))
                {
                    return option;
                }
            }

            return null;
        }

        public ActivityOption FindOption(
            string activityId,
            string selectionKey)
        {
            IReadOnlyList<ActivityOption> options = GetOptions();

            foreach (ActivityOption option in options)
            {
                bool activityMatches = string.Equals(
                    option.Request.Activity.Id,
                    activityId,
                    StringComparison.OrdinalIgnoreCase);

                bool inputMatches = string.Equals(
                    option.Request.Input.SelectionKey ?? string.Empty,
                    selectionKey ?? string.Empty,
                    StringComparison.OrdinalIgnoreCase);

                if (activityMatches && inputMatches)
                {
                    return option;
                }
            }

            return null;
        }
    }
}
