using UnityEngine;

namespace HeartOfPrince.Domain
{
    public enum TopicCompletionKind
    {
        Ponder,
        Conversation
    }

    [CreateAssetMenu(
        fileName = "Topic Discussed Condition",
        menuName = "Heart of Prince/Narrative/Conditions/Topic Discussed")]
    public sealed class TopicDiscussedCondition : CompletionCondition
    {
        [SerializeField]
        private TopicCompletionKind kind = TopicCompletionKind.Ponder;

        [SerializeField, Tooltip("Node name of the topic that must be discussed.")]
        private string topicNode;

        [SerializeField, Tooltip("Required only for Conversation topics.")]
        private string characterId;

        [SerializeField]
        private ConversationTopicDirection direction = ConversationTopicDirection.None;

        public TopicCompletionKind Kind => kind;
        public string TopicNode => topicNode;
        public string CharacterId => characterId;
        public ConversationTopicDirection Direction => direction;

        public override bool IsMet(NarrativeProgress progress)
        {
            if (progress.State == null ||
                string.IsNullOrWhiteSpace(topicNode))
            {
                return false;
            }

            var topic = new TopicName(topicNode);

            switch (kind)
            {
                case TopicCompletionKind.Ponder:
                    return progress.State.Ponder.HasDiscussedTopic(topic);

                case TopicCompletionKind.Conversation:
                    return IsConversationTopicDiscussed(
                        progress.State,
                        topic);

                default:
                    return false;
            }
        }

        private bool IsConversationTopicDiscussed(
            GameState state,
            TopicName topic)
        {
            if (string.IsNullOrWhiteSpace(characterId) ||
                direction == ConversationTopicDirection.None)
            {
                return false;
            }

            CharacterTopicState characterTopics = state
                .GetOrCreateCharacterTopics(
                    new CharacterID(characterId));

            return characterTopics.HasDiscussedTopic(
                topic,
                direction);
        }
    }
}
