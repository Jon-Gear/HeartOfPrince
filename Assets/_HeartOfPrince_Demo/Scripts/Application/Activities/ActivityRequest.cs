using System;
using HeartOfPrince.Domain;

namespace HeartOfPrince.Application
{
    public interface IActivityRequest
    {
        ActivityDefinition Activity { get; }
        IActivityInput Input { get; }
    }

    public sealed class ActivityRequest<TInput> : IActivityRequest
        where TInput : class, IActivityInput
    {
        public ActivityDefinition Activity { get; }
        public TInput TypedInput { get; }

        IActivityInput IActivityRequest.Input => TypedInput;

        public ActivityRequest(
            ActivityDefinition activity,
            TInput input)
        {
            Activity = activity != null
                ? activity
                : throw new ArgumentNullException(nameof(activity));

            TypedInput = input
                ?? throw new ArgumentNullException(nameof(input));
        }
    }
}
