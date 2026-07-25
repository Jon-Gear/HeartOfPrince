using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeartOfPrince.Application;
using HeartOfPrince.Domain;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

namespace HeartOfPrince.Presentation
{
    [DefaultExecutionOrder(-900)]
    [DisallowMultipleComponent]
    public sealed class GameLoopService : MonoBehaviour
    {
        public static GameLoopService Instance { get; private set; }

        [Header("Flow")]
        [SerializeField] private bool startAutomatically = true;

        [Header("Diagnostics")]
        [SerializeField] private bool logTransitions = true;
        [SerializeField] private GameLoopPhase inspectorPhase;
        [SerializeField] private int inspectorCurrentAct;
        [SerializeField] private int inspectorCurrentDay;
        [SerializeField] private int inspectorMinuteOfDay;
        [SerializeField] private int inspectorActionsCompleted;
        [SerializeField] private string inspectorCurrentActivity;
        [SerializeField] private bool inspectorDayEnding;
        [SerializeField] private bool inspectorGameComplete;
        [SerializeField] private bool inspectorStandaloneSceneMode;
        [SerializeField] private string inspectorActiveScene;

        private readonly SceneTransitionService sceneTransitions = new();

        private Coroutine activeTransition;
        private GameState state;
        private GameLoopState loopState;
        private Chapter currentChapter;
        private bool standaloneSceneMode;
        private bool isSceneLoadInProgress;

        public GameLoopPhase Phase =>
            loopState?.Phase ?? GameLoopPhase.None;

        public int CurrentAct =>
            loopState?.CurrentAct ?? 0;

        public int CurrentDay =>
            state?.Clock?.Day ?? 0;

        public int ActionsCompletedToday =>
            state?.Day?.ActionsCompleted ?? 0;

        public int MaximumActionsPerDay =>
            CurrentDayRules?.MaximumActions ?? 0;

        public int ActionsRemainingToday
        {
            get
            {
                int maximum = MaximumActionsPerDay;

                return maximum <= 0
                    ? -1
                    : Mathf.Max(
                        0,
                        maximum - ActionsCompletedToday);
            }
        }

        public bool IsActionRunning =>
            state?.Day?.HasRunningActivity ?? false;

        public bool IsDayEnding =>
            loopState?.IsDayEnding ?? false;

        public bool IsGameComplete =>
            loopState?.IsGameComplete ?? false;

        public bool IsStandaloneSceneMode =>
            standaloneSceneMode;

        public string CurrentTimeDisplay
        {
            get
            {
                int minute =
                    state?.Clock?.NormalizedMinuteOfDay ?? 0;
                int hour = minute / 60;
                int minutes = minute % 60;
                return $"{hour:00}:{minutes:00}";
            }
        }

        public Chapter CurrentChapterDefinition =>
            currentChapter;

        private Act CurrentActDefinition
        {
            get
            {
                EnsureConfiguration();

                int index = Mathf.Clamp(
                    CurrentAct - 1,
                    0,
                    currentChapter.ActCount - 1);

                return currentChapter.GetAct(index);
            }
        }

        private DayRules CurrentDayRules
        {
            get
            {
                if (currentChapter == null ||
                    currentChapter.ActCount == 0)
                {
                    return GameSession.Instance?
                        .Configuration?
                        .ActivityCatalog?
                        .DayRules;
                }

                DayRules actRules =
                    CurrentActDefinition.DayRules;

                return actRules != null
                    ? actRules
                    : GameSession.Instance.Configuration
                        .ActivityCatalog.DayRules;
            }
        }

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
            if (!startAutomatically ||
                Phase != GameLoopPhase.None)
            {
                return;
            }

            string sceneName =
                SceneManager.GetActiveScene().name;

            if (string.Equals(
                    sceneName,
                    GameSession.Instance.Configuration.BootstrapScene,
                    StringComparison.OrdinalIgnoreCase))
            {
                StartNewGame();
            }
            else
            {
                StartStandaloneScene(sceneName);
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
            GameSession session = GameSession.Instance;

            state = session?.State;
            loopState = state?.Loop;
            currentChapter =
                session?.Configuration?.StartingChapter;

            if (CurrentDayRules != null &&
                session?.Activities != null)
            {
                session.Activities.SetDayRules(
                    CurrentDayRules);
            }

            SyncInspector();
        }

        [ContextMenu("Start New Game")]
        public void StartNewGame()
        {
            standaloneSceneMode = false;
            ReplaceTransition(StartNewGameRoutine());
        }

        [ContextMenu("Reset All Progression")]
        public void ResetAllProgression()
        {
            if (standaloneSceneMode)
            {
                ReplaceTransition(
                    ResetStandaloneSceneRoutine());
                return;
            }

            StartNewGame();
        }

        [ContextMenu("Skip To Next Day")]
        public void DebugSkipToNextDay()
        {
            if (standaloneSceneMode ||
                Phase == GameLoopPhase.Completed)
            {
                return;
            }

            ReplaceTransition(
                DebugSkipToNextDayRoutine());
        }

        public void RequestActivity(ActivityOption option)
        {
            if (option == null)
            {
                Debug.LogWarning(
                    "[GameLoop] The requested activity option " +
                    "could not be found.");
                return;
            }

            if (!option.IsAvailable)
            {
                Debug.LogWarning(
                    $"[GameLoop] '{option.DisplayName}' is unavailable: " +
                    option.UnavailableReason);
                return;
            }

            RequestActivity(option.Request);
        }

        public void RequestActivity(IActivityRequest request)
        {
            if (request == null)
            {
                Debug.LogWarning(
                    "[GameLoop] Ignored a null activity request.");
                return;
            }

            if (Phase != GameLoopPhase.AwaitingDecision &&
                Phase != GameLoopPhase.StandaloneScene)
            {
                Debug.LogWarning(
                    $"[GameLoop] Cannot start an activity while in {Phase}.");
                return;
            }

            SetPhase(GameLoopPhase.LoadingActivity);
            ReplaceTransition(
                BeginActivityRoutine(request));
        }

        public void RequestActivity<TInput>(
            ActivityDefinition activity,
            TInput input)
            where TInput : class, IActivityInput
        {
            RequestActivity(
                new ActivityRequest<TInput>(
                    activity,
                    input));
        }

        public void RequestActivity(
            string activityId,
            IActivityInput input)
        {
            ActivityOption option =
                GameSession.Instance.ActivityQuery.FindOption(
                    activityId,
                    input);

            RequestActivity(option);
        }

        public void RequestActivity(
            string activityId,
            string selectionKey)
        {
            ActivityOption option =
                GameSession.Instance.ActivityQuery.FindOption(
                    activityId,
                    selectionKey);

            RequestActivity(option);
        }

        public void NotifyActivityCompleted(
            ActivityResult result = null)
        {
            if (state?.Day?.CurrentActivity == null)
            {
                Debug.LogWarning(
                    $"[GameLoop] Ignored activity completion " +
                    $"while in {Phase}; no activity is running.");
                return;
            }

            SetPhase(GameLoopPhase.ResolvingActivity);
            ReplaceTransition(
                ResolveActivityRoutine(
                    result ?? ActivityResult.Success()));
        }

        public void CompleteDayOpening()
        {
            ReplaceTransition(
                ContinueAfterDialogueRoutine(
                    BeginDecisionRoutine()));
        }

        public void CompleteDay()
        {
            ReplaceTransition(
                ContinueAfterDialogueRoutine(
                    AdvanceAfterDayRoutine()));
        }

        public void CompleteChapterStart()
        {
            if (standaloneSceneMode)
            {
                ReplaceTransition(
                    CompleteStandaloneSequenceAfterDialogueRoutine());
                return;
            }

            ReplaceTransition(
                ContinueAfterDialogueRoutine(
                    BeginActRoutine()));
        }

        public void CompleteActStart()
        {
            if (standaloneSceneMode)
            {
                ReplaceTransition(
                    CompleteStandaloneSequenceAfterDialogueRoutine());
                return;
            }

            ReplaceTransition(
                ContinueAfterDialogueRoutine(
                    BeginDayRoutine(1)));
        }

        public void CompleteAct()
        {
            if (standaloneSceneMode)
            {
                ReplaceTransition(
                    CompleteStandaloneSequenceAfterDialogueRoutine());
                return;
            }

            ReplaceTransition(
                ContinueAfterDialogueRoutine(
                    AdvanceAfterActRoutine()));
        }

        public void CompleteChapter()
        {
            if (standaloneSceneMode)
            {
                ReplaceTransition(
                    CompleteStandaloneSequenceAfterDialogueRoutine());
                return;
            }

            ReplaceTransition(
                ContinueAfterDialogueRoutine(
                    CompleteGameRoutine()));
        }

        public void NotifySequenceCompleted()
        {
            if (standaloneSceneMode)
            {
                ReplaceTransition(
                    CompleteStandaloneSequenceAfterDialogueRoutine());
                return;
            }

            IEnumerator continuation = Phase switch
            {
                GameLoopPhase.StartingGame =>
                    BeginActRoutine(),
                GameLoopPhase.StartingAct =>
                    BeginDayRoutine(1),
                GameLoopPhase.PlayingDayOpening =>
                    BeginDecisionRoutine(),
                GameLoopPhase.EndingDay =>
                    AdvanceAfterDayRoutine(),
                GameLoopPhase.TransitioningAct =>
                    AdvanceAfterActRoutine(),
                GameLoopPhase.PlayingEnding =>
                    CompleteGameRoutine(),
                _ => null
            };

            if (continuation == null)
            {
                Debug.LogWarning(
                    $"[GameLoop] Ignored sequence completion " +
                    $"while in {Phase}.");
                return;
            }

            ReplaceTransition(
                ContinueAfterDialogueRoutine(
                    continuation));
        }

        private IEnumerator StartNewGameRoutine()
        {
            StopActiveSceneDialogue();
            yield return WaitForActiveSceneDialogueToFinish();

            GameSession.Instance.ResetRuntimeState();
            BindToCurrentState();

            loopState.Reset();
            state.Day.History.Clear();
            state.Day.BeginDay();

            SetPhase(GameLoopPhase.StartingGame);
            Log($"Starting {currentChapter.DisplayName}.");

            yield return LoadSceneRoutine(
                currentChapter.StartScene);
        }

        private IEnumerator BeginActivityRoutine(
            IActivityRequest request)
        {
            yield return WaitForActiveSceneDialogueToFinish();

            bool started =
                GameSession.Instance.Activities.TryStartActivity(
                    request,
                    out ActivityRunState run,
                    out string failureReason);

            if (!started)
            {
                Debug.LogWarning(
                    $"[GameLoop] Activity could not start: " +
                    failureReason);

                SetPhase(GameLoopPhase.AwaitingDecision);
                EnsureDecisionPresenter();
                yield break;
            }

            SetPhase(GameLoopPhase.PerformingActivity);
            Log(
                $"Starting activity '{run.ActivityId}' in " +
                $"scene '{run.SceneName}' at {CurrentTimeDisplay}.");

            yield return LoadSceneRoutine(run.SceneName);
        }

        private IEnumerator ResolveActivityRoutine(
            ActivityResult result)
        {
            yield return WaitForActiveSceneDialogueToFinish();

            ActivityCompletion completion =
                GameSession.Instance.Activities
                    .CompleteCurrentActivity(result);

            Log(
                $"Completed '{completion.CompletedRun.ActivityId}'. " +
                $"Advanced {completion.DurationMinutes} minutes to " +
                $"{CurrentTimeDisplay}.");

            if (standaloneSceneMode)
            {
                SetPhase(GameLoopPhase.StandaloneComplete);
                yield break;
            }

            if (completion.ShouldEndDay)
            {
                yield return BeginEndOfDayRoutine();
            }
            else
            {
                yield return BeginDecisionRoutine();
            }
        }

        private IEnumerator BeginActRoutine()
        {
            state.Clock.BeginDay(
                1,
                CurrentDayRules.WakeMinute);

            state.Day.BeginDay();
            loopState.IsDayEnding = false;

            GameSession.Instance.Activities.SetDayRules(
                CurrentDayRules);

            SetPhase(GameLoopPhase.StartingAct);
            yield return LoadSceneRoutine(
                CurrentActDefinition.StartScene);
        }

        private IEnumerator BeginDayRoutine(int day)
        {
            state.Clock.BeginDay(
                day,
                CurrentDayRules.WakeMinute);

            state.Day.BeginDay();
            loopState.IsDayEnding = false;

            GameSession.Instance.Activities.SetDayRules(
                CurrentDayRules);

            SetPhase(GameLoopPhase.PlayingDayOpening);
            yield return LoadSceneRoutine(
                CurrentActDefinition.DayStartScene);
        }

        private IEnumerator BeginDecisionRoutine()
        {
            if (CurrentDayRules.ShouldEndDay(state))
            {
                yield return BeginEndOfDayRoutine();
                yield break;
            }

            IReadOnlyList<ActivityOption> options =
                GameSession.Instance.ActivityQuery.GetOptions();

            if (options.Count == 0 ||
                !options.Any(option => option.IsAvailable))
            {
                Log(
                    "No activities are available; ending the day.");
                yield return BeginEndOfDayRoutine();
                yield break;
            }

            SetPhase(GameLoopPhase.AwaitingDecision);
            yield return LoadSceneRoutine(
                CurrentActDefinition.DecisionScene);
        }

        private IEnumerator BeginEndOfDayRoutine()
        {
            loopState.IsDayEnding = true;
            SetPhase(GameLoopPhase.EndingDay);

            yield return LoadSceneRoutine(
                CurrentActDefinition.DayEndScene);
        }

        private IEnumerator AdvanceAfterDayRoutine()
        {
            loopState.IsDayEnding = false;

            NarrativeProgress progress =
                CreateNarrativeProgress(
                    completedActsInChapter:
                        CurrentAct - 1);

            if (CurrentActDefinition.IsComplete(progress))
            {
                SetPhase(GameLoopPhase.TransitioningAct);
                yield return LoadSceneRoutine(
                    CurrentActDefinition.EndScene);
                yield break;
            }

            yield return BeginDayRoutine(
                CurrentDay + 1);
        }

        private IEnumerator AdvanceAfterActRoutine()
        {
            NarrativeProgress chapterProgress =
                CreateNarrativeProgress(
                    completedActsInChapter:
                        CurrentAct);

            if (currentChapter.IsComplete(chapterProgress))
            {
                yield return BeginEndingRoutine();
                yield break;
            }

            if (CurrentAct >= currentChapter.ActCount)
            {
                Debug.LogError(
                    $"[GameLoop] Chapter '{currentChapter.Id}' " +
                    "has no next act, but its completion " +
                    "condition is not met.");
                yield break;
            }

            loopState.CurrentAct++;
            yield return BeginActRoutine();
        }

        private IEnumerator BeginEndingRoutine()
        {
            loopState.IsGameComplete = false;
            loopState.IsDayEnding = false;

            SetPhase(GameLoopPhase.PlayingEnding);
            yield return LoadSceneRoutine(
                currentChapter.EndScene);
        }

        private IEnumerator CompleteGameRoutine()
        {
            loopState.IsGameComplete = true;
            loopState.IsDayEnding = false;

            SetPhase(GameLoopPhase.Completed);
            Log("Demo completed.");
            yield break;
        }

        private IEnumerator DebugSkipToNextDayRoutine()
        {
            StopActiveSceneDialogue();
            yield return WaitForActiveSceneDialogueToFinish();

            state.Day.CurrentActivity = null;
            loopState.IsDayEnding = false;

            yield return AdvanceAfterDayRoutine();
        }

        private void StartStandaloneScene(string sceneName)
        {
            ConfigureStandaloneSceneState(
                sceneName,
                resetRuntime: true);

            Log(
                $"Standalone-scene mode: '{sceneName}'.");
            StartCoroutine(
                ValidateActiveSceneRunnerRoutine(sceneName));

            if (IsDecisionScene(sceneName))
            {
                EnsureDecisionPresenter();
            }

            if (state?.Day?.CurrentActivity != null)
            {
                EnsureActivitySceneController();
            }
        }

        private void ConfigureStandaloneSceneState(
            string sceneName,
            bool resetRuntime)
        {
            standaloneSceneMode = true;

            if (resetRuntime)
            {
                GameSession.Instance.ResetRuntimeState();
                BindToCurrentState();
            }

            loopState.Reset();

            int actIndex =
                currentChapter.FindActIndexForScene(
                    sceneName);

            if (actIndex >= 0)
            {
                loopState.CurrentAct = actIndex + 1;
            }

            int startMinute =
                CurrentDayRules.WakeMinute;

            ActivityDefinition standaloneActivity =
                GameSession.Instance.Configuration
                    .ActivityCatalog
                    .FindActivityForScene(sceneName);

            if (standaloneActivity != null)
            {
                standaloneActivity.TryGetStandaloneMinuteForScene(
                    sceneName,
                    startMinute,
                    out startMinute);
            }

            state.Clock.BeginDay(1, startMinute);
            state.Day.BeginDay();

            GameSession.Instance.Activities.SetDayRules(
                CurrentDayRules);

            if (string.Equals(
                    sceneName,
                    currentChapter.StartScene,
                    StringComparison.OrdinalIgnoreCase))
            {
                SetPhase(GameLoopPhase.StartingGame);
                return;
            }

            if (string.Equals(
                    sceneName,
                    CurrentActDefinition.StartScene,
                    StringComparison.OrdinalIgnoreCase))
            {
                SetPhase(GameLoopPhase.StartingAct);
                return;
            }

            if (string.Equals(
                    sceneName,
                    CurrentActDefinition.DayStartScene,
                    StringComparison.OrdinalIgnoreCase))
            {
                SetPhase(GameLoopPhase.PlayingDayOpening);
                return;
            }

            if (string.Equals(
                    sceneName,
                    CurrentActDefinition.DayEndScene,
                    StringComparison.OrdinalIgnoreCase))
            {
                loopState.IsDayEnding = true;
                SetPhase(GameLoopPhase.EndingDay);
                return;
            }

            if (IsDecisionScene(sceneName))
            {
                SetPhase(GameLoopPhase.AwaitingDecision);
                return;
            }

            if (string.Equals(
                    sceneName,
                    CurrentActDefinition.EndScene,
                    StringComparison.OrdinalIgnoreCase))
            {
                SetPhase(GameLoopPhase.TransitioningAct);
                return;
            }

            if (string.Equals(
                    sceneName,
                    currentChapter.EndScene,
                    StringComparison.OrdinalIgnoreCase))
            {
                SetPhase(GameLoopPhase.PlayingEnding);
                return;
            }

            if (TryPrepareStandaloneActivity(sceneName))
            {
                SetPhase(GameLoopPhase.PerformingActivity);
                return;
            }

            SetPhase(GameLoopPhase.StandaloneScene);
        }

        private bool TryPrepareStandaloneActivity(
            string sceneName)
        {
            ActivityCatalog catalog =
                GameSession.Instance.Configuration
                    .ActivityCatalog;

            ActivityDefinition activity =
                catalog.FindActivityForScene(sceneName);

            if (activity == null)
            {
                return false;
            }

            if (!GameSession.Instance.ActivityModules
                    .TryCreateRequestForScene(
                        activity,
                        catalog,
                        sceneName,
                        out IActivityRequest request))
            {
                Debug.LogError(
                    $"[GameLoop] Activity '{activity.Id}' could not " +
                    $"create a standalone request for scene " +
                    $"'{sceneName}'.");
                return false;
            }

            bool started =
                GameSession.Instance.Activities.TryStartActivity(
                    request,
                    out ActivityRunState run,
                    out string failureReason);

            if (!started)
            {
                Debug.LogError(
                    $"[GameLoop] Standalone activity could not " +
                    $"start: {failureReason}");
                return false;
            }

            if (!string.Equals(
                    run.SceneName,
                    sceneName,
                    StringComparison.OrdinalIgnoreCase))
            {
                Debug.LogWarning(
                    $"[GameLoop] Standalone scene '{sceneName}' " +
                    $"resolved to '{run.SceneName}'. The current " +
                    "scene remains active for debugging.");
            }

            return true;
        }

        private IEnumerator ResetStandaloneSceneRoutine()
        {
            string sceneName =
                SceneManager.GetActiveScene().name;

            StopActiveSceneDialogue();
            yield return WaitForActiveSceneDialogueToFinish();

            GameSession.Instance.ResetRuntimeState();
            BindToCurrentState();

            ConfigureStandaloneSceneState(
                sceneName,
                resetRuntime: false);

            yield return LoadSceneRoutine(sceneName);
        }

        private IEnumerator CompleteStandaloneSequenceAfterDialogueRoutine()
        {
            yield return WaitForActiveSceneDialogueToFinish();
            loopState.IsDayEnding = false;
            SetPhase(GameLoopPhase.StandaloneComplete);
        }

        private IEnumerator ContinueAfterDialogueRoutine(
            IEnumerator continuation)
        {
            yield return WaitForActiveSceneDialogueToFinish();

            while (continuation.MoveNext())
            {
                yield return continuation.Current;
            }
        }

        private void StopActiveSceneDialogue()
        {
            DialogueRunner runner =
                FindActiveSceneDialogueRunner();

            if (runner != null &&
                runner.IsDialogueRunning)
            {
                runner.Stop();
            }
        }

        private IEnumerator WaitForActiveSceneDialogueToFinish()
        {
            DialogueRunner runner =
                FindActiveSceneDialogueRunner();

            yield return null;

            while (runner != null &&
                   runner.IsDialogueRunning)
            {
                yield return null;
            }
        }

        private IEnumerator LoadSceneRoutine(
            string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogError(
                    "[GameLoop] Cannot load an empty scene name.");
                yield break;
            }

            isSceneLoadInProgress = true;
            SyncInspector();
            Log($"Loading scene '{sceneName}'.");

            yield return sceneTransitions.LoadSingle(sceneName);

            isSceneLoadInProgress = false;
            SyncInspector();

            yield return ValidateActiveSceneRunnerRoutine(
                sceneName);
        }

        private IEnumerator ValidateActiveSceneRunnerRoutine(
            string sceneName)
        {
            yield return null;

            DialogueRunner runner =
                FindActiveSceneDialogueRunner();

            if (runner == null)
            {
                Debug.LogWarning(
                    $"[GameLoop] Scene '{sceneName}' has no " +
                    "active scene-local DialogueRunner.");
                yield break;
            }

            if (!runner.autoStart)
            {
                Debug.LogWarning(
                    $"[GameLoop] DialogueRunner in '{sceneName}' " +
                    "has Auto Start disabled.");
            }
        }

        private DialogueRunner FindActiveSceneDialogueRunner()
        {
            Scene activeScene =
                SceneManager.GetActiveScene();

            return FindObjectsOfType<DialogueRunner>(true)
                .FirstOrDefault(
                    runner =>
                        runner != null &&
                        runner.gameObject.scene == activeScene &&
                        runner.gameObject.activeInHierarchy &&
                        runner.enabled);
        }

        private void OnSceneLoaded(
            Scene scene,
            LoadSceneMode mode)
        {
            SyncInspector();

            if (IsDecisionScene(scene.name))
            {
                EnsureDecisionPresenter();
            }

            if (state?.Day?.CurrentActivity != null &&
                string.Equals(
                    state.Day.CurrentActivity.SceneName,
                    scene.name,
                    StringComparison.OrdinalIgnoreCase))
            {
                EnsureActivitySceneController();
            }
        }

        private void EnsureActivitySceneController()
        {
            if (FindObjectOfType<ActivitySceneController>() != null)
            {
                return;
            }

            var controllerObject =
                new GameObject("Activity Scene Controller");

            controllerObject.AddComponent<ActivitySceneController>();
        }

        private void EnsureDecisionPresenter()
        {
            if (FindObjectOfType<DecisionScenePresenter>() != null)
            {
                return;
            }

            var presenterObject =
                new GameObject("Activity Decision Presenter");

            presenterObject.AddComponent<
                DecisionScenePresenter>();
        }

        private bool IsDecisionScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                return false;
            }

            if (currentChapter != null &&
                currentChapter.ActCount > 0 &&
                string.Equals(
                    sceneName,
                    CurrentActDefinition.DecisionScene,
                    StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private NarrativeProgress CreateNarrativeProgress(
            int completedActsInChapter)
        {
            return new NarrativeProgress(
                state,
                completedDaysInAct: CurrentDay,
                completedActsInChapter:
                    completedActsInChapter,
                totalActsInChapter:
                    currentChapter.ActCount);
        }

        private void EnsureConfiguration()
        {
            if (currentChapter == null)
            {
                throw new InvalidOperationException(
                    "GameLoopService has no Starting Chapter.");
            }

            if (currentChapter.ActCount == 0)
            {
                throw new InvalidOperationException(
                    $"Chapter '{currentChapter.name}' contains " +
                    "no acts.");
            }
        }

        private void ReplaceTransition(
            IEnumerator routine)
        {
            if (activeTransition != null)
            {
                StopCoroutine(activeTransition);
            }

            activeTransition =
                StartCoroutine(RunTransition(routine));
        }

        private IEnumerator RunTransition(
            IEnumerator routine)
        {
            while (routine.MoveNext())
            {
                yield return routine.Current;
            }

            activeTransition = null;
        }

        private void SetPhase(
            GameLoopPhase nextPhase)
        {
            if (loopState == null)
            {
                BindToCurrentState();
            }

            GameLoopPhase previous =
                loopState.Phase;

            loopState.Phase = nextPhase;
            SyncInspector();

            if (previous != nextPhase)
            {
                Log(
                    $"Phase: {previous} -> {nextPhase} " +
                    $"(Act {CurrentAct}, Day {CurrentDay}, " +
                    $"Time {CurrentTimeDisplay}, " +
                    $"Actions {ActionsCompletedToday}).");
            }
        }

        private void SyncInspector()
        {
            inspectorPhase = Phase;
            inspectorCurrentAct = CurrentAct;
            inspectorCurrentDay = CurrentDay;
            inspectorMinuteOfDay =
                state?.Clock?.MinuteOfDay ?? 0;
            inspectorActionsCompleted =
                state?.Day?.ActionsCompleted ?? 0;
            inspectorCurrentActivity =
                state?.Day?.CurrentActivity?.ActivityId;
            inspectorDayEnding = IsDayEnding;
            inspectorGameComplete = IsGameComplete;
            inspectorStandaloneSceneMode =
                standaloneSceneMode;
            inspectorActiveScene =
                SceneManager.GetActiveScene().name;
        }

        private void Log(string message)
        {
            if (logTransitions)
            {
                Debug.Log(
                    $"[GameLoop] {message}",
                    this);
            }
        }
    }
}
