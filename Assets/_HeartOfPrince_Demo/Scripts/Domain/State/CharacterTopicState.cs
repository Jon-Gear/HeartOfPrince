using System;
using System.Collections.Generic;
using System.Text;

namespace HeartOfPrince.Domain
{
    [Serializable]
    public sealed class CharacterTopicState
    {
        private readonly List<TopicName> _playerToCharacterTopics = new();
        private readonly List<TopicName> _characterToPlayerTopics = new();

        public CharacterID CharacterId { get; }
        public IReadOnlyList<TopicName> PlayerToCharacterTopics => _playerToCharacterTopics;
        public IReadOnlyList<TopicName> CharacterToPlayerTopics => _characterToPlayerTopics;

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

        public bool HasPlayerToCharacterTopic(TopicName topicName) => _playerToCharacterTopics.Contains(topicName);
        public bool HasCharacterToPlayerTopic(TopicName topicName) => _characterToPlayerTopics.Contains(topicName);



        public void AddPlayerToCharacterTopic(TopicName topicName)
        {
            if (!_playerToCharacterTopics.Contains(topicName))
                _playerToCharacterTopics.Add(topicName);
        }

        public void AddCharacterToPlayerTopic(TopicName topicName)
        {
            if (!_characterToPlayerTopics.Contains(topicName))
                _characterToPlayerTopics.Add(topicName);
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
