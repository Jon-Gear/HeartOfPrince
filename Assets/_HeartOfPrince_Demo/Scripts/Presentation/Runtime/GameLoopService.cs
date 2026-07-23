using HeartOfPrince.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using Yarn.Unity;

namespace HeartOfPrince.Presentation
{
    /// <summary>
    /// Central state machine for the complete game, day, and decision loops.
    ///
    /// Every narrative scene owns its own DialogueRunner. The runner's configured
    /// starting node begins that scene's dialogue and is destroyed with the scene.
    /// This service persists only loop state and coordinates scene transitions.
    /// </summary>
    [DefaultExecutionOrder(-900)]
    [DisallowMultipleComponent]
    public sealed class GameLoopService : MonoBehaviour
    {
        [Serializable]
        private sealed class TalkActionRoute
        {
            [SerializeField] private string characterId;
            [SerializeField] private string morningSceneName;
            [SerializeField] private string eveningSceneName;

            public string CharacterId => characterId;

            public TalkActionRoute()
            {
            }

            public TalkActionRoute(
                string characterId,
                string morningSceneName,
                string eveningSceneName)
            {
                this.characterId = characterId;
                this.morningSceneName = morningSceneName;
                this.eveningSceneName = eveningSceneName;
            }

            public string GetSceneName(bool useMorningVariant)
            {
                var preferred = useMorningVariant ? morningSceneName : eveningSceneName;
                var fallback = useMorningVariant ? eveningSceneName : morningSceneName;
                return !string.IsNullOrWhiteSpace(preferred) ? preferred : fallback;
            }
        }

        public static GameLoopService Instance { get; private set; }

        private const string BootstrapScene = "Bootstrap";
        private const string DayStartScene = "Day_Start";
        private const string DayEndScene = "Day_End";
        private const string PonderMorningScene = "Ponder_Morning";
        private const string PonderEveningScene = "Ponder_Evening";
        private const string DecisionMorningScene = "Decision_Morning";
        private const string DecisionEveningScene = "Decision_Evening";
        private const string MunirMorningScene = "Conversation_Munir_Morning";
        private const string MunirEveningScene = "Conversation_Munir_Evening";

        private static readonly string[] StandalonePlayerTopics =
        {
            "PlaceholderTopic7",
            "PlaceholderTopic8",
            "PlaceholderTopic9",
            "PlaceholderTopic10",
            "PlaceholderTopic11",
            "PlaceholderTopic12"
        };

        private static readonly string[] StandaloneNpcTopics =
        {
            "PlaceholderTopic1",
            "PlaceholderTopic2",
            "PlaceholderTopic3",
            "PlaceholderTopic4",
            "PlaceholderTopic5",
            "PlaceholderTopic6"
        };

        private static readonly string[] StandalonePonderTopics =
        {
            "ReflectOnDuty",
            "ReflectOnFamily",
            "ReflectOnFaith",
            "ReflectOnFuture",
            "ReflectOnFear",
            "ReflectOnMercy"
        };

        [Header("Demo Configuration")]
        [SerializeField, Min(1)] private int decisionsPerDay = 2;
        [SerializeField, Min(1)] private int daysPerAct = 2;
        [SerializeField, Min(1)] private int actsInDemo = 2;
        [SerializeField] private bool startAutomatically = true;
        [SerializeField] private List<TalkActionRoute> talkRoutes = new()
        {
            new TalkActionRoute("Munir", MunirMorningScene, MunirEveningScene)
        };

        [Header("Diagnostics")]
        [SerializeField] private bool logTransitions = true;
        [SerializeField] private GameLoopPhase inspectorPhase;
        [SerializeField] private int inspectorCurrentAct;
        [SerializeField] private int inspectorCurrentDay;
        [SerializeField] private int inspectorDecisionIndex;
        [SerializeField] private bool inspectorActionRunning;
        [SerializeField] private bool inspectorDayEnding;
        [SerializeField] private bool inspectorGameComplete;
        [SerializeField] private bool inspectorStandaloneSceneMode;
        [SerializeField] private string inspectorActiveScene;

        private Coroutine activeTransition;
        private GameLoopState loopState;
        private bool isSceneLoadInProgress;
        private bool standaloneSceneMode;
        private bool standaloneUsesEveningVariant;

        public GameLoopPhase Phase => loopState?.Phase ?? GameLoopPhase.None;
        public int CurrentAct => loopState?.CurrentAct ?? 0;
        public int CurrentDay => loopState?.CurrentDay ?? 0;
        public int CurrentDecisionIndex => loopState?.CurrentDecisionIndex ?? 0;
        public int DecisionsAllowedPerDay => loopState?.DecisionsAllowedPerDay ?? decisionsPerDay;
        public bool IsActionRunning => loopState?.IsActionRunning ?? false;
        public bool IsDayEnding => loopState?.IsDayEnding ?? false;
        public bool IsGameComplete => loopState?.IsGameComplete ?? false;
        public bool IsStandaloneSceneMode => standaloneSceneMode;

        private bool UseMorningVariant =>
            standaloneSceneMode ? !standaloneUsesEveningVariant : IsMorningSlot;

        private bool IsMorningSlot =>
            CurrentDecisionIndex < Mathf.CeilToInt(DecisionsAllowedPerDay * 0.5f);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            BindToCurrentState();
        }

        private void Start()
        {
            if (!startAutomatically || Phase != GameLoopPhase.None)
            {
                return;
            }

            var activeSceneName = SceneManager.GetActiveScene().name;
            if (string.Equals(activeSceneName, BootstrapScene, StringComparison.OrdinalIgnoreCase))
            {
                StartNewGame();
            }
            else
            {
                StartStandaloneScene(activeSceneName);
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void BindToCurrentState()
        {
            loopState = GameSession.Instance?.State?.Loop;
            SyncInspector();
        }

        [ContextMenu("Start New Game")]
        public void StartNewGame()
        {
            standaloneSceneMode = false;
            standaloneUsesEveningVariant = false;
            ReplaceTransition(StartNewGameRoutine());
        }

        [ContextMenu("Reset All Progression")]
        public void ResetAllProgression()
        {
            if (standaloneSceneMode)
            {
                ReplaceTransition(ResetStandaloneSceneRoutine());
                return;
            }

            StartNewGame();
        }

        [ContextMenu("Skip To Next Day")]
        public void DebugSkipToNextDay()
        {
            if (standaloneSceneMode)
            {
                Debug.LogWarning(
                    "[GameLoop] Skip To Next Day is disabled in standalone-scene mode. " +
                    "Use Start New Game to enter the complete loop.");
                return;
            }

            if (Phase == GameLoopPhase.Completed)
            {
                Debug.LogWarning("[GameLoop] Cannot skip: the demo is already complete.");
                return;
            }

            ReplaceTransition(DebugSkipToNextDayRoutine());
        }


        #region Gameplay Verbs
        public void RequestPonder()
        {
            loopState.CurrentAction = GameLoopAction.Ponder;
            loopState.IsActionRunning = true;
            SetPhase(GameLoopPhase.LoadingAction);
            ReplaceTransition(BeginPonder());
        }

        private IEnumerator BeginPonder()
        {
            yield return WaitForActiveSceneDialogueToFinish();
            
            SetPhase(GameLoopPhase.PerformingPonder);
            yield return LoadSceneRoutine(UseMorningVariant ? PonderMorningScene : PonderEveningScene);
        }



        public void RequestTalk(string characterId)
        {
            if (string.IsNullOrWhiteSpace(characterId))
            {
                Debug.LogWarning("[GameLoop] Ignored Talk request with an empty character ID.");
                return;
            }

            loopState.CurrentAction = GameLoopAction.Talk;
            loopState.CurrentTalkCharacterId = characterId.Trim();
            loopState.IsActionRunning = true;
            SetPhase(GameLoopPhase.LoadingAction);
            ReplaceTransition(BeginTalk());
        }

        private IEnumerator BeginTalk()
        {
            yield return WaitForActiveSceneDialogueToFinish();

            var route = ResolveTalkRoute(loopState.CurrentTalkCharacterId);
            if (route == null)
            {
                loopState.IsActionRunning = false;
                loopState.CurrentAction = GameLoopAction.None;
                loopState.CurrentTalkCharacterId = null;
                yield return BeginDecisionRoutine();
                yield break;
            }

            var sceneName = route.GetSceneName(UseMorningVariant);
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogError(
                    $"[GameLoop] Talk route for '{route.CharacterId}' has no scene.");
                loopState.IsActionRunning = false;
                loopState.CurrentAction = GameLoopAction.None;
                loopState.CurrentTalkCharacterId = null;
                yield return BeginDecisionRoutine();
                yield break;
            }

            SetPhase(GameLoopPhase.PerformingTalk);
            yield return LoadSceneRoutine(sceneName);
        }

        public void NotifyActionCompleted()
        {
            if (Phase != GameLoopPhase.PerformingTalk &&
                Phase != GameLoopPhase.PerformingPonder)
            {
                Debug.LogWarning($"[GameLoop] Ignored action completion while in {Phase}.");
                return;
            }

            SetPhase(GameLoopPhase.ResolvingAction);
            ReplaceTransition(ResolveAction());
        }

        private IEnumerator ResolveAction()
        {
            yield return WaitForActiveSceneDialogueToFinish();

            loopState.IsActionRunning = false;
            loopState.CurrentAction = GameLoopAction.None;
            loopState.CurrentTalkCharacterId = null;

            if (standaloneSceneMode)
            {
                SetPhase(GameLoopPhase.StandaloneComplete);
                Log("Standalone action completed. No automatic return to Bootstrap.");
                yield break;
            }

            loopState.CurrentDecisionIndex++;
            SyncInspector();

            Log($"Resolved decision {CurrentDecisionIndex}/{DecisionsAllowedPerDay}.");

            if (CurrentDecisionIndex >= DecisionsAllowedPerDay)
            {
                yield return BeginEndOfDayRoutine();
            }
            else
            {
                yield return BeginDecisionRoutine();
            }
        }


        #endregion

        #region Day Loop
        private IEnumerator BeginDayRoutine()
        {
            loopState.IsDayEnding = false;
            loopState.IsActionRunning = false;
            loopState.CurrentAction = GameLoopAction.None;
            loopState.CurrentTalkCharacterId = null;
            loopState.CurrentDecisionIndex = 0;

            SetPhase(GameLoopPhase.PlayingDayOpening);
            yield return LoadSceneRoutine(DayStartScene);
        }

        private IEnumerator BeginDecisionRoutine()
        {
            if (CurrentDecisionIndex >= DecisionsAllowedPerDay)
            {
                yield return BeginEndOfDayRoutine();
                yield break;
            }

            SetPhase(GameLoopPhase.AwaitingDecision);
            var scene = IsMorningSlot ? DecisionMorningScene : DecisionEveningScene;
            yield return LoadSceneRoutine(scene);
        }

        private IEnumerator BeginEndOfDayRoutine()
        {
            loopState.IsDayEnding = true;
            loopState.IsActionRunning = false;
            loopState.CurrentAction = GameLoopAction.None;
            loopState.CurrentTalkCharacterId = null;

            SetPhase(GameLoopPhase.EndingDay);
            yield return LoadSceneRoutine(DayEndScene);
        }

        public void DecisionLoop()
        {
            ReplaceTransition(ContinueAfterDialogueRoutine(BeginDecisionRoutine()));
        }

        public void CompleteDay()
        {
            ReplaceTransition(ContinueAfterDialogueRoutine(AdvanceAfterDayRoutine()));
        }

        public void CompleteAct()
        {
            Debug.Log("Completing act");
        }

        public void CompleteChapter()
        {
            ReplaceTransition(ContinueAfterDialogueRoutine(CompleteGameRoutine()));
        }




        public void NotifySequenceCompleted()
        {
            if (standaloneSceneMode)
            {
                ReplaceTransition(CompleteStandaloneSequenceAfterDialogueRoutine());
                return;
            }

            switch (Phase)
            {
                case GameLoopPhase.PlayingDayOpening:
                    ReplaceTransition(
                        ContinueAfterDialogueRoutine(BeginDecisionRoutine()));
                    break;

                case GameLoopPhase.EndingDay:
                    ReplaceTransition(
                        ContinueAfterDialogueRoutine(AdvanceAfterDayRoutine()));
                    break;

                case GameLoopPhase.TransitioningAct:
                    // The Day_Start scene's own start node presents both the act
                    // transition and the new day's opening in one scene-local run.
                    ReplaceTransition(
                        ContinueAfterDialogueRoutine(BeginDecisionRoutine()));
                    break;

                case GameLoopPhase.PlayingEnding:
                    ReplaceTransition(
                        ContinueAfterDialogueRoutine(CompleteGameRoutine()));
                    break;

                default:
                    Debug.LogWarning(
                        $"[GameLoop] Ignored sequence completion while in {Phase}.");
                    break;
            }
        }

        #endregion







        private IEnumerator StartNewGameRoutine()
        {
            StopActiveSceneDialogue();
            yield return WaitForActiveSceneDialogueToFinish();

            GameSession.Instance.ResetRuntimeState();
            BindToCurrentState();

            loopState.Reset(decisionsPerDay);
            SeedPrototypeProgression();

            SetPhase(GameLoopPhase.StartingGame);
            Log("Starting a new game.");

            SetPhase(GameLoopPhase.StartingAct);
            yield return BeginDayRoutine();
        }

        private void StartStandaloneScene(string sceneName)
        {
            ConfigureStandaloneSceneState(sceneName, resetRuntime: true);

            Log(
                $"Standalone-scene mode: '{sceneName}'. " +
                "The scene-local DialogueRunner will start its configured node.");

            StartCoroutine(ValidateActiveSceneRunnerRoutine(sceneName));
        }

        private void ConfigureStandaloneSceneState(
            string sceneName,
            bool resetRuntime)
        {
            standaloneSceneMode = true;
            standaloneUsesEveningVariant =
                sceneName.IndexOf("Evening", StringComparison.OrdinalIgnoreCase) >= 0;

            if (resetRuntime)
            {
                GameSession.Instance.ResetRuntimeState();
                BindToCurrentState();
            }

            loopState.Reset(decisionsPerDay);
            SeedPrototypeProgression();
            SeedStandaloneDebugTopics();

            if (standaloneUsesEveningVariant && decisionsPerDay > 1)
            {
                loopState.CurrentDecisionIndex = decisionsPerDay - 1;
            }

            if (string.Equals(sceneName, DayStartScene, StringComparison.OrdinalIgnoreCase))
            {
                SetPhase(GameLoopPhase.PlayingDayOpening);
            }
            else if (string.Equals(sceneName, DayEndScene, StringComparison.OrdinalIgnoreCase))
            {
                loopState.IsDayEnding = true;
                SetPhase(GameLoopPhase.EndingDay);
            }
            else if (string.Equals(sceneName, DecisionMorningScene, StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(sceneName, DecisionEveningScene, StringComparison.OrdinalIgnoreCase))
            {
                SetPhase(GameLoopPhase.AwaitingDecision);
            }
            else if (sceneName.StartsWith("Conversation_", StringComparison.OrdinalIgnoreCase))
            {
                loopState.IsActionRunning = true;
                loopState.CurrentAction = GameLoopAction.Talk;
                loopState.CurrentTalkCharacterId = InferTalkCharacter(sceneName);
                SetPhase(GameLoopPhase.PerformingTalk);
            }
            else if (sceneName.StartsWith("Ponder_", StringComparison.OrdinalIgnoreCase))
            {
                loopState.IsActionRunning = true;
                loopState.CurrentAction = GameLoopAction.Ponder;
                SetPhase(GameLoopPhase.PerformingPonder);
            }
            else
            {
                SetPhase(GameLoopPhase.StandaloneScene);
            }
        }

        private IEnumerator ResetStandaloneSceneRoutine()
        {
            var sceneName = SceneManager.GetActiveScene().name;
            StopActiveSceneDialogue();
            yield return WaitForActiveSceneDialogueToFinish();

            GameSession.Instance.ResetRuntimeState();
            BindToCurrentState();
            ConfigureStandaloneSceneState(sceneName, resetRuntime: false);

            isSceneLoadInProgress = true;
            var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            if (operation == null)
            {
                isSceneLoadInProgress = false;
                Debug.LogError($"[GameLoop] Could not reload standalone scene '{sceneName}'.");
                yield break;
            }

            while (!operation.isDone)
            {
                yield return null;
            }

            isSceneLoadInProgress = false;
            SyncInspector();
            yield return ValidateActiveSceneRunnerRoutine(sceneName);
        }

        




        private IEnumerator CompleteStandaloneSequenceAfterDialogueRoutine()
        {
            yield return WaitForActiveSceneDialogueToFinish();
            loopState.IsActionRunning = false;
            loopState.IsDayEnding = false;
            SetPhase(GameLoopPhase.StandaloneComplete);
            Log("Standalone scene sequence completed. The current scene remains loaded.");
        }

        

        private IEnumerator AdvanceAfterDayRoutine()
        {
            loopState.IsDayEnding = false;
            loopState.CurrentDecisionIndex = 0;

            var finishedFinalDay =
                CurrentAct >= actsInDemo && CurrentDay >= daysPerAct;

            if (finishedFinalDay)
            {
                yield return BeginEndingRoutine();
                yield break;
            }

            if (CurrentDay >= daysPerAct)
            {
                loopState.CurrentAct++;
                loopState.CurrentDay = 1;
                SetPhase(GameLoopPhase.TransitioningAct);
                yield return LoadSceneRoutine(DayStartScene);
                yield break;
            }

            loopState.CurrentDay++;
            yield return BeginDayRoutine();
        }

        private IEnumerator BeginEndingRoutine()
        {
            loopState.IsGameComplete = false;
            loopState.IsActionRunning = false;
            loopState.IsDayEnding = false;
            loopState.CurrentAction = GameLoopAction.None;
            loopState.CurrentTalkCharacterId = null;

            SetPhase(GameLoopPhase.PlayingEnding);

            // Reloading gives Day_End a fresh scene-local DialogueRunner. Its fixed
            // start node branches on PlayingEnding and presents the demo ending.
            yield return LoadSceneRoutine(DayEndScene);
        }

        private IEnumerator CompleteGameRoutine()
        {
            loopState.IsGameComplete = true;
            loopState.IsActionRunning = false;
            loopState.IsDayEnding = false;
            loopState.CurrentAction = GameLoopAction.None;
            loopState.CurrentTalkCharacterId = null;
            SetPhase(GameLoopPhase.Completed);
            Log("Demo completed.");
            yield break;
        }

        private IEnumerator DebugSkipToNextDayRoutine()
        {
            StopActiveSceneDialogue();
            yield return WaitForActiveSceneDialogueToFinish();

            loopState.IsActionRunning = false;
            loopState.IsDayEnding = false;
            loopState.CurrentAction = GameLoopAction.None;
            loopState.CurrentTalkCharacterId = null;
            loopState.CurrentDecisionIndex = 0;

            if (CurrentAct >= actsInDemo && CurrentDay >= daysPerAct)
            {
                yield return BeginEndingRoutine();
                yield break;
            }

            if (CurrentDay >= daysPerAct)
            {
                loopState.CurrentAct++;
                loopState.CurrentDay = 1;
                SetPhase(GameLoopPhase.TransitioningAct);
                yield return LoadSceneRoutine(DayStartScene);
            }
            else
            {
                loopState.CurrentDay++;
                yield return BeginDayRoutine();
            }
        }

        private IEnumerator ContinueAfterDialogueRoutine(IEnumerator continuation)
        {
            yield return WaitForActiveSceneDialogueToFinish();

            while (continuation.MoveNext())
            {
                yield return continuation.Current;
            }
        }

        private void StopActiveSceneDialogue()
        {
            var runner = FindActiveSceneDialogueRunner();
            if (runner != null && runner.IsDialogueRunning)
            {
                runner.Stop();
            }
        }

        private IEnumerator WaitForActiveSceneDialogueToFinish()
        {
            var runner = FindActiveSceneDialogueRunner();

            // Yarn commands run before the node itself has completely unwound.
            yield return null;

            while (runner != null && runner.IsDialogueRunning)
            {
                yield return null;
            }
        }

        private IEnumerator LoadSceneRoutine(string sceneName)
        {
            isSceneLoadInProgress = true;
            SyncInspector();
            Log($"Loading scene '{sceneName}'.");

            var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            if (operation == null)
            {
                isSceneLoadInProgress = false;
                Debug.LogError(
                    $"[GameLoop] Could not load scene '{sceneName}'. " +
                    "Use Heart of Prince > Rebuild Demo Scene List or add the scene to Build Settings.");
                yield break;
            }

            while (!operation.isDone)
            {
                yield return null;
            }

            isSceneLoadInProgress = false;
            SyncInspector();

            yield return ValidateActiveSceneRunnerRoutine(sceneName);
        }

        private IEnumerator ValidateActiveSceneRunnerRoutine(string sceneName)
        {
            // Let scene Awake/Start complete so the local runner can auto-start.
            yield return null;

            var runner = FindActiveSceneDialogueRunner();
            if (runner == null)
            {
                Debug.LogError(
                    $"[GameLoop] Scene '{sceneName}' has no active scene-local DialogueRunner.");
                yield break;
            }

            if (!runner.autoStart)
            {
                Debug.LogWarning(
                    $"[GameLoop] DialogueRunner in '{sceneName}' has Auto Start disabled. " +
                    "This architecture expects the scene runner's Starting Node to begin dialogue.");
            }

            Log(
                $"Scene-local Yarn runner '{runner.gameObject.name}' owns '{sceneName}'. " +
                "Its configured Starting Node controls scene dialogue.");
        }

        private DialogueRunner FindActiveSceneDialogueRunner()
        {
            var activeScene = SceneManager.GetActiveScene();

            return FindObjectsOfType<DialogueRunner>(true)
                .FirstOrDefault(runner =>
                    runner != null &&
                    runner.gameObject.scene == activeScene &&
                    runner.gameObject.activeInHierarchy &&
                    runner.enabled);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SyncInspector();
        }

        private TalkActionRoute ResolveTalkRoute(string characterId)
        {
            var route = talkRoutes?.FirstOrDefault(
                item => item != null &&
                        string.Equals(
                            item.CharacterId,
                            characterId,
                            StringComparison.OrdinalIgnoreCase));

            if (route == null)
            {
                Debug.LogError(
                    $"[GameLoop] No Talk route is configured for character '{characterId}'. " +
                    "Add morning/evening scene names to the GameLoopService route list.");
            }

            return route;
        }

        private static string InferTalkCharacter(string sceneName)
        {
            const string prefix = "Conversation_";
            var remainder = sceneName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                ? sceneName.Substring(prefix.Length)
                : sceneName;

            var separator = remainder.IndexOf('_');
            return separator > 0 ? remainder.Substring(0, separator) : remainder;
        }

        private void SeedPrototypeProgression()
        {
            var munir = (CharacterID)"Munir";
            var topics = GameSession.Instance.State.GetOrCreateCharacterTopics(munir);

            topics.AddTopic(
                (TopicName)"PrototypeAskAboutResponsibility",
                ConversationTopicDirection.PlayerToCharacter);

            GameSession.Instance.State.Ponder.AddTopic(
                (TopicName)"PrototypeQuietMoment");

            GameSession.Instance.State.GetOrCreateRelationship(munir);
        }

        private void SeedStandaloneDebugTopics()
        {
            var munir = (CharacterID)"Munir";
            var topics = GameSession.Instance.State.GetOrCreateCharacterTopics(munir);

            foreach (var topic in StandalonePlayerTopics)
            {
                topics.AddTopic(
                    (TopicName)topic,
                    ConversationTopicDirection.PlayerToCharacter);
            }

            foreach (var topic in StandaloneNpcTopics)
            {
                topics.AddTopic(
                    (TopicName)topic,
                    ConversationTopicDirection.CharacterToPlayer);
            }

            foreach (var topic in StandalonePonderTopics)
            {
                GameSession.Instance.State.Ponder.AddTopic((TopicName)topic);
            }
        }

        private void ReplaceTransition(IEnumerator routine)
        {
            if (activeTransition != null)
            {
                StopCoroutine(activeTransition);
            }

            activeTransition = StartCoroutine(RunTransition(routine));
        }

        private IEnumerator RunTransition(IEnumerator routine)
        {
            while (routine.MoveNext())
            {
                yield return routine.Current;
            }

            activeTransition = null;
        }

        private void SetPhase(GameLoopPhase nextPhase)
        {
            if (loopState == null)
            {
                BindToCurrentState();
            }

            var previous = loopState.Phase;
            loopState.Phase = nextPhase;
            SyncInspector();

            if (previous != nextPhase)
            {
                Log(
                    $"Phase: {previous} -> {nextPhase} " +
                    $"(Act {CurrentAct}, Day {CurrentDay}, " +
                    $"Decision {CurrentDecisionIndex}/{DecisionsAllowedPerDay}).");
            }
        }

        private void SyncInspector()
        {
            inspectorPhase = Phase;
            inspectorCurrentAct = CurrentAct;
            inspectorCurrentDay = CurrentDay;
            inspectorDecisionIndex = CurrentDecisionIndex;
            inspectorActionRunning = IsActionRunning;
            inspectorDayEnding = IsDayEnding;
            inspectorGameComplete = IsGameComplete;
            inspectorStandaloneSceneMode = standaloneSceneMode;
            inspectorActiveScene = SceneManager.GetActiveScene().name;
        }

        private void Log(string message)
        {
            if (logTransitions)
            {
                Debug.Log($"[GameLoop] {message}", this);
            }
        }
    }
}
