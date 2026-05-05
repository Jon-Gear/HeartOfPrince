using HeartOfPrince.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace HeartOfPrince.Application
{
    public sealed class ConversationService
    {
        private readonly GameState _gameState;
        private readonly Random _random = new();

        private readonly List<TopicName> _preparedTopics = new();


        public ConversationService(GameState gameState) 
        { 
            _gameState = gameState;
        }

        public void StartConversation(CharacterID characterID)
        {
            _gameState.ConversationState.StartConversation(characterID);
        }

        public void EndConversation()
        {
            _gameState.ConversationState.EndConversation();
        }


        public bool HasCharacter()
        {
            return _gameState.ConversationState.CurrentCharacterID != null;
        }

        public bool CurrentCharacter(string characterID)
        {
            return _gameState.ConversationState.CurrentCharacterID == characterID;
        }

        public string GetCurrentCharacter()
        {
            return _gameState.ConversationState.CurrentCharacterID;
        }

        public string GetCurrentTopic()
        {
            return _gameState.ConversationState.CurrentTopic;
        }

        public void SetCurrentTopic(TopicName topicName)
        {
            _gameState.ConversationState.SetTopicName(topicName);
        }

        public void Prepare(CharacterID characterID, ConversationTopicDirection direction, int amount)
        {
            if(amount <= 0)
            {
                amount = 3;
            }

            IReadOnlyList<TopicName> availableTopics = _gameState.CharactersTopics[characterID].GetTopics(direction);

            List<TopicName> selectedTopics = availableTopics.ToList();

            Shuffle(selectedTopics);

            _preparedTopics.Clear();
            _preparedTopics.AddRange(selectedTopics.Take(amount));

            _gameState.ConversationState.SetCurrentCharacter(characterID);
            _gameState.ConversationState.SetTopicDirection(direction);
        }
        public bool HasPreparedTopic(int index)
        {
            return index >= 0 && index < _preparedTopics.Count;
        }

        public TopicName GetPreparedTopic(int index)
        {
            if (!HasPreparedTopic(index))
                throw new ArgumentOutOfRangeException(nameof(index));

            return _preparedTopics[index];
        }

        public string GetPreparedTopicName(int index)
        {
            return HasPreparedTopic(index) ? _preparedTopics[index].Value : string.Empty;
        }

        public string GetPreparedDisplayName(int index)
        {
            return _preparedTopics[index].Value;
        }

        public bool HasAnyTopic(ConversationTopicDirection direction)
        {
            if(_gameState.ConversationState.CurrentCharacterID == null)
            {
                return false;
            }

            CharacterID currentCharacterID = (CharacterID)_gameState.ConversationState.CurrentCharacterID;

            return _gameState.CharactersTopics[currentCharacterID].GetTopics(direction).Count > 0;
        }



        // Helpers

        private void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }


    }
}
