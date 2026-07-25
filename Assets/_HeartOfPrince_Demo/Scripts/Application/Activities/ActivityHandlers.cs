using System;
using HeartOfPrince.Domain;

namespace HeartOfPrince.Application
{
    public interface IActivityHandler
    {
        Type InputType { get; }

        ActivityStartResult Resolve(
            ActivityEvaluationContext context,
            ActivityDefinition activity,
            IActivityInput input);
    }

    public abstract class ActivityHandler<TInput> : IActivityHandler
        where TInput : class, IActivityInput
    {
        public Type InputType => typeof(TInput);

        ActivityStartResult IActivityHandler.Resolve(
            ActivityEvaluationContext context,
            ActivityDefinition activity,
            IActivityInput input)
        {
            if (input is not TInput typedInput)
            {
                return ActivityStartResult.Failed(
                    $"Activity '{activity.Id}' expected input " +
                    $"'{typeof(TInput).Name}', but received " +
                    $"'{input?.GetType().Name ?? "null"}'.");
            }

            return ResolveTyped(context, activity, typedInput);
        }

        protected abstract ActivityStartResult ResolveTyped(
            ActivityEvaluationContext context,
            ActivityDefinition activity,
            TInput input);
    }

    public sealed class NoInputActivityHandler :
        ActivityHandler<NoActivityInput>
    {
        protected override ActivityStartResult ResolveTyped(
            ActivityEvaluationContext context,
            ActivityDefinition activity,
            NoActivityInput input)
        {
            string sceneName = activity.ResolveScene(context);

            if (string.IsNullOrWhiteSpace(sceneName))
            {
                return ActivityStartResult.Failed(
                    $"Activity '{activity.Id}' has no scene variant " +
                    "for the current time.");
            }

            return ActivityStartResult.Success(
                new ActivityRunState
                {
                    ActivityId = activity.Id,
                    DisplayName = activity.DisplayName,
                    SceneName = sceneName,
                    StartMinute = context.State.Clock.MinuteOfDay,
                    PlannedDurationMinutes = activity.DurationMinutes,
                    Data = new EmptyActivityRunData()
                });
        }
    }

    public sealed class TalkActivityHandler :
        ActivityHandler<TalkActivityInput>
    {
        private readonly ActivityCatalog catalog;

        public TalkActivityHandler(ActivityCatalog catalog)
        {
            this.catalog = catalog != null
                ? catalog
                : throw new ArgumentNullException(nameof(catalog));
        }

        protected override ActivityStartResult ResolveTyped(
            ActivityEvaluationContext context,
            ActivityDefinition activity,
            TalkActivityInput input)
        {
            CharacterDefinition character =
                catalog.FindCharacter(input.CharacterId);

            if (character == null)
            {
                return ActivityStartResult.Failed(
                    $"Unknown character '{input.CharacterId}'.");
            }

            AvailabilityResult characterAvailability =
                character.EvaluateTalkAvailability(context, activity);

            if (!characterAvailability.IsAvailable)
            {
                return ActivityStartResult.Failed(
                    characterAvailability.Reason);
            }

            string sceneName =
                activity.ResolveScene(context, character.Id);

            if (string.IsNullOrWhiteSpace(sceneName))
            {
                return ActivityStartResult.Failed(
                    $"No talk scene is configured for " +
                    $"{character.DisplayName} at the current time.");
            }

            return ActivityStartResult.Success(
                new ActivityRunState
                {
                    ActivityId = activity.Id,
                    DisplayName = $"Talk to {character.DisplayName}",
                    SceneName = sceneName,
                    StartMinute = context.State.Clock.MinuteOfDay,
                    PlannedDurationMinutes = activity.DurationMinutes,
                    Data = new TalkActivityRunData(character.Id)
                });
        }
    }
}
