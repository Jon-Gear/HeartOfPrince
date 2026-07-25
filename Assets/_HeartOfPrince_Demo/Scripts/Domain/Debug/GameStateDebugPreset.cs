using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace HeartOfPrince.Domain
{
    [CreateAssetMenu(
        fileName = "GameStateDebugPreset",
        menuName = "Heart of Prince/Debug/Game State Debug Preset")]
    public sealed class GameStateDebugPreset : ScriptableObject
    {
        [Header("Narrative")]
        [SerializeField] private Chapter startingChapter;
        [SerializeField, Min(1)] private int startingAct = 1;
        [SerializeField, Min(1)] private int startingDay = 1;

        [FormerlySerializedAs("startingDecisionIndex")]
        [SerializeField, Min(0)]
        private int startingActionsCompleted;

        [SerializeField, Range(0, 1439)]
        private int startingMinute = 8 * 60;

        [Header("Ponder")]
        [SerializeField] private List<string> ponderTopics = new();
        [SerializeField] private List<string> discussedPonderTopics = new();

        [Header("Characters")]
        [SerializeField] private List<CharacterDebugState> characters = new();

        public Chapter StartingChapter => startingChapter;
        public int StartingAct => Mathf.Max(1, startingAct);
        public int StartingDay => Mathf.Max(1, startingDay);
        public int StartingActionsCompleted =>
            Mathf.Max(0, startingActionsCompleted);
        public int StartingMinute => Mathf.Clamp(startingMinute, 0, 1439);
        public IReadOnlyList<string> PonderTopics => ponderTopics;
        public IReadOnlyList<string> DiscussedPonderTopics =>
            discussedPonderTopics;
        public IReadOnlyList<CharacterDebugState> Characters =>
            characters;

        public GameState CreateGameState()
        {
            var gameState = new GameState();

            PopulatePonderState(gameState);
            PopulateCharacterState(gameState);
            PopulateLoopState(gameState);

            return gameState;
        }

        private void PopulatePonderState(GameState gameState)
        {
            foreach (string topicNode in ponderTopics)
            {
                if (!string.IsNullOrWhiteSpace(topicNode))
                {
                    gameState.Ponder.AddTopic(
                        GameStateDebugConversion.ToTopicName(topicNode));
                }
            }

            foreach (string topicNode in discussedPonderTopics)
            {
                if (string.IsNullOrWhiteSpace(topicNode))
                {
                    continue;
                }

                TopicName topic =
                    GameStateDebugConversion.ToTopicName(topicNode);

                gameState.Ponder.AddTopic(topic);
                gameState.Ponder.MarkDiscussed(topic);
            }
        }

        private void PopulateCharacterState(GameState gameState)
        {
            foreach (CharacterDebugState character in characters)
            {
                if (character == null ||
                    string.IsNullOrWhiteSpace(character.CharacterId))
                {
                    continue;
                }

                CharacterID characterId =
                    GameStateDebugConversion.ToCharacterId(
                        character.CharacterId);

                var topicState =
                    new CharacterTopicState(characterId);

                AddTopics(
                    topicState,
                    character.TopicState.PlayerToCharacterTopics,
                    ConversationTopicDirection.PlayerToCharacter);

                AddTopics(
                    topicState,
                    character.TopicState.CharacterToPlayerTopics,
                    ConversationTopicDirection.CharacterToPlayer);

                AddDiscussedTopics(
                    topicState,
                    character.TopicState.DiscussedPlayerToCharacterTopics,
                    ConversationTopicDirection.PlayerToCharacter);

                AddDiscussedTopics(
                    topicState,
                    character.TopicState.DiscussedCharacterToPlayerTopics,
                    ConversationTopicDirection.CharacterToPlayer);

                gameState.CharactersTopics[characterId] = topicState;
                gameState.GetOrCreateRelationship(characterId)
                    .ChangeTrust(character.Trust);
            }
        }

        private void PopulateLoopState(GameState gameState)
        {
            gameState.Loop.Chapter = 1;
            gameState.Loop.CurrentAct = StartingAct;
            gameState.Clock.BeginDay(
                StartingDay,
                StartingMinute);
            gameState.Day.ActionsCompleted =
                StartingActionsCompleted;
        }

        private static void AddTopics(
            CharacterTopicState state,
            IEnumerable<string> topics,
            ConversationTopicDirection direction)
        {
            foreach (string topicNode in topics)
            {
                if (!string.IsNullOrWhiteSpace(topicNode))
                {
                    state.AddTopic(
                        GameStateDebugConversion.ToTopicName(topicNode),
                        direction);
                }
            }
        }

        private static void AddDiscussedTopics(
            CharacterTopicState state,
            IEnumerable<string> topics,
            ConversationTopicDirection direction)
        {
            foreach (string topicNode in topics)
            {
                if (string.IsNullOrWhiteSpace(topicNode))
                {
                    continue;
                }

                TopicName topic =
                    GameStateDebugConversion.ToTopicName(topicNode);

                state.AddTopic(topic, direction);
                state.MarkDiscussed(topic, direction);
            }
        }
    }

    [Serializable]
    public sealed class CharacterDebugState
    {
        [SerializeField] private string characterId;
        [SerializeField] private int trust;
        [SerializeField] private CharacterTopicDebugState topicState = new();

        public string CharacterId
        {
            get => characterId;
            set => characterId = value;
        }

        public int Trust
        {
            get => trust;
            set => trust = value;
        }

        public CharacterTopicDebugState TopicState => topicState;
    }

    [Serializable]
    public sealed class CharacterTopicDebugState
    {
        [SerializeField] private List<string> playerToCharacterTopics = new();
        [SerializeField] private List<string> characterToPlayerTopics = new();
        [SerializeField] private List<string> discussedPlayerToCharacterTopics = new();
        [SerializeField] private List<string> discussedCharacterToPlayerTopics = new();

        public List<string> PlayerToCharacterTopics =>
            playerToCharacterTopics;
        public List<string> CharacterToPlayerTopics =>
            characterToPlayerTopics;
        public List<string> DiscussedPlayerToCharacterTopics =>
            discussedPlayerToCharacterTopics;
        public List<string> DiscussedCharacterToPlayerTopics =>
            discussedCharacterToPlayerTopics;
    }
}
