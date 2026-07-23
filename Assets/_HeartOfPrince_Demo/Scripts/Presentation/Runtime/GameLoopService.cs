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
        private const string PonderMorningScene = "Ponder_Morning";
        private const string PonderEveningScene = "Ponder_Evening";
        private const string MunirMorningScene = "Conversation_Munir_Morning";
        private const string MunirEveningScene = "Conversation_Munir_Evening";

        [Header("Narrative")]
        [SerializeField]
        private Chapter startingChapter;


        [Header("Demo Configuration")]
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
        private Chapter currentChapter;
        private bool isSceneLoadInProgress;
        private bool standaloneSceneMode;
        private bool standaloneUsesEveningVariant;

        public Chapter CurrentChapterDefinition
        {
            get
            {
                //EnsureChapterDefinition();
                return currentChapter;
            }
        }

        private Act CurrentActDefinition
        {
            get
            {
                //EnsureChapterDefinition();

                int actIndex = Math.Max(0, CurrentAct - 1);
                actIndex = Math.Min(actIndex, currentChapter.ActCount - 1);
                return currentChapter.GetAct(actIndex);
            }
        }

        public GameLoopPhase Phase => loopState?.Phase ?? GameLoopPhase.None;
        public int CurrentAct => loopState?.CurrentAct ?? 0;
        public int CurrentDay => loopState?.CurrentDay ?? 0;
        public int CurrentDecisionIndex => loopState?.CurrentDecisionIndex ?? 0;
        public int DecisionsAllowedPerDay =>
            loopState?.DecisionsAllowedPerDay ?? CurrentActDefinition.DecisionsPerDay;
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
            //EnsureChapterDefinition();
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
            loopState.DecisionsAllowedPerDay = CurrentActDefinition.DecisionsPerDay;

            SetPhase(GameLoopPhase.PlayingDayOpening);
            yield return LoadSceneRoutine(CurrentActDefinition.DayStartScene);
        }

        private IEnumerator BeginDecisionRoutine()
        {
            if (CurrentDecisionIndex >= DecisionsAllowedPerDay)
            {
                yield return BeginEndOfDayRoutine();
                yield break;
            }

            SetPhase(GameLoopPhase.AwaitingDecision);
            yield return LoadSceneRoutine(
                CurrentActDefinition.GetDecisionScene(CurrentDecisionIndex));
        }

        private IEnumerator BeginEndOfDayRoutine()
        {
            loopState.IsDayEnding = true;
            loopState.IsActionRunning = false;
            loopState.CurrentAction = GameLoopAction.None;
            loopState.CurrentTalkCharacterId = null;

            SetPhase(GameLoopPhase.EndingDay);
            yield return LoadSceneRoutine(CurrentActDefinition.DayEndScene);
        }

        public void DecisionLoop()
        {
            ReplaceTransition(ContinueAfterDialogueRoutine(BeginDecisionRoutine()));
        }

        public void CompleteDay()
        {
            ReplaceTransition(ContinueAfterDialogueRoutine(AdvanceAfterDayRoutine()));
        }

        public void CompleteChapterStart()
        {
            if (standaloneSceneMode)
            {
                ReplaceTransition(CompleteStandaloneSequenceAfterDialogueRoutine());
                return;
            }

            ReplaceTransition(ContinueAfterDialogueRoutine(BeginActRoutine()));
        }

        public void CompleteActStart()
        {
            if (standaloneSceneMode)
            {
                ReplaceTransition(CompleteStandaloneSequenceAfterDialogueRoutine());
                return;
            }

            ReplaceTransition(ContinueAfterDialogueRoutine(BeginDayRoutine()));
        }

        public void CompleteAct()
        {
            if (standaloneSceneMode)
            {
                ReplaceTransition(CompleteStandaloneSequenceAfterDialogueRoutine());
                return;
            }

            ReplaceTransition(ContinueAfterDialogueRoutine(AdvanceAfterActRoutine()));
        }

        public void CompleteChapter()
        {
            if (standaloneSceneMode)
            {
                ReplaceTransition(CompleteStandaloneSequenceAfterDialogueRoutine());
                return;
            }

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
                case GameLoopPhase.StartingGame:
                    ReplaceTransition(
                        ContinueAfterDialogueRoutine(BeginActRoutine()));
                    break;

                case GameLoopPhase.StartingAct:
                    ReplaceTransition(
                        ContinueAfterDialogueRoutine(BeginDayRoutine()));
                    break;

                case GameLoopPhase.PlayingDayOpening:
                    ReplaceTransition(
                        ContinueAfterDialogueRoutine(BeginDecisionRoutine()));
                    break;

                case GameLoopPhase.EndingDay:
                    ReplaceTransition(
                        ContinueAfterDialogueRoutine(AdvanceAfterDayRoutine()));
                    break;

                case GameLoopPhase.TransitioningAct:
                    ReplaceTransition(
                        ContinueAfterDialogueRoutine(AdvanceAfterActRoutine()));
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

            //EnsureChapterDefinition();
            loopState.Reset(currentChapter.GetAct(0).DecisionsPerDay);
            //SeedPrototypeProgression();

            SetPhase(GameLoopPhase.StartingGame);
            Log($"Starting {currentChapter.DisplayName}.");
            yield return LoadSceneRoutine(currentChapter.StartScene);
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

            //EnsureChapterDefinition();
            loopState.Reset(currentChapter.GetAct(0).DecisionsPerDay);
            //SeedPrototypeProgression();
            //SeedStandaloneDebugTopics();

            if (standaloneUsesEveningVariant && DecisionsAllowedPerDay > 1)
            {
                loopState.CurrentDecisionIndex = DecisionsAllowedPerDay - 1;
            }

            if (string.Equals(
                    sceneName,
                    currentChapter.StartScene,
                    StringComparison.OrdinalIgnoreCase))
            {
                SetPhase(GameLoopPhase.StartingGame);
            }
            else if (string.Equals(
                         sceneName,
                         CurrentActDefinition.StartScene,
                         StringComparison.OrdinalIgnoreCase))
            {
                SetPhase(GameLoopPhase.StartingAct);
            }
            else if (string.Equals(
                         sceneName,
                         CurrentActDefinition.DayStartScene,
                         StringComparison.OrdinalIgnoreCase))
            {
                SetPhase(GameLoopPhase.PlayingDayOpening);
            }
            else if (string.Equals(
                         sceneName,
                         CurrentActDefinition.DayEndScene,
                         StringComparison.OrdinalIgnoreCase))
            {
                loopState.IsDayEnding = true;
                SetPhase(GameLoopPhase.EndingDay);
            }
            else if (IsDecisionScene(sceneName))
            {
                SetPhase(GameLoopPhase.AwaitingDecision);
            }
            else if (string.Equals(
                         sceneName,
                         CurrentActDefinition.EndScene,
                         StringComparison.OrdinalIgnoreCase))
            {
                SetPhase(GameLoopPhase.TransitioningAct);
            }
            else if (string.Equals(
                         sceneName,
                         currentChapter.EndScene,
                         StringComparison.OrdinalIgnoreCase))
            {
                SetPhase(GameLoopPhase.PlayingEnding);
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

            var progress = CreateNarrativeProgress(
                completedActsInChapter: CurrentAct - 1);

            if (CurrentActDefinition.IsComplete(progress))
            {
                SetPhase(GameLoopPhase.TransitioningAct);
                yield return LoadSceneRoutine(CurrentActDefinition.EndScene);
                yield break;
            }

            loopState.CurrentDay++;
            yield return BeginDayRoutine();
        }

        private IEnumerator BeginActRoutine()
        {
            loopState.CurrentDay = 1;
            loopState.CurrentDecisionIndex = 0;
            loopState.DecisionsAllowedPerDay = CurrentActDefinition.DecisionsPerDay;
            loopState.IsActionRunning = false;
            loopState.IsDayEnding = false;
            loopState.CurrentAction = GameLoopAction.None;
            loopState.CurrentTalkCharacterId = null;

            SetPhase(GameLoopPhase.StartingAct);
            yield return LoadSceneRoutine(CurrentActDefinition.StartScene);
        }

        private IEnumerator AdvanceAfterActRoutine()
        {
            var chapterProgress = CreateNarrativeProgress(
                completedActsInChapter: CurrentAct);

            if (currentChapter.IsComplete(chapterProgress))
            {
                yield return BeginEndingRoutine();
                yield break;
            }

            if (CurrentAct >= currentChapter.ActCount)
            {
                Debug.LogError(
                    $"[GameLoop] Chapter '{currentChapter.Id}' has no next act, " +
                    "but its completion condition is not met.");
                yield break;
            }

            loopState.CurrentAct++;
            yield return BeginActRoutine();
        }

        private IEnumerator BeginEndingRoutine()
        {
            loopState.IsGameComplete = false;
            loopState.IsActionRunning = false;
            loopState.IsDayEnding = false;
            loopState.CurrentAction = GameLoopAction.None;
            loopState.CurrentTalkCharacterId = null;

            SetPhase(GameLoopPhase.PlayingEnding);
            yield return LoadSceneRoutine(currentChapter.EndScene);
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

            yield return AdvanceAfterDayRoutine();
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
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogError("[GameLoop] Cannot load an empty narrative scene name.");
                yield break;
            }

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

        private void EnsureChapterDefinition()
        {
            if (currentChapter != null)
            {
                return;
            }

            currentChapter = startingChapter;

            if (currentChapter == null)
            {
                throw new InvalidOperationException(
                    "GameLoopService has no Starting Chapter assigned.");
            }

            if (currentChapter.ActCount == 0)
            {
                throw new InvalidOperationException(
                    $"Chapter '{currentChapter.name}' contains no acts.");
            }
        }

        private NarrativeProgress CreateNarrativeProgress(
            int completedActsInChapter)
        {
            return new NarrativeProgress(
                state: GameSession.Instance?.State,
                completedDaysInAct: CurrentDay,
                completedActsInChapter: completedActsInChapter,
                totalActsInChapter: currentChapter.ActCount);
        }

        private bool IsDecisionScene(string sceneName)
        {
            for (int i = 0; i < CurrentActDefinition.DecisionsPerDay; i++)
            {
                if (string.Equals(
                        sceneName,
                        CurrentActDefinition.GetDecisionScene(i),
                        StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
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

        //private void SeedPrototypeProgression()
        //{
        //    var munir = (CharacterID)"Munir";
        //    var topics = GameSession.Instance.State.GetOrCreateCharacterTopics(munir);

        //    topics.AddTopic(
        //        (TopicName)"PrototypeAskAboutResponsibility",
        //        ConversationTopicDirection.PlayerToCharacter);

        //    GameSession.Instance.State.Ponder.AddTopic(
        //        (TopicName)"PrototypeQuietMoment");

        //    GameSession.Instance.State.GetOrCreateRelationship(munir);
        //}

        //private void SeedStandaloneDebugTopics()
        //{
        //    var munir = (CharacterID)"Munir";
        //    var topics = GameSession.Instance.State.GetOrCreateCharacterTopics(munir);

        //    foreach (var topic in StandalonePlayerTopics)
        //    {
        //        topics.AddTopic(
        //            (TopicName)topic,
        //            ConversationTopicDirection.PlayerToCharacter);
        //    }

        //    foreach (var topic in StandaloneNpcTopics)
        //    {
        //        topics.AddTopic(
        //            (TopicName)topic,
        //            ConversationTopicDirection.CharacterToPlayer);
        //    }

        //    foreach (var topic in StandalonePonderTopics)
        //    {
        //        GameSession.Instance.State.Ponder.AddTopic((TopicName)topic);
        //    }
        //}

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
