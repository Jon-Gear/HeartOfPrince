using System.Collections.Generic;
using UnityEngine;

namespace HeartOfPrince.Domain
{
    public sealed class GameState
    {


        private readonly Dictionary<CharacterID, CharacterTopicState> _charactersTopics = new();

        public IReadOnlyDictionary<CharacterID, CharacterTopicState> CharactersTopics => _charactersTopics;

    }
}
