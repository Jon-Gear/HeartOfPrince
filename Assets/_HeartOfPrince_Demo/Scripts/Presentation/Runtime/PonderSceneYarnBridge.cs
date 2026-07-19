using UnityEngine;
using Yarn.Unity;

namespace HeartOfPrince.Presentation
{
    /// <summary>
    /// Yarn-facing API for Prince's private pondering scene.
    /// Command names are intentionally Ponder-specific so they can coexist with
    /// ConversationSceneYarnBridge in the same Yarn project.
    /// </summary>
    public sealed class PonderSceneYarnBridge : MonoBehaviour
    {
        [YarnCommand("StartPonder")]
        public static void StartPonder()
        {
            GameSession.Instance.Ponder.StartPonder();
        }

        [YarnCommand("EndPonder")]
        public static void EndPonder()
        {
            GameSession.Instance.Ponder.EndPonder();
        }

        [YarnCommand("TakePonderTurn")]
        public static void TakePonderTurn()
        {
            GameSession.Instance.Ponder.TakeTurn();
        }

        [YarnCommand("CountPonderTurn")]
        public static void CountPonderTurn()
        {
            GameSession.Instance.Ponder.CountTurn();
        }

        [YarnCommand("PreparePonderTopics")]
        public static void PreparePonderTopics(int amount)
        {
            GameSession.Instance.Ponder.PrepareTopics(amount);
        }

        [YarnFunction("IsPondering")]
        public static bool IsPondering()
        {
            return GameSession.Instance.Ponder.IsPondering;
        }

        [YarnFunction("HasPonderTopics")]
        public static bool HasPonderTopics()
        {
            return GameSession.Instance.Ponder.HasTopics();
        }

        [YarnFunction("HasPreparedPonderTopic")]
        public static bool HasPreparedPonderTopic(int index)
        {
            return GameSession.Instance.Ponder.HasPreparedTopic(index);
        }

        [YarnFunction("SelectPonderTopic")]
        public static string SelectPonderTopic(int index)
        {
            return GameSession.Instance.Ponder.SelectTopic(index) ?? "None";
        }

        [YarnFunction("SelectRandomPonderTopic")]
        public static string SelectRandomPonderTopic()
        {
            return GameSession.Instance.Ponder.SelectRandomTopic() ?? "None";
        }

        [YarnFunction("PonderTopicDisplayName")]
        public static string PonderTopicDisplayName(int index)
        {
            return GameSession.Instance.Ponder.GetPreparedDisplayName(index) ?? "...";
        }

        [YarnFunction("CanRefreshPonderTopics")]
        public static bool CanRefreshPonderTopics()
        {
            return GameSession.Instance.Ponder.CanRefreshPreparedTopics();
        }

        [YarnFunction("PonderTurnsLeft")]
        public static int PonderTurnsLeft()
        {
            return GameSession.Instance.Ponder.TurnsLeft;
        }

        [YarnFunction("AmountOfPonderTurns")]
        public static int AmountOfPonderTurns()
        {
            return GameSession.Instance.Ponder.AmountOfTurnsUsed;
        }
    }
}
