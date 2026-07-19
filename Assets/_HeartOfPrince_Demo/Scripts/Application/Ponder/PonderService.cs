using HeartOfPrince.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Random = System.Random;
using UnityEngine;

namespace HeartOfPrince.Application
{
    /// <summary>
    /// Runs Prince's private pondering flow. Unlike ConversationService, this service
    /// has no current actor and no topic direction: every topic belongs to Prince.
    /// </summary>
    public sealed class PonderService
    {
        private const int AmountOfTurns = 3;

        private readonly GameState _gameState;
        private readonly Random _random = new();

        private TopicName? _currentTopic;
        private readonly List<TopicName> _preparedTopics = new();

        private bool _isPondering;
        private bool _canRefreshPreparedTopics;
        private int _turnsLeft;
        private int _amountOfTurnsUsed;

        public bool IsPondering => _isPondering;
        public int TurnsLeft => _turnsLeft;
        public int AmountOfTurnsUsed => _amountOfTurnsUsed;

        public PonderService(GameState gameState)
        {
            _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
        }

        public void StartPonder()
        {
            _isPondering = true;
            _currentTopic = null;
            _preparedTopics.Clear();
            _canRefreshPreparedTopics = false;
            _turnsLeft = AmountOfTurns;
            _amountOfTurnsUsed = 0;
        }

        public void EndPonder()
        {
            _isPondering = false;
            _currentTopic = null;
            _preparedTopics.Clear();
            _canRefreshPreparedTopics = false;
            _turnsLeft = 0;
        }

        public bool HasTopics()
        {
            return _gameState.Ponder.Topics.Count > 0;
        }

        public void PrepareTopics(int amount)
        {
            if (amount <= 0)
            {
                amount = 3;
            }

            var selectedTopics = _gameState.Ponder.Topics.ToList();
            Shuffle(selectedTopics);

            

            _preparedTopics.Clear();
            _preparedTopics.AddRange(selectedTopics.Take(amount));
            _canRefreshPreparedTopics = selectedTopics.Count > amount;
        }

        public bool HasPreparedTopic(int index)
        {
            return index >= 0 && index < _preparedTopics.Count;
        }

        public string SelectTopic(int index)
        {
            if (!HasPreparedTopic(index))
            {
                return string.Empty;
            }

            ConsumeTopic(index);
            return _currentTopic?.Value ?? string.Empty;
        }

        public string SelectRandomTopic()
        {
            if (_preparedTopics.Count == 0)
            {
                return string.Empty;
            }

            int randomIndex = _random.Next(0, _preparedTopics.Count);
            ConsumeTopic(randomIndex);
            return _currentTopic?.Value ?? string.Empty;
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

        public void TakeTurn()
        {
            if (_turnsLeft > 0)
            {
                _turnsLeft--;
            }
        }

        public void CountTurn()
        {
            _amountOfTurnsUsed++;
        }

        private void ConsumeTopic(int index)
        {
            _currentTopic = _preparedTopics[index];
            _gameState.Ponder.RemoveTopic(_currentTopic.Value);

            if (_turnsLeft > 0)
            {
                _turnsLeft--;
            }
        }

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
