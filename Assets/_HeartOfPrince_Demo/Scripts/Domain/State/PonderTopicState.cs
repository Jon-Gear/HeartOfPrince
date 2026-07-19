using System;
using System.Collections.Generic;

namespace HeartOfPrince.Domain
{
    [Serializable]
    public sealed class PonderTopicState
    {
        private readonly List<TopicName> _topics = new();

        public IReadOnlyList<TopicName> Topics => _topics;

        public bool HasTopic(TopicName topicName) => _topics.Contains(topicName);

        public void AddTopic(TopicName topicName)
        {
            if (!_topics.Contains(topicName))
            {
                _topics.Add(topicName);
            }
        }

        public void RemoveTopic(TopicName topicName)
        {
            _topics.Remove(topicName);
        }
    }
}
