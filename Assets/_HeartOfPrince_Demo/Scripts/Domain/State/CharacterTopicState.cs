using System;
using System.Collections.Generic;

namespace HeartOfPrince.Domain
{
    [Serializable]
    public sealed class CharacterTopicState
    {
        private readonly List<TopicName> _playerToCharacterTopics = new();
        private readonly List<TopicName> _characterToPlayerTopics = new();
        private readonly List<TopicName> _discussedPlayerToCharacterTopics = new();
        private readonly List<TopicName> _discussedCharacterToPlayerTopics = new();

        public CharacterID CharacterId { get; }
        public IReadOnlyList<TopicName> PlayerToCharacterTopics => _playerToCharacterTopics;
        public IReadOnlyList<TopicName> CharacterToPlayerTopics => _characterToPlayerTopics;
        public IReadOnlyList<TopicName> DiscussedPlayerToCharacterTopics => _discussedPlayerToCharacterTopics;
        public IReadOnlyList<TopicName> DiscussedCharacterToPlayerTopics => _discussedCharacterToPlayerTopics;

        public CharacterTopicState(CharacterID characterId)
        {
            CharacterId = characterId;
        }

        public IReadOnlyList<TopicName> GetTopics(ConversationTopicDirection direction)
        {
            return direction switch
            {
                ConversationTopicDirection.CharacterToPlayer => CharacterToPlayerTopics,
                ConversationTopicDirection.PlayerToCharacter => PlayerToCharacterTopics,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }

        public IReadOnlyList<TopicName> GetDiscussedTopics(ConversationTopicDirection direction)
        {
            return direction switch
            {
                ConversationTopicDirection.CharacterToPlayer => DiscussedCharacterToPlayerTopics,
                ConversationTopicDirection.PlayerToCharacter => DiscussedPlayerToCharacterTopics,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }

        public bool HasTopic(TopicName topicName)
        {
            return HasCharacterToPlayerTopic(topicName) || HasPlayerToCharacterTopic(topicName);
        }

        public bool HasTopic(TopicName topicName, ConversationTopicDirection direction)
        {
            return direction switch
            {
                ConversationTopicDirection.CharacterToPlayer => HasCharacterToPlayerTopic(topicName),
                ConversationTopicDirection.PlayerToCharacter => HasPlayerToCharacterTopic(topicName),
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }

        public bool HasDiscussedTopic(TopicName topicName, ConversationTopicDirection direction)
        {
            return direction switch
            {
                ConversationTopicDirection.CharacterToPlayer => _discussedCharacterToPlayerTopics.Contains(topicName),
                ConversationTopicDirection.PlayerToCharacter => _discussedPlayerToCharacterTopics.Contains(topicName),
                _ => false
            };
        }

        public void AddTopic(TopicName topicName, ConversationTopicDirection direction)
        {
            switch (direction)
            {
                case ConversationTopicDirection.CharacterToPlayer:
                    AddCharacterToPlayerTopic(topicName);
                    break;
                case ConversationTopicDirection.PlayerToCharacter:
                    AddPlayerToCharacterTopic(topicName);
                    break;
            }
        }

        public void RemoveTopic(TopicName topicName, ConversationTopicDirection direction)
        {
            switch (direction)
            {
                case ConversationTopicDirection.CharacterToPlayer:
                    RemoveCharacterToPlayerTopic(topicName);
                    break;
                case ConversationTopicDirection.PlayerToCharacter:
                    RemovePlayerToCharacterTopic(topicName);
                    break;
            }
        }

        public void MarkDiscussed(TopicName topicName, ConversationTopicDirection direction)
        {
            RemoveTopic(topicName, direction);
            var discussed = direction == ConversationTopicDirection.PlayerToCharacter
                ? _discussedPlayerToCharacterTopics
                : _discussedCharacterToPlayerTopics;

            if (!discussed.Contains(topicName))
            {
                discussed.Add(topicName);
            }
        }

        public bool HasPlayerToCharacterTopic(TopicName topicName) => _playerToCharacterTopics.Contains(topicName);
        public bool HasCharacterToPlayerTopic(TopicName topicName) => _characterToPlayerTopics.Contains(topicName);

        public void AddPlayerToCharacterTopic(TopicName topicName)
        {
            if (!_playerToCharacterTopics.Contains(topicName) &&
                !_discussedPlayerToCharacterTopics.Contains(topicName))
            {
                _playerToCharacterTopics.Add(topicName);
            }
        }

        public void AddCharacterToPlayerTopic(TopicName topicName)
        {
            if (!_characterToPlayerTopics.Contains(topicName) &&
                !_discussedCharacterToPlayerTopics.Contains(topicName))
            {
                _characterToPlayerTopics.Add(topicName);
            }
        }

        public void RemovePlayerToCharacterTopic(TopicName topicName)
        {
            _playerToCharacterTopics.Remove(topicName);
        }

        public void RemoveCharacterToPlayerTopic(TopicName topicName)
        {
            _characterToPlayerTopics.Remove(topicName);
        }
    }
}
