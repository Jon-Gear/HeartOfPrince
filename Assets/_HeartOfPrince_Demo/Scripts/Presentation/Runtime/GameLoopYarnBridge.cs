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

        #region Gameplay Verbs
        [YarnCommand("TalkTo")]
        public static void TalkTo(string characterId)
        {
            GameLoopService.Instance?.RequestTalk(characterId);
        }

        [YarnCommand("Ponder")]
        public static void Ponder()
        {
            GameLoopService.Instance?.RequestPonder();
        }

        [YarnCommand("ActionComplete")]
        public static void ActionComplete()
        {
            GameLoopService.Instance?.NotifyActionCompleted();
        }
        #endregion

        #region Sequences
        [YarnCommand("DecisionLoop")]
        public static void DecisionLoop()
        {
            GameLoopService.Instance?.DecisionLoop();
        }

        [YarnCommand("CompleteDay")]
        public static void CompleteDay()
        {
            GameLoopService.Instance?.CompleteDay();
        }

        [YarnCommand("CompleteAct")]
        public static void CompleteAct()
        {
            GameLoopService.Instance?.CompleteAct();
        }


        [YarnCommand("CompleteChapter")]
        public static void CompleteChapter()
        {
            GameLoopService.Instance?.CompleteChapter();
        }

        #endregion

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

        [YarnFunction("DecisionIndex")]
        public static int DecisionIndex()
        {
            return (GameLoopService.Instance?.CurrentDecisionIndex ?? 0) + 1;
        }

        [YarnFunction("DecisionsPerDay")]
        public static int DecisionsPerDay()
        {
            return GameLoopService.Instance?.DecisionsAllowedPerDay ?? 0;
        }

        
    }
}
