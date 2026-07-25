using System;
using System.Collections.Generic;
using HeartOfPrince.Domain;

namespace HeartOfPrince.Application
{
    public sealed class ActivityService
    {
        private readonly GameState state;
        private DayRules dayRules;
        private readonly Dictionary<string, IActivityHandler> handlers =
            new(StringComparer.OrdinalIgnoreCase);

        public ActivityService(
            GameState state,
            DayRules dayRules)
        {
            this.state = state
                ?? throw new ArgumentNullException(nameof(state));

            this.dayRules = dayRules != null
                ? dayRules
                : throw new ArgumentNullException(nameof(dayRules));
        }


        public void SetDayRules(DayRules rules)
        {
            dayRules = rules != null
                ? rules
                : throw new ArgumentNullException(nameof(rules));
        }

        public ActivityEvaluationContext CreateContext()
        {
            return new ActivityEvaluationContext(state, dayRules);
        }

        public void Register(
            string activityId,
            IActivityHandler handler)
        {
            if (string.IsNullOrWhiteSpace(activityId))
            {
                throw new ArgumentException(
                    "An activity ID is required.",
                    nameof(activityId));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            string normalizedId = activityId.Trim();

            if (handlers.ContainsKey(normalizedId))
            {
                throw new InvalidOperationException(
                    $"An activity handler is already registered for " +
                    $"'{normalizedId}'.");
            }

            handlers.Add(normalizedId, handler);
        }

        public ActivityStartResult Evaluate(IActivityRequest request)
        {
            if (request == null)
            {
                return ActivityStartResult.Failed(
                    "The activity request is null.");
            }

            if (request.Activity == null)
            {
                return ActivityStartResult.Failed(
                    "The activity definition is missing.");
            }

            if (request.Input == null)
            {
                return ActivityStartResult.Failed(
                    $"Activity '{request.Activity.Id}' has no input payload.");
            }

            if (!handlers.TryGetValue(
                    request.Activity.Id,
                    out IActivityHandler handler))
            {
                return ActivityStartResult.Failed(
                    $"No handler is registered for activity " +
                    $"'{request.Activity.Id}'.");
            }

            AvailabilityResult dayAvailability =
                dayRules.CanStartActivity(
                    state,
                    request.Activity.DurationMinutes);

            if (!dayAvailability.IsAvailable)
            {
                return ActivityStartResult.Failed(
                    dayAvailability.Reason);
            }

            ActivityEvaluationContext context = CreateContext();

            AvailabilityResult activityAvailability =
                request.Activity.EvaluateAvailability(
                    context,
                    request.Input.SelectionKey);

            if (!activityAvailability.IsAvailable)
            {
                return ActivityStartResult.Failed(
                    activityAvailability.Reason);
            }

            return handler.Resolve(
                context,
                request.Activity,
                request.Input);
        }

        public bool TryStartActivity(
            IActivityRequest request,
            out ActivityRunState run,
            out string failureReason)
        {
            ActivityStartResult result = Evaluate(request);

            if (!result.CanStart)
            {
                run = null;
                failureReason = result.FailureReason;
                return false;
            }

            state.Day.CurrentActivity = result.Run;
            run = result.Run;
            failureReason = null;
            return true;
        }

        public bool TryStartActivity<TInput>(
            ActivityDefinition activity,
            TInput input,
            out ActivityRunState run,
            out string failureReason)
            where TInput : class, IActivityInput
        {
            return TryStartActivity(
                new ActivityRequest<TInput>(activity, input),
                out run,
                out failureReason);
        }

        public ActivityCompletion CompleteCurrentActivity(
            ActivityResult result)
        {
            ActivityRunState run = state.Day.CurrentActivity;

            if (run == null)
            {
                throw new InvalidOperationException(
                    "No activity is currently running.");
            }

            result ??= ActivityResult.Success();
            result.ApplyTo(state);

            int duration = Math.Max(
                0,
                result.DurationOverrideMinutes ??
                run.PlannedDurationMinutes);

            state.Clock.Advance(duration);
            state.Day.ActionsCompleted++;

            state.Day.History.Add(
                new ActivityHistoryEntry
                {
                    Day = state.Clock.Day,
                    ActivityId = run.ActivityId,
                    DisplayName = run.DisplayName,
                    DataSummary = run.Data?.Summary,
                    StartMinute = run.StartMinute,
                    EndMinute = state.Clock.MinuteOfDay
                });

            state.Day.CurrentActivity = null;

            return new ActivityCompletion(
                run,
                duration,
                dayRules.ShouldEndDay(state));
        }
    }
}
