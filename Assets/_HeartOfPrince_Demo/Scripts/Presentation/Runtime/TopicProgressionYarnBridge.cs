using System;
using HeartOfPrince.Domain;
using UnityEngine;
using Yarn.Unity;

namespace HeartOfPrince.Presentation
{
    /// <summary>
    /// Yarn-facing topic and relationship progression API.
    /// It supports Munir now, but takes character IDs so additional characters can use it later.
    /// </summary>
    public static class TopicProgressionYarnBridge
    {
        [YarnCommand("UnlockPonderTopic")]
        public static void UnlockPonderTopic(string topicNode)
        {
            if (!TryTopic(topicNode, out var topic))
            {
                return;
            }

            GameSession.Instance.State.Ponder.AddTopic(topic);
            Debug.Log($"[Topics] Unlocked Ponder topic '{topicNode}'.");
        }

        [YarnCommand("MarkPonderTopicDiscussed")]
        public static void MarkPonderTopicDiscussed(string topicNode)
        {
            if (!TryTopic(topicNode, out var topic))
            {
                return;
            }

            GameSession.Instance.State.Ponder.MarkDiscussed(topic);
            Debug.Log($"[Topics] Discussed Ponder topic '{topicNode}'.");
        }

        [YarnCommand("UnlockConversationTopic")]
        public static void UnlockConversationTopic(
            string characterId,
            string direction,
            string topicNode)
        {
            if (!TryCharacter(characterId, out var character) ||
                !TryDirection(direction, out var parsedDirection) ||
                !TryTopic(topicNode, out var topic))
            {
                return;
            }

            GameSession.Instance.State
                .GetOrCreateCharacterTopics(character)
                .AddTopic(topic, parsedDirection);

            Debug.Log(
                $"[Topics] Unlocked {parsedDirection} topic '{topicNode}' for '{characterId}'.");
        }

        [YarnCommand("MarkConversationTopicDiscussed")]
        public static void MarkConversationTopicDiscussed(
            string characterId,
            string direction,
            string topicNode)
        {
            if (!TryCharacter(characterId, out var character) ||
                !TryDirection(direction, out var parsedDirection) ||
                !TryTopic(topicNode, out var topic))
            {
                return;
            }

            GameSession.Instance.State
                .GetOrCreateCharacterTopics(character)
                .MarkDiscussed(topic, parsedDirection);

            Debug.Log(
                $"[Topics] Discussed {parsedDirection} topic '{topicNode}' for '{characterId}'.");
        }

        [YarnCommand("AddPlayerToCharacterTopic")]
        public static void AddPlayerToCharacterTopic(string characterId, string topicNode)
        {
            UnlockConversationTopic(characterId, "PlayerToCharacter", topicNode);
        }

        [YarnCommand("AddCharacterToPlayerTopic")]
        public static void AddCharacterToPlayerTopic(string characterId, string topicNode)
        {
            UnlockConversationTopic(characterId, "CharacterToPlayer", topicNode);
        }

        [YarnCommand("RemovePlayerToCharacterTopic")]
        public static void RemovePlayerToCharacterTopic(string characterId, string topicNode)
        {
            RemoveConversationTopic(characterId, "PlayerToCharacter", topicNode);
        }

        [YarnCommand("RemoveCharacterToPlayerTopic")]
        public static void RemoveCharacterToPlayerTopic(string characterId, string topicNode)
        {
            RemoveConversationTopic(characterId, "CharacterToPlayer", topicNode);
        }

        [YarnCommand("ChangeRelationship")]
        public static void ChangeRelationship(string characterId, int trustDelta)
        {
            if (!TryCharacter(characterId, out var character))
            {
                return;
            }

            var relationship = GameSession.Instance.State.GetOrCreateRelationship(character);
            relationship.ChangeTrust(trustDelta);
            Debug.Log(
                $"[Relationship] {characterId} trust changed by {trustDelta}; now {relationship.Trust}.");
        }

        [YarnFunction("HasPonderTopic")]
        public static bool HasPonderTopic(string topicNode)
        {
            return TryTopic(topicNode, out var topic) &&
                   GameSession.Instance.State.Ponder.HasTopic(topic);
        }

        [YarnFunction("HasDiscussedPonderTopic")]
        public static bool HasDiscussedPonderTopic(string topicNode)
        {
            return TryTopic(topicNode, out var topic) &&
                   GameSession.Instance.State.Ponder.HasDiscussedTopic(topic);
        }

        [YarnFunction("HasConversationTopic")]
        public static bool HasConversationTopic(
            string characterId,
            string direction,
            string topicNode)
        {
            return TryCharacter(characterId, out var character) &&
                   TryDirection(direction, out var parsedDirection) &&
                   TryTopic(topicNode, out var topic) &&
                   GameSession.Instance.State
                       .GetOrCreateCharacterTopics(character)
                       .HasTopic(topic, parsedDirection);
        }

        [YarnFunction("HasDiscussedConversationTopic")]
        public static bool HasDiscussedConversationTopic(
            string characterId,
            string direction,
            string topicNode)
        {
            return TryCharacter(characterId, out var character) &&
                   TryDirection(direction, out var parsedDirection) &&
                   TryTopic(topicNode, out var topic) &&
                   GameSession.Instance.State
                       .GetOrCreateCharacterTopics(character)
                       .HasDiscussedTopic(topic, parsedDirection);
        }

        [YarnFunction("RelationshipTrust")]
        public static int RelationshipTrust(string characterId)
        {
            if (!TryCharacter(characterId, out var character))
            {
                return 0;
            }

            return GameSession.Instance.State.GetOrCreateRelationship(character).Trust;
        }

        private static void RemoveConversationTopic(
            string characterId,
            string direction,
            string topicNode)
        {
            if (!TryCharacter(characterId, out var character) ||
                !TryDirection(direction, out var parsedDirection) ||
                !TryTopic(topicNode, out var topic))
            {
                return;
            }

            GameSession.Instance.State
                .GetOrCreateCharacterTopics(character)
                .RemoveTopic(topic, parsedDirection);
        }

        private static bool TryCharacter(string raw, out CharacterID character)
        {
            character = default;

            if (string.IsNullOrWhiteSpace(raw))
            {
                Debug.LogError("[Topics] Character ID cannot be empty.");
                return false;
            }

            character = (CharacterID)raw;
            return true;
        }

        private static bool TryTopic(string raw, out TopicName topic)
        {
            topic = default;

            if (string.IsNullOrWhiteSpace(raw))
            {
                Debug.LogError("[Topics] Topic node cannot be empty.");
                return false;
            }

            topic = (TopicName)raw;
            return true;
        }

        private static bool TryDirection(
            string raw,
            out ConversationTopicDirection direction)
        {
            if (Enum.TryParse(raw, true, out direction) &&
                direction != ConversationTopicDirection.None)
            {
                return true;
            }

            Debug.LogError(
                $"[Topics] Unknown direction '{raw}'. " +
                "Expected PlayerToCharacter or CharacterToPlayer.");
            return false;
        }
    }
}
