using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeartOfPrince.Domain
{
    [CreateAssetMenu(
        fileName = "GameStateDebugPreset",
        menuName = "Heart of Prince/Debug/Game State Debug Preset")]
    public sealed class GameStateDebugPreset : ScriptableObject
    {
        [Header("Ponder")]
        [SerializeField] private List<string> ponderTopics = new();

        [Header("Characters")]
        [SerializeField] private List<CharacterDebugState> characters = new();

        public IReadOnlyList<string> PonderTopics => ponderTopics;
        public IReadOnlyList<CharacterDebugState> Characters => characters;

        public GameState CreateGameState()
        {
            var gameState = new GameState();

            foreach (var topicNode in ponderTopics)
            {
                if (string.IsNullOrWhiteSpace(topicNode))
                {
                    continue;
                }

                gameState.Ponder.AddTopic(
                    GameStateDebugConversion.ToTopicName(topicNode));
            }

            foreach (var character in characters)
            {
                if (string.IsNullOrWhiteSpace(character.CharacterId))
                {
                    continue;
                }

                var characterId = GameStateDebugConversion.ToCharacterId(character.CharacterId);
                var topicState = new CharacterTopicState(characterId);

                foreach (var topicNode in character.TopicState.PlayerToCharacterTopics)
                {
                    if (string.IsNullOrWhiteSpace(topicNode))
                    {
                        continue;
                    }

                    topicState.AddTopic(
                        GameStateDebugConversion.ToTopicName(topicNode),
                        ConversationTopicDirection.PlayerToCharacter);
                }

                foreach (var topicNode in character.TopicState.CharacterToPlayerTopics)
                {
                    if (string.IsNullOrWhiteSpace(topicNode))
                    {
                        continue;
                    }

                    topicState.AddTopic(
                        GameStateDebugConversion.ToTopicName(topicNode),
                        ConversationTopicDirection.CharacterToPlayer);
                }

                gameState.CharactersTopics[characterId] = topicState;
            }

            return gameState;
        }

#if UNITY_EDITOR
        public List<string> Editor_PonderTopics => ponderTopics;
        public List<CharacterDebugState> Editor_Characters => characters;
#endif
    }

    [Serializable]
    public sealed class CharacterDebugState
    {
        [SerializeField] private string characterId;
        [SerializeField] private CharacterTopicDebugState topicState = new();

        public string CharacterId
        {
            get => characterId;
            set => characterId = value;
        }

        public CharacterTopicDebugState TopicState => topicState;
    }

    [Serializable]
    public sealed class CharacterTopicDebugState
    {
        [SerializeField] private List<string> playerToCharacterTopics = new();
        [SerializeField] private List<string> characterToPlayerTopics = new();

        public List<string> PlayerToCharacterTopics => playerToCharacterTopics;
        public List<string> CharacterToPlayerTopics => characterToPlayerTopics;
    }
}
