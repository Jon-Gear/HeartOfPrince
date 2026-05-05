using HeartOfPrince.Domain;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

namespace HeartOfPrince.Infrastructure
{
    // not important now
    public sealed class YarnTopicCatalog
    {
        private YarnProject _yarnProject;
        private readonly Dictionary<TopicName, YarnTopicInfo> _topicsByName = new();

        public YarnTopicCatalog(YarnProject yarnProject)
        {
            _yarnProject = yarnProject;

         
        }


        private void RegisterAllTopics()
        {

        }


    }
}
