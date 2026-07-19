using System;
using System.Collections.Generic;

namespace HeartOfPrince.Domain
{
    [Serializable]
    public sealed class PonderTopicState
    {
        private readonly List<TopicName> _topics = new();
        private readonly List<TopicName> _discussedTopics = new();

        public IReadOnlyList<TopicName> Topics => _topics;
        public IReadOnlyList<TopicName> DiscussedTopics => _discussedTopics;

        public bool HasTopic(TopicName topicName) => _topics.Contains(topicName);
        public bool HasDiscussedTopic(TopicName topicName) => _discussedTopics.Contains(topicName);

        public void AddTopic(TopicName topicName)
        {
            if (!_topics.Contains(topicName) && !_discussedTopics.Contains(topicName))
            {
                _topics.Add(topicName);
            }
        }

        public void RemoveTopic(TopicName topicName)
        {
            _topics.Remove(topicName);
        }

        public void MarkDiscussed(TopicName topicName)
        {
            _topics.Remove(topicName);

            if (!_discussedTopics.Contains(topicName))
            {
                _discussedTopics.Add(topicName);
            }
        }
    }
}
