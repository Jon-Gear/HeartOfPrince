using System.Collections.Generic;

namespace HeartOfPrince.Domain
{
    public sealed class GameState
    {
        public Dictionary<CharacterID, CharacterTopicState> CharactersTopics = new();
        public PonderTopicState Ponder = new();
    }
}
