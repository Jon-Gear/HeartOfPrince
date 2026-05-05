using System.Collections.Generic;
using UnityEngine;

namespace HeartOfPrince.Domain
{
    public sealed class GameState
    {
        public ConversationState ConversationState { get; set; }
        public Dictionary<CharacterID, CharacterTopicState> CharactersTopics = new();


    }
}
