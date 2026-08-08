using HeartOfPrince.Application;
using Yarn.Unity;

namespace HeartOfPrince.Presentation
{
    public static class GameLoopYarnBridge
    {
        [YarnCommand("StartActivity")]
        public static void StartActivity(
            string activityId,
            string selectionKey)
        {
            GameLoopService.Instance?.RequestActivity(
                activityId,
                selectionKey);
        }

        [YarnCommand("CompleteActivity")]
        public static void CompleteActivity()
        {
            GameLoopService.Instance?
                .NotifyActivityCompleted(
                    ActivityResult.Success());
        }

        [YarnCommand("SetStoryFlag")]
        public static void SetStoryFlag(string flag)
        {
            GameSession.Instance?.State.SetFlag(flag);
        }

        [YarnCommand("ClearStoryFlag")]
        public static void ClearStoryFlag(string flag)
        {
            GameSession.Instance?.State.ClearFlag(flag);
        }

        [YarnCommand("CompleteDayOpening")]
        public static void CompleteDayOpening()
        {
            GameLoopService.Instance?.CompleteDayOpening();
        }

        [YarnCommand("CompleteDay")]
        public static void CompleteDay()
        {
            GameLoopService.Instance?.CompleteDay();
        }

        [YarnCommand("CompleteChapterStart")]
        public static void CompleteChapterStart()
        {
            GameLoopService.Instance?
                .CompleteChapterStart();
        }

        [YarnCommand("CompleteActStart")]
        public static void CompleteActStart()
        {
            GameLoopService.Instance?
                .CompleteActStart();
        }

        [YarnCommand("CompleteAct")]
        public static void CompleteAct()
        {
            GameLoopService.Instance?.CompleteAct();
        }

        [YarnCommand("CompleteChapter")]
        public static void CompleteChapter()
        {
            GameLoopService.Instance?
                .CompleteChapter();
        }

        [YarnFunction("loop_current_act")]
        public static int CurrentAct()
        {
            return GameLoopService.Instance?
                .CurrentAct ?? 0;
        }

        [YarnFunction("loop_current_day")]
        public static int CurrentDay()
        {
            return GameLoopService.Instance?
                .CurrentDay ?? 0;
        }

        [YarnFunction("CurrentTime")]
        public static string CurrentTime()
        {
            return GameLoopService.Instance?
                .CurrentTimeDisplay ?? "00:00";
        }

        [YarnFunction("ActionsCompletedToday")]
        public static int ActionsCompletedToday()
        {
            return GameLoopService.Instance?
                .ActionsCompletedToday ?? 0;
        }

        [YarnFunction("ActionsRemainingToday")]
        public static int ActionsRemainingToday()
        {
            return GameLoopService.Instance?
                .ActionsRemainingToday ?? 0;
        }

        [YarnFunction("MaximumActionsPerDay")]
        public static int MaximumActionsPerDay()
        {
            return GameLoopService.Instance?
                .MaximumActionsPerDay ?? 0;
        }
    }
}
