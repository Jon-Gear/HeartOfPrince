using HeartOfPrince.Domain;
using HeartOfPrince.Infrastructure;
using System;
using UnityEngine;
using Yarn.Unity;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;

namespace HeartOfPrince.Presentation
{
    public class ConversationSceneYarnBridge : MonoBehaviour
    {
        [YarnCommand("StartConversation")]
        public static void StartConversation(string actorId)
        {
            GameSession.Instance.Conversation.StartConversation((CharacterID)actorId);
        }

        [YarnCommand("EndConversation")]
        public static void FinishConversation()
        {
            GameSession.Instance.Conversation.EndConversation();
        }
        [YarnCommand("TakeTurn")]
        public static void TakeTurn()
        {
            GameSession.Instance.Conversation.TakeTurn();
        }

        [YarnCommand("CountPlayerTurn")]
        public static void CountPlayerTurn()
        {
            GameSession.Instance.Conversation.CountPlayerTurn();
        }
        
        [YarnCommand("CountCurrentActorTurn")]
        public static void CountCurrentActorTurn()
        {
            GameSession.Instance.Conversation.CountCurrentActorTurn();
        }

        [YarnFunction("AmountOfPlayerTurns")]
        public static int AmountOfPlayerTurns()
        {
            return GameSession.Instance.Conversation.AmountOfTurnsPlayerUsed;
        }
        
        [YarnFunction("AmountOfCurrentActorTurns")]
        public static int AmountOfCurrentActorTurns()
        {
            return GameSession.Instance.Conversation.AmountOfTurnsCurrentActorUsed;
        }

        

        [YarnFunction("IsCurrentActor")]
        public static bool IsCurrentCharacter(string actorId)
        {
            return GameSession.Instance.Conversation.IsCurrentActor(actorId);
        }

        [YarnFunction("GetCurrentActor")]
        public static string GetCurrentCharacter()
        {
            return GameSession.Instance.Conversation.GetCurrentActor();
        }

        [YarnCommand("PrepareTopics")]
        public static void PrepareTopics(string actorId, string direction, int amount)
        {
            if (!TryParseDirection(direction, out ConversationTopicDirection parsedDirection))
                return;

            GameSession.Instance.Conversation.PrepareTopics((CharacterID)actorId, parsedDirection, amount);
        }

        [YarnFunction("HasPreparedTopic")]
        public static bool HasPreparedTopic(int index)
        {
            return GameSession.Instance.Conversation.HasPreparedTopic(index);
        }

        [YarnFunction("SelectTopic")]
        public static string SelectTopic(int index)
        {
            return GameSession.Instance.Conversation.SelectTopic(index) ?? "None";
        }
        
        [YarnFunction("SelectRandomTopic")]
        public static string SelectRandomTopic()
        {
            return GameSession.Instance.Conversation.SelectRandomTopic() ?? "None";
        }

        [YarnFunction("TopicDisplayName")]
        public static string GetPreparedTopicDisplayName(int index)
        {
            return GameSession.Instance.Conversation.GetPreparedDisplayName(index) ?? "...";
        }

        [YarnFunction("HasTopicsForCurrentActor")]
        public static bool HasTopicsForCurrentCharacter(string direction)
        {
            if (!TryParseDirection(direction, out ConversationTopicDirection parsedDirection))
                return false;
            return GameSession.Instance.Conversation.HasTopicsForCurrentActor(parsedDirection);
        }

        [YarnFunction("CanRefreshPreparedTopics")]
        public static bool CanRefreshPreparedTopics()
        {
            return GameSession.Instance.Conversation.CanRefreshPreparedTopics();
        }

        [YarnFunction("TurnsLeft")]
        public static int GetTurnsLeft()
        {
            return GameSession.Instance.Conversation.TurnsLeft;
        }
        
        


        private static bool TryParseDirection(string raw, out ConversationTopicDirection direction)
        {
            return Enum.TryParse(raw, ignoreCase: true, out direction)
                   && direction != ConversationTopicDirection.None;
        }
    }
}
