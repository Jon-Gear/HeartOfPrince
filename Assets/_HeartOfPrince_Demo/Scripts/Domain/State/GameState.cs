using System.Collections.Generic;

namespace HeartOfPrince.Domain
{
    public sealed class GameState
    {
        public Dictionary<CharacterID, CharacterTopicState> CharactersTopics = new();
        public Dictionary<CharacterID, CharacterRelationshipState> CharacterRelationships = new();
        public PonderTopicState Ponder = new();
        public GameLoopState Loop = new();

        public CharacterTopicState GetOrCreateCharacterTopics(CharacterID characterId)
        {
            if (!CharactersTopics.TryGetValue(characterId, out var topics))
            {
                topics = new CharacterTopicState(characterId);
                CharactersTopics.Add(characterId, topics);
            }

            return topics;
        }

        public CharacterRelationshipState GetOrCreateRelationship(CharacterID characterId)
        {
            if (!CharacterRelationships.TryGetValue(characterId, out var relationship))
            {
                relationship = new CharacterRelationshipState(characterId);
                CharacterRelationships.Add(characterId, relationship);
            }

            return relationship;
        }
    }
}
