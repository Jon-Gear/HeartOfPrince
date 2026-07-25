using System;
using System.Collections.Generic;

namespace HeartOfPrince.Domain
{
    [Serializable]
    public sealed class GameState
    {
        public Dictionary<CharacterID, CharacterTopicState> CharactersTopics = new();
        public Dictionary<CharacterID, CharacterRelationshipState> CharacterRelationships = new();
        public HashSet<string> Flags = new(StringComparer.OrdinalIgnoreCase);
        public PonderTopicState Ponder = new();
        public GameLoopState Loop = new();
        public WorldClockState Clock = new();
        public DayActivityState Day = new();

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

        public bool HasFlag(string flag)
        {
            return !string.IsNullOrWhiteSpace(flag) && Flags.Contains(flag.Trim());
        }

        public void SetFlag(string flag)
        {
            if (!string.IsNullOrWhiteSpace(flag))
            {
                Flags.Add(flag.Trim());
            }
        }

        public void ClearFlag(string flag)
        {
            if (!string.IsNullOrWhiteSpace(flag))
            {
                Flags.Remove(flag.Trim());
            }
        }
    }
}
