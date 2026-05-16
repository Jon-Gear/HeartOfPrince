using System.Collections.Generic;
using UnityEngine;

namespace HeartOfPrince.Domain
{
    public sealed class GameState
    {
        public Dictionary<CharacterID, CharacterTopicState> CharactersTopics = new();
        public MonologueTopicState MonologueTopics = new();
    }
}
