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
