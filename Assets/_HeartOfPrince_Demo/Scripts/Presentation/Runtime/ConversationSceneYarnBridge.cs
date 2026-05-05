using HeartOfPrince.Domain;
using HeartOfPrince.Infrastructure;
using System;
using UnityEngine;
using Yarn.Unity;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;

namespace HeartOfPrince.Presentation
{
    public static class ConversationSceneYarnBridge
    {
        

        [YarnFunction("CurrentActor")]
        public static bool CurrentCharacter(string actorId)
        {
            return GameSession.Instance.Conversation.CurrentCharacter(actorId);
        }

        [YarnFunction("GetCurrentActor")]
        public static string GetCurrentCharacter()
        {
            return GameSession.Instance.Conversation.GetCurrentCharacter();
        }

        [YarnFunction("GetCurrentTopic")]
        public static string GetCurrentTopic()
        {
            return GameSession.Instance.Conversation.GetCurrentTopic();
        }

        [YarnCommand("SetCurrentTopic")]
        public static void SetCurrentTopic(string topicName)
        {
            if (string.IsNullOrWhiteSpace(topicName))
                return;

            GameSession.Instance.Conversation.SetCurrentTopic((TopicName)topicName);
        }

        [YarnCommand("PrepareTopicChoices")]
        public static void PrepareTopicChoices(string actorId, string direction, int amount)
        {
            if (!TryParseDirection(direction, out ConversationTopicDirection parsedDirection))
                return;

            GameSession.Instance.Conversation.Prepare((CharacterID)actorId, parsedDirection, amount);
        }

        [YarnFunction("HasPreparedTopic")]
        public static bool HasPreparedTopic(int index)
        {
            return GameSession.Instance.Conversation.HasPreparedTopic(index);
        }

        [YarnFunction("GetPreparedTopicName")]
        public static string GetPreparedTopicName(int index)
        {
            return GameSession.Instance.Conversation.GetPreparedTopicName(index) ?? string.Empty;
        }

        [YarnFunction("GetPreparedTopicDisplayName")]
        public static string GetPreparedTopicDisplayName(int index)
        {
            return GameSession.Instance.Conversation.GetPreparedDisplayName(index) ?? "...";
        }

        [YarnFunction("HasTopicsForCurrentCharacter")]
        public static bool HasTopicsForCurrentCharacter(string direction)
        {
            if (!GameSession.Instance.Conversation.HasCharacter())
                return false;

            if (!TryParseDirection(direction, out ConversationTopicDirection parsedDirection))
                return false;

            return GameSession.Instance.Conversation.HasAnyTopic(parsedDirection);
        }

        //[YarnCommand("PlayPreparedTopic")]
        //public static void PlayPreparedTopic(int index)
        //{
            
        //    if (!GameSession.Instance.Conversation.HasPreparedTopic(index))
        //        return;

        //    TopicName topic = GameSession.Instance.Conversation.GetPreparedTopic(index);
        //    GameSession.Instance.Conversation.SetCurrentTopic(topic);
        //    _nodePlayer.PlayNode(topic.TopicName.Value);
        //}

        private static bool TryParseDirection(string raw, out ConversationTopicDirection direction)
        {
            return Enum.TryParse(raw, ignoreCase: true, out direction)
                   && direction != ConversationTopicDirection.None;
        }
    }
}
