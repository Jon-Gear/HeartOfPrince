using System;
using System.Collections.Generic;
using System.Text;

namespace HeartOfPrince.Domain
{
    public sealed class MonologueTopicState
    {
        private readonly List<TopicName> _monologueTopics = new();
        public IReadOnlyList<TopicName> MonologueTopics => _monologueTopics;
        
        public bool HasTopic(TopicName topicName) => _monologueTopics.Contains(topicName);
        
        public void AddTopic(TopicName topicName)
        {
            if (!_monologueTopics.Contains(topicName))
            {
                _monologueTopics.Add(topicName);
            }
        }

        public void RemoveTopic(TopicName topicName)
        {
            _monologueTopics.Remove(topicName);
        }

    }
}