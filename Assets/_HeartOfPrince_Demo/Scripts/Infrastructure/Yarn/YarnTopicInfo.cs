using HeartOfPrince.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartOfPrince.Infrastructure
{
    public sealed class YarnTopicInfo
    {
        public TopicName TopicName { get; }
        public CharacterID CharacterId { get; }
        public ConversationTopicDirection Direction { get; }
        public string DisplayName { get; }

        public YarnTopicInfo(
            TopicName topicName,
            CharacterID characterId,
            ConversationTopicDirection direction,
            string displayName)
        {
            TopicName = topicName;
            CharacterId = characterId;
            Direction = direction;
            DisplayName = displayName;
        }
    }
}
