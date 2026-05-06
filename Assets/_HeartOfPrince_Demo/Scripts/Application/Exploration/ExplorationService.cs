using HeartOfPrince.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = System.Random;

namespace HeartOfPrince.Application
{
    public sealed class ExplorationService
    {
        private readonly GameState _gameState;

        private const int AMOUNT_OF_TURNS = 3;

        private int _turnsLeft;

        public ExplorationService(GameState gameState)
        {
            _gameState = gameState;
        }
        
        
    }
}