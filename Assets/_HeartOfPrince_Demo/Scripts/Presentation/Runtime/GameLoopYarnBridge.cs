using System;
using HeartOfPrince.Domain;
using UnityEngine;
using Yarn.Unity;

namespace HeartOfPrince.Presentation
{
    /// <summary>
    /// Yarn commands and functions that expose only the transitions needed by the loop.
    /// Yarn requests transitions; GameLoopService validates and performs them.
    /// </summary>
    public static class GameLoopYarnBridge
    {
        [YarnCommand("loop_choose_action")]
        public static void ChooseAction(string actionName)
        {
            if (!Enum.TryParse(actionName, true, out GameLoopAction action) ||
                action == GameLoopAction.None)
            {
                Debug.LogError($"[GameLoop] Unknown action '{actionName}'. Expected Talk or Ponder.");
                return;
            }

            GameLoopService.Instance?.RequestAction(action);
        }

        [YarnCommand("loop_choose_talk")]
        public static void ChooseTalkTarget(string characterId)
        {
            GameLoopService.Instance?.RequestTalk(characterId);
        }

        [YarnCommand("loop_action_complete")]
        public static void ActionComplete()
        {
            GameLoopService.Instance?.NotifyActionCompleted();
        }

        [YarnCommand("loop_sequence_complete")]
        public static void SequenceComplete()
        {
            GameLoopService.Instance?.NotifySequenceCompleted();
        }

        [YarnCommand("loop_new_game")]
        public static void NewGame()
        {
            GameLoopService.Instance?.StartNewGame();
        }

        [YarnFunction("loop_current_act")]
        public static int CurrentAct()
        {
            return GameLoopService.Instance?.CurrentAct ?? 0;
        }

        [YarnFunction("loop_current_day")]
        public static int CurrentDay()
        {
            return GameLoopService.Instance?.CurrentDay ?? 0;
        }

        [YarnFunction("loop_decision_number")]
        public static int DecisionNumber()
        {
            return (GameLoopService.Instance?.CurrentDecisionIndex ?? 0) + 1;
        }

        [YarnFunction("LoopDecisionsPerDay")]
        public static int DecisionsPerDay()
        {
            return GameLoopService.Instance?.DecisionsAllowedPerDay ?? 0;
        }

        [YarnFunction("loop_is_complete")]
        public static bool IsComplete()
        {
            return GameLoopService.Instance?.IsGameComplete ?? false;
        }
    }
}
