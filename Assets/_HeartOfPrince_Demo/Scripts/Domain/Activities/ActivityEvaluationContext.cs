using System;

namespace HeartOfPrince.Domain
{
    public sealed class ActivityEvaluationContext
    {
        public GameState State { get; }
        public DayRules DayRules { get; }

        public ActivityEvaluationContext(GameState state, DayRules dayRules)
        {
            State = state ?? throw new ArgumentNullException(nameof(state));
            DayRules = dayRules ?? throw new ArgumentNullException(nameof(dayRules));
        }
    }
}
