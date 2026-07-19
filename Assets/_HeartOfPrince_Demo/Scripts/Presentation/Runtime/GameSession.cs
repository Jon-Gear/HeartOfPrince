using HeartOfPrince.Application;
using HeartOfPrince.Domain;
using UnityEngine;

namespace HeartOfPrince.Presentation
{
    /// <summary>
    /// The single persistent runtime composition root for Heart of Prince.
    /// Game state and application services are rebuilt here for new games and debug resets.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public sealed class GameSession : MonoBehaviour
    {
        public static GameSession Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private GameStateDebugPreset initialStatePreset;

        public GameState State { get; private set; }
        public ConversationService Conversation { get; private set; }
        public PonderService Ponder { get; private set; }
        public ExplorationService Exploration { get; private set; }
        public GameLoopService GameLoop { get; private set; }

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
            EnsureGameLoopService();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void ResetRuntimeState()
        {
            BuildRuntime();
            GameLoop?.BindToCurrentState();
        }

        private void BuildRuntime()
        {
            State = initialStatePreset != null
                ? initialStatePreset.CreateGameState()
                : new GameState();

            Conversation = new ConversationService(State);
            Ponder = new PonderService(State);
            Exploration = new ExplorationService(State);
        }

        private void EnsureGameLoopService()
        {
            GameLoop = GetComponent<GameLoopService>();

            if (GameLoop == null)
            {
                GameLoop = gameObject.AddComponent<GameLoopService>();
            }
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
            Conversation = new ConversationService(State);
            Ponder = new PonderService(State);
            Exploration = new ExplorationService(State);
            GameLoop?.BindToCurrentState();
        }
#endif
    }
}
