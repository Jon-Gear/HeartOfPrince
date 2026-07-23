using HeartOfPrince.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = System.Random;

namespace HeartOfPrince.Application
{
    public sealed class ConversationService
    {
        private readonly GameState _gameState;
        private readonly Random _random = new();

        private const int AMOUNT_OF_TURNS = 3;
        
        // Current Actor
        private CharacterID? _currentCharacterID;
        public bool HasActor() => _currentCharacterID != null;
        public bool IsCurrentActor(CharacterID characterID) => _currentCharacterID == characterID;
        public bool IsCurrentActor(string  characterID) => _currentCharacterID == characterID;
        public void SetCurrentActor(CharacterID characterID) => _currentCharacterID = characterID;
        public CharacterID GetCurrentActor() => _currentCharacterID ?? throw new InvalidOperationException("No current character is set.");
        
        
        // Current Topic
        private TopicName? _currentTopic;
        private ConversationTopicDirection _currentConversationTopicDirection;

        private int _turnsLeft;
        public int TurnsLeft => _turnsLeft;
        public void TakeTurn()
        {
            if (_turnsLeft > 0)
            {
                _turnsLeft--;
            }
        }
        
        private readonly List<TopicName> _preparedTopics = new();
        private bool _canRefreshPreparedTopics;
        public bool HasPreparedTopic(int index) => index >= 0 && index < _preparedTopics.Count;
        
        // Miscellaneous Variables
        private int _amountOfTurnsPlayerUsed = 0;
        public  int AmountOfTurnsPlayerUsed => _amountOfTurnsPlayerUsed;
        public void CountPlayerTurn() => _amountOfTurnsPlayerUsed++;
        
        private int _amountOfTurnsCurrentActorUsed = 0;
        public int AmountOfTurnsCurrentActorUsed => _amountOfTurnsCurrentActorUsed;
        public void CountCurrentActorTurn() => _amountOfTurnsCurrentActorUsed++;
        
        public ConversationService(GameState gameState) 
        { 
            _gameState = gameState;
        }
        
        public void StartConversation(CharacterID characterId)
        {
            _currentCharacterID = characterId;
            _currentTopic = null;
            _currentConversationTopicDirection = ConversationTopicDirection.None;

            _preparedTopics.Clear();
            _canRefreshPreparedTopics = false;
            _amountOfTurnsPlayerUsed = 0;
            _amountOfTurnsCurrentActorUsed = 0;
            _turnsLeft = AMOUNT_OF_TURNS;
        }

        public void EndConversation()
        {
            _currentCharacterID = null;
            _currentTopic = null;
            _currentConversationTopicDirection = ConversationTopicDirection.None;

            _preparedTopics.Clear();
            _canRefreshPreparedTopics = false;
        }

        public bool HasTopicsForCurrentActor(ConversationTopicDirection direction)
        {
            if (!HasActor())
            {
                return false;
            }
            else
            {
                return _gameState.GetOrCreateCharacterTopics(GetCurrentActor())
                    .GetTopics(direction)
                    .Count > 0;
            }
        }
        
        public void PrepareTopics(CharacterID characterID, ConversationTopicDirection direction, int amount)
        {
            if(amount <= 0)
            {
                amount = 3;
            }
            

            IReadOnlyList<TopicName> availableTopics = _gameState
                .GetOrCreateCharacterTopics(characterID)
                .GetTopics(direction);

            List<TopicName> prototypeTopics = availableTopics
                .Where(topic => topic.Value.StartsWith("Prototype", StringComparison.Ordinal))
                .ToList();

            List<TopicName> regularTopics = availableTopics
                .Where(topic => !topic.Value.StartsWith("Prototype", StringComparison.Ordinal))
                .ToList();

            Shuffle(prototypeTopics);
            Shuffle(regularTopics);

            List<TopicName> selectedTopics = prototypeTopics
                .Concat(regularTopics)
                .ToList();

            _preparedTopics.Clear();
            _preparedTopics.AddRange(selectedTopics.Take(amount));
            _canRefreshPreparedTopics = selectedTopics.Count > amount;
            _currentConversationTopicDirection = direction;
        }

        public string SelectTopic(int index)
        {
            if (!HasPreparedTopic(index)) return string.Empty;
            ConsumeTopic(index);
            return  _currentTopic.Value;
        }

        public string SelectRandomTopic()
        {
           if (_preparedTopics.Count == 0)
           {
               return string.Empty;
           }

           int prototypeIndex = _preparedTopics.FindIndex(
               topic => topic.Value.StartsWith("Prototype", StringComparison.Ordinal));

           int selectedIndex = prototypeIndex >= 0
               ? prototypeIndex
               : _random.Next(0, _preparedTopics.Count);

           ConsumeTopic(selectedIndex);
           return _currentTopic.Value;
        }

        private void ConsumeTopic(int index)
        {
            _currentTopic = _preparedTopics[index];
            _gameState.GetOrCreateCharacterTopics(GetCurrentActor())
                .MarkDiscussed(_currentTopic.Value, _currentConversationTopicDirection);

            if (_turnsLeft > 0)
            {
                _turnsLeft--;
            }
        }

        
        
        

        public string GetPreparedDisplayName(int index)
        {
            if (!HasPreparedTopic(index))
            {
                return string.Empty;
            }

            string raw = _preparedTopics[index].Value;
            return Regex.Replace(raw, @"([a-z])([A-Z0-9])", "$1 $2");
        }

        

        public bool CanRefreshPreparedTopics()
        {
            return _canRefreshPreparedTopics;
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
