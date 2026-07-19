using HeartOfPrince.Application;
using HeartOfPrince.Domain;
using UnityEngine;

namespace HeartOfPrince.Presentation
{
    public sealed class GameSession : MonoBehaviour
    {
        public static GameSession Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private GameStateDebugPreset initialStatePreset;
        public GameState State { get; private set; }

        public ConversationService Conversation { get; private set; }
        public PonderService Ponder { get; private set; }
        public ExplorationService Exploration { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            BuildRuntime();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void BuildRuntime()
        {
            BuildStateRuntime();
            BuildServices();
        }

        private void BuildStateRuntime()
        {
            State = initialStatePreset != null
                ? initialStatePreset.CreateGameState()
                : new GameState();
        }

        private void BuildServices()
        {
            Conversation = new ConversationService(State);
            Ponder = new PonderService(State);
            Exploration = new ExplorationService(State);
        }

#if UNITY_EDITOR
        public void Editor_ApplyPreset(GameStateDebugPreset preset)
        {
            if (preset == null)
            {
                Debug.LogWarning("Cannot apply null GameStateDebugPreset.");
                return;
            }

            State = preset.CreateGameState();
            BuildServices();
        }
#endif
    }
}
