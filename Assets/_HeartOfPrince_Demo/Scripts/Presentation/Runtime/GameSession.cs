using HeartOfPrince.Application;
using HeartOfPrince.Domain;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        public GameLoopService GameLoop { get; private set; }

        /// <summary>
        /// Makes every demo scene directly playable in the editor.
        ///
        /// Bootstrap already contains a configured GameSession, so this method does
        /// nothing there. When another scene is entered directly, it creates the same
        /// single persistent composition root without redirecting to Bootstrap.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegisterDirectSceneBootstrap()
        {
            SceneManager.sceneLoaded -= EnsureSessionAfterSceneLoad;
            SceneManager.sceneLoaded += EnsureSessionAfterSceneLoad;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureSessionAfterInitialSceneLoad()
        {
            EnsureSessionForDirectScenePlay();
        }

        private static void EnsureSessionAfterSceneLoad(
            Scene scene,
            LoadSceneMode mode)
        {
            EnsureSessionForDirectScenePlay();
        }

        private static void EnsureSessionForDirectScenePlay()
        {
            if (Instance != null)
            {
                return;
            }

            var sceneName = SceneManager.GetActiveScene().name;
            if (!IsHeartOfPrinceRuntimeScene(sceneName))
            {
                return;
            }

            var sceneSession = UnityEngine.Object.FindObjectOfType<GameSession>();
            if (sceneSession != null)
            {
                // Its Awake normally ran before this callback; avoid creating
                // a competing persistent session.
                return;
            }

            var runtimeObject = new GameObject("GameSession [Direct Scene Debug]");
            runtimeObject.AddComponent<GameSession>();
        }

        private static bool IsHeartOfPrinceRuntimeScene(string sceneName)
        {
            return string.Equals(sceneName, "Bootstrap", System.StringComparison.OrdinalIgnoreCase) ||
                   sceneName.StartsWith("Chapter_", System.StringComparison.OrdinalIgnoreCase) ||
                   sceneName.StartsWith("Act_", System.StringComparison.OrdinalIgnoreCase) ||
                   sceneName.StartsWith("Day_", System.StringComparison.OrdinalIgnoreCase) ||
                   sceneName.StartsWith("Decision_", System.StringComparison.OrdinalIgnoreCase) ||
                   sceneName.StartsWith("Conversation_", System.StringComparison.OrdinalIgnoreCase) ||
                   sceneName.StartsWith("Ponder_", System.StringComparison.OrdinalIgnoreCase);
        }

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
            GameLoop?.BindToCurrentState();
        }
#endif
    }
}
