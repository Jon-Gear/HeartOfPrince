using System;
using HeartOfPrince.Application;
using HeartOfPrince.Domain;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeartOfPrince.Presentation
{
    [DefaultExecutionOrder(-1000)]
    public sealed class GameSession : MonoBehaviour
    {
        private const string DefaultConfigurationResource =
            "HeartOfPrince/GameConfiguration";

        private const string DefaultActivityModulesResource =
            "HeartOfPrince/ActivityModules";

        public static GameSession Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private GameConfiguration configuration;

        [Header("Initial State")]
        [SerializeField] private GameStateDebugPreset initialStatePreset;

        public GameConfiguration Configuration { get; private set; }
        public GameState State { get; private set; }
        public ConversationService Conversation { get; private set; }
        public PonderService Ponder { get; private set; }
        public ActivityService Activities { get; private set; }
        public ActivityQueryService ActivityQuery { get; private set; }
        public ActivityModuleRegistry ActivityModules { get; private set; }
        public GameLoopService GameLoop { get; private set; }
        public ActivityCatalog ActiveActivityCatalog { get; private set; }

        [RuntimeInitializeOnLoadMethod(
            RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegisterDirectSceneBootstrap()
        {
            SceneManager.sceneLoaded -= EnsureSessionAfterSceneLoad;
            SceneManager.sceneLoaded += EnsureSessionAfterSceneLoad;
        }

        [RuntimeInitializeOnLoadMethod(
            RuntimeInitializeLoadType.AfterSceneLoad)]
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

            string sceneName = SceneManager.GetActiveScene().name;

            if (!IsHeartOfPrinceRuntimeScene(sceneName))
            {
                return;
            }

            GameSession sceneSession =
                UnityEngine.Object.FindObjectOfType<GameSession>();

            if (sceneSession != null)
            {
                return;
            }

            var runtimeObject =
                new GameObject("GameSession [Direct Scene Debug]");

            runtimeObject.AddComponent<GameSession>();
        }

        private static bool IsHeartOfPrinceRuntimeScene(
            string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                return false;
            }

            GameConfiguration gameConfiguration =
                Resources.Load<GameConfiguration>(
                    DefaultConfigurationResource);

            if (gameConfiguration == null)
            {
                return false;
            }

            bool isBootstrap = string.Equals(
                sceneName,
                gameConfiguration.BootstrapScene,
                StringComparison.OrdinalIgnoreCase);

            bool isNarrativeScene =
                gameConfiguration.StartingChapter != null &&
                gameConfiguration.StartingChapter
                    .ContainsScene(sceneName);

            bool isActivityScene =
                gameConfiguration
                    .FindActivityForScene(sceneName) != null;

            return isBootstrap ||
                   isNarrativeScene ||
                   isActivityScene;
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

            ResolveConfiguration();
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

        public void ApplyActivityCatalog(ActivityCatalog catalog)
        {
            ActiveActivityCatalog =
                catalog ?? Configuration.ActivityCatalog;

            if (ActiveActivityCatalog == null)
            {
                return;
            }

            Activities.ResetRegistrations();
            ActivityQuery.ClearProviders();
            ActivityModules.Configure(
                ActiveActivityCatalog,
                Activities,
                ActivityQuery);
        }

        private void ResolveConfiguration()
        {
            Configuration = configuration != null
                ? configuration
                : Resources.Load<GameConfiguration>(
                    DefaultConfigurationResource);

            if (Configuration == null)
            {
                throw new InvalidOperationException(
                    "No Heart of Prince GameConfiguration is assigned " +
                    "and Resources/HeartOfPrince/GameConfiguration.asset " +
                    "could not be loaded.");
            }

            if (Configuration.StartingChapter == null)
            {
                throw new InvalidOperationException(
                    "GameConfiguration has no Starting Chapter.");
            }

            if (Configuration.ActivityCatalog == null ||
                Configuration.ActivityCatalog.DayRules == null)
            {
                throw new InvalidOperationException(
                    "GameConfiguration has no valid Activity Catalog " +
                    "and Day Rules.");
            }
        }

        private void BuildRuntime()
        {
            GameStateDebugPreset preset =
                initialStatePreset != null
                    ? initialStatePreset
                    : Configuration.InitialStatePreset;

            State = preset != null
                ? preset.CreateGameState()
                : new GameState();

            Conversation = new ConversationService(State);
            Ponder = new PonderService(State);

            ActivityCatalog catalog =
                Configuration.ActivityCatalog;

            Activities = new ActivityService(
                State,
                catalog.DayRules);

            ActivityQuery =
                new ActivityQueryService(Activities);

            ActivityRuntimeModule[] modules =
                Resources.LoadAll<ActivityRuntimeModule>(
                    DefaultActivityModulesResource);

            ActivityModules =
                new ActivityModuleRegistry(modules);

            ApplyActivityCatalog(catalog);
        }

        private void EnsureGameLoopService()
        {
            GameLoop = GetComponent<GameLoopService>();

            if (GameLoop == null)
            {
                GameLoop =
                    gameObject.AddComponent<GameLoopService>();
            }
        }

#if UNITY_EDITOR
        public void Editor_ApplyPreset(
            GameStateDebugPreset preset)
        {
            if (preset == null)
            {
                Debug.LogWarning(
                    "Cannot apply null GameStateDebugPreset.");
                return;
            }

            initialStatePreset = preset;
            BuildRuntime();
            GameLoop?.BindToCurrentState();
        }
#endif
    }
}
