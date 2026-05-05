using System;
using System.Collections.Generic;
using System.Text;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;

namespace HeartOfPrince.Domain
{
    public class ConversationState
    {
        public CharacterID? CurrentCharacterID { get; private set; }
        public TopicName? CurrentTopic { get; private set; }

        public ConversationTopicDirection? ConversationTopicDirection { get; private set; }

        public bool ConversationActive = false;

        public void SetCurrentCharacter(CharacterID characterID)
        {
            CurrentCharacterID = characterID;
        }

        public void SetTopicName(TopicName topicName)
        {
            CurrentTopic = topicName;
        }

        public void SetTopicDirection(ConversationTopicDirection direction)
        {
            ConversationTopicDirection = direction;
        }

        public void ClearCurrentCharacter()
        {
            CurrentCharacterID = null;
        }
        
        public void ClearCurrentTopic()
        {
            CurrentTopic = null;
        }

        public void ClearTopicDirection()
        {
            ConversationTopicDirection = null;
        }

        public void StartConversation(CharacterID characterID)
        {
            SetCurrentCharacter(characterID);
            ConversationActive = true;
        }

        public void EndConversation()
        {
            ClearCurrentCharacter();
            ClearCurrentTopic();
            ConversationActive = false;
        }

    }
}
