using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeartOfPrince.Domain;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Yarn.Unity;

namespace HeartOfPrince.Presentation
{
    /// <summary>
    /// Central state machine for the complete game, day, and decision loops.
    /// Scenes provide presentation and report completion; this service decides what happens next.
    /// </summary>
    [DefaultExecutionOrder(-900)]
    [DisallowMultipleComponent]
    public sealed class GameLoopService : MonoBehaviour
    {
        [Serializable]
        private sealed class TalkActionRoute
        {
            [SerializeField] private string characterId;
            [SerializeField] private string sceneName;
            [SerializeField] private string yarnNode;

            public string CharacterId => characterId;
            public string SceneName => sceneName;
            public string YarnNode => yarnNode;

            public TalkActionRoute()
            {
            }

            public TalkActionRoute(string characterId, string sceneName, string yarnNode)
            {
                this.characterId = characterId;
                this.sceneName = sceneName;
                this.yarnNode = yarnNode;
            }
        }
        public static GameLoopService Instance { get; private set; }

        private const string NarrativeHostScene = "Conversation_Munir_Evening";
        private const string DayStartScene = "Day_Start";
        private const string DayEndScene = "Day_End";
        private const string TalkScene = "Conversation_Munir_Evening";
        private const string PonderMorningScene = "Ponder_Morning";
        private const string PonderEveningScene = "Ponder_Evening";
        private const string DecisionMorningScene = "Decision_Morning";
        private const string DecisionEveningScene = "Decision_Evening";

        private const string DayOpeningFallbackNode = "Loop_DayOpening";
        private const string DayEndingFallbackNode = "Loop_DayEnding";
        private const string DecisionNode = "Loop_Decision";
        private const string TalkNode = "Loop_Talk_Munir";
        private const string PonderNode = "Loop_Ponder";
        private const string EndingNode = "Loop_DemoEnding";

        [Header("Demo Configuration")]
        [SerializeField, Min(1)] private int decisionsPerDay = 2;
        [SerializeField, Min(1)] private int daysPerAct = 2;
        [SerializeField, Min(1)] private int actsInDemo = 2;
        [SerializeField] private bool startAutomatically = true;
        [SerializeField] private List<TalkActionRoute> talkRoutes = new()
        {
            new TalkActionRoute("Munir", TalkScene, TalkNode)
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

        private DialogueRunner dialogueRunner;
        private EventSystem persistentEventSystem;
        private Coroutine activeTransition;
        private GameLoopState loopState;
        private bool hostIsReady;
        private bool isSceneLoadInProgress;

        public GameLoopPhase Phase => loopState?.Phase ?? GameLoopPhase.None;
        public int CurrentAct => loopState?.CurrentAct ?? 0;
        public int CurrentDay => loopState?.CurrentDay ?? 0;
        public int CurrentDecisionIndex => loopState?.CurrentDecisionIndex ?? 0;
        public int DecisionsAllowedPerDay => loopState?.DecisionsAllowedPerDay ?? decisionsPerDay;
        public bool IsActionRunning => loopState?.IsActionRunning ?? false;
        public bool IsDayEnding => loopState?.IsDayEnding ?? false;
        public bool IsGameComplete => loopState?.IsGameComplete ?? false;

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
            if (startAutomatically && Phase == GameLoopPhase.None)
            {
                StartNewGame();
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
            ReplaceTransition(StartNewGameRoutine());
        }

        [ContextMenu("Reset All Progression")]
        public void ResetAllProgression()
        {
            StartNewGame();
        }

        [ContextMenu("Skip To Next Day")]
        public void DebugSkipToNextDay()
        {
            if (Phase == GameLoopPhase.Completed)
            {
                Debug.LogWarning("[GameLoop] Cannot skip: the demo is already complete.");
                return;
            }

            ReplaceTransition(DebugSkipToNextDayRoutine());
        }

        public void RequestAction(GameLoopAction action)
        {
            if (action == GameLoopAction.Talk)
            {
                RequestTalk("Munir");
                return;
            }

            RequestActionInternal(action, null);
        }

        public void RequestTalk(string characterId)
        {
            if (string.IsNullOrWhiteSpace(characterId))
            {
                Debug.LogWarning("[GameLoop] Ignored Talk request with an empty character ID.");
                return;
            }

            RequestActionInternal(GameLoopAction.Talk, characterId.Trim());
        }

        private void RequestActionInternal(GameLoopAction action, string talkCharacterId)
        {
            if (action == GameLoopAction.None)
            {
                Debug.LogWarning("[GameLoop] Ignored an empty action request.");
                return;
            }

            if (Phase != GameLoopPhase.AwaitingDecision || IsActionRunning || isSceneLoadInProgress)
            {
                Debug.LogWarning(
                    $"[GameLoop] Rejected {action}. Phase={Phase}, ActionRunning={IsActionRunning}, SceneLoading={isSceneLoadInProgress}.");
                return;
            }

            loopState.CurrentAction = action;
            loopState.CurrentTalkCharacterId = talkCharacterId;
            loopState.IsActionRunning = true;
            SetPhase(GameLoopPhase.LoadingAction);
            ReplaceTransition(BeginActionAfterDialogueRoutine(action));
        }

        public void NotifyActionCompleted()
        {
            if (Phase != GameLoopPhase.PerformingTalk && Phase != GameLoopPhase.PerformingPonder)
            {
                Debug.LogWarning($"[GameLoop] Ignored action completion while in {Phase}.");
                return;
            }

            SetPhase(GameLoopPhase.ResolvingAction);
            ReplaceTransition(ResolveActionAfterDialogueRoutine());
        }

        public void NotifySequenceCompleted()
        {
            switch (Phase)
            {
                case GameLoopPhase.PlayingDayOpening:
                    ReplaceTransition(ContinueAfterDialogueRoutine(BeginDecisionRoutine()));
                    break;

                case GameLoopPhase.EndingDay:
                    ReplaceTransition(ContinueAfterDialogueRoutine(AdvanceAfterDayRoutine()));
                    break;

                case GameLoopPhase.TransitioningAct:
                    ReplaceTransition(ContinueAfterDialogueRoutine(BeginDayRoutine()));
                    break;

                case GameLoopPhase.PlayingEnding:
                    ReplaceTransition(ContinueAfterDialogueRoutine(CompleteGameRoutine()));
                    break;

                default:
                    Debug.LogWarning($"[GameLoop] Ignored sequence completion while in {Phase}.");
                    break;
            }
        }

        private IEnumerator StartNewGameRoutine()
        {
            StopCurrentDialogue();
            yield return WaitForDialogueToFinish();
            ResetNarrativeHost();
            yield return null;

            GameSession.Instance.ResetRuntimeState();
            BindToCurrentState();

            loopState.Reset(decisionsPerDay);
            SeedPrototypeProgression();

            SetPhase(GameLoopPhase.StartingGame);
            Log("Starting a new game.");

            yield return EnsureNarrativeHostRoutine();

            if (dialogueRunner == null)
            {
                yield break;
            }

            SetPhase(GameLoopPhase.StartingAct);
            yield return BeginDayRoutine();
        }

        private IEnumerator EnsureNarrativeHostRoutine()
        {
            if (dialogueRunner != null && hostIsReady)
            {
                yield break;
            }

            yield return LoadSceneRoutine(NarrativeHostScene);
            CaptureAndPersistNarrativeHost();

            if (dialogueRunner == null)
            {
                Debug.LogError(
                    "[GameLoop] No DialogueRunner was found in the narrative host scene. " +
                    "Open Conversation_Munir_Evening and ensure its Dialogue System is present.");
                yield break;
            }

            yield return null;
        }

        private IEnumerator BeginDayRoutine()
        {
            loopState.IsDayEnding = false;
            loopState.IsActionRunning = false;
            loopState.CurrentAction = GameLoopAction.None;
            loopState.CurrentTalkCharacterId = null;
            loopState.CurrentDecisionIndex = 0;

            SetPhase(GameLoopPhase.StartingDay);
            yield return LoadSceneRoutine(DayStartScene);

            SetPhase(GameLoopPhase.PlayingDayOpening);
            var requestedNode = $"Loop_DayOpening_A{CurrentAct}_D{CurrentDay}";
            StartDialogueWithFallback(requestedNode, DayOpeningFallbackNode);
        }

        private IEnumerator BeginDecisionRoutine()
        {
            if (CurrentDecisionIndex >= DecisionsAllowedPerDay)
            {
                yield return BeginEndOfDayRoutine();
                yield break;
            }

            var scene = IsMorningSlot ? DecisionMorningScene : DecisionEveningScene;
            yield return LoadSceneRoutine(scene);

            SetPhase(GameLoopPhase.AwaitingDecision);
            StartDialogue(DecisionNode);
        }

        private IEnumerator BeginActionAfterDialogueRoutine(GameLoopAction action)
        {
            yield return WaitForDialogueToFinish();

            switch (action)
            {
                case GameLoopAction.Talk:
                    var route = ResolveTalkRoute(loopState.CurrentTalkCharacterId);
                    if (route == null)
                    {
                        loopState.IsActionRunning = false;
                        loopState.CurrentAction = GameLoopAction.None;
                        loopState.CurrentTalkCharacterId = null;
                        yield return BeginDecisionRoutine();
                        yield break;
                    }

                    yield return LoadSceneRoutine(route.SceneName);
                    SetPhase(GameLoopPhase.PerformingTalk);
                    StartDialogue(route.YarnNode);
                    break;

                case GameLoopAction.Ponder:
                    yield return LoadSceneRoutine(IsMorningSlot ? PonderMorningScene : PonderEveningScene);
                    SetPhase(GameLoopPhase.PerformingPonder);
                    StartDialogue(PonderNode);
                    break;
            }
        }

        private IEnumerator ResolveActionAfterDialogueRoutine()
        {
            yield return WaitForDialogueToFinish();

            loopState.CurrentDecisionIndex++;
            loopState.IsActionRunning = false;
            loopState.CurrentAction = GameLoopAction.None;
            loopState.CurrentTalkCharacterId = null;
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

        private IEnumerator BeginEndOfDayRoutine()
        {
            loopState.IsDayEnding = true;
            loopState.IsActionRunning = false;
            loopState.CurrentAction = GameLoopAction.None;
            loopState.CurrentTalkCharacterId = null;

            SetPhase(GameLoopPhase.EndingDay);
            yield return LoadSceneRoutine(DayEndScene);

            var requestedNode = $"Loop_DayEnding_A{CurrentAct}_D{CurrentDay}";
            StartDialogueWithFallback(requestedNode, DayEndingFallbackNode);
        }

        private IEnumerator AdvanceAfterDayRoutine()
        {
            loopState.IsDayEnding = false;
            loopState.CurrentDecisionIndex = 0;

            var finishedFinalDay = CurrentAct >= actsInDemo && CurrentDay >= daysPerAct;
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
                StartDialogueWithFallback(
                    $"Loop_ActTransition_A{CurrentAct}",
                    "Loop_ActTransition");
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
            SetPhase(GameLoopPhase.PlayingEnding);

            yield return LoadSceneRoutine(DayEndScene);
            StartDialogue(EndingNode);
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
            StopCurrentDialogue();
            yield return WaitForDialogueToFinish();

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
                StartDialogueWithFallback($"Loop_ActTransition_A{CurrentAct}", "Loop_ActTransition");
            }
            else
            {
                loopState.CurrentDay++;
                yield return BeginDayRoutine();
            }
        }

        private IEnumerator ContinueAfterDialogueRoutine(IEnumerator continuation)
        {
            yield return WaitForDialogueToFinish();

            while (continuation.MoveNext())
            {
                yield return continuation.Current;
            }
        }

        private IEnumerator WaitForDialogueToFinish()
        {
            yield return null;

            while (dialogueRunner != null && dialogueRunner.IsDialogueRunning)
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
                    "Use Heart of Prince > Rebuild Demo Scene List or add the demo scenes to Build Settings.");
                yield break;
            }

            while (!operation.isDone)
            {
                yield return null;
            }

            yield return null;
            CaptureAndPersistNarrativeHost();
            DestroyDuplicateRuntimeObjects();

            isSceneLoadInProgress = false;
            SyncInspector();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            CaptureAndPersistNarrativeHost();
            DestroyDuplicateRuntimeObjects();
        }

        private void CaptureAndPersistNarrativeHost()
        {
            if (dialogueRunner == null)
            {
                var runners = FindObjectsOfType<DialogueRunner>(true);
                dialogueRunner = runners.FirstOrDefault(runner => runner.gameObject.name == "Dialogue System")
                                 ?? runners.FirstOrDefault();

                if (dialogueRunner != null)
                {
                    dialogueRunner.autoStart = false;
                    DontDestroyOnLoad(dialogueRunner.transform.root.gameObject);
                    hostIsReady = true;
                    Log($"Using Yarn runner '{dialogueRunner.gameObject.name}' as the persistent narrative host.");
                }
            }

            if (persistentEventSystem == null)
            {
                var eventSystems = FindObjectsOfType<EventSystem>(true);
                persistentEventSystem = eventSystems.FirstOrDefault(
                                            system => system.transform.root.name == "EventSystem")
                                        ?? eventSystems.FirstOrDefault(
                                            system => system.transform.root.GetComponent<DialogueRunner>() == null)
                                        ?? eventSystems.FirstOrDefault();

                if (persistentEventSystem != null)
                {
                    DontDestroyOnLoad(persistentEventSystem.transform.root.gameObject);
                }
            }
        }

        private void DestroyDuplicateRuntimeObjects()
        {
            if (dialogueRunner != null)
            {
                foreach (var runner in FindObjectsOfType<DialogueRunner>(true))
                {
                    if (runner != dialogueRunner)
                    {
                        Destroy(runner.transform.root.gameObject);
                    }
                }
            }

            if (persistentEventSystem != null)
            {
                foreach (var eventSystem in FindObjectsOfType<EventSystem>(true))
                {
                    if (eventSystem != persistentEventSystem)
                    {
                        Destroy(eventSystem.transform.root.gameObject);
                    }
                }
            }
        }

        private bool DialogueNodeExists(string nodeName)
        {
            var nodeNames = dialogueRunner?.YarnProject?.NodeNames;
            return nodeNames != null && nodeNames.Contains(nodeName);
        }

        private void StartDialogueWithFallback(string requestedNode, string fallbackNode)
        {
            if (dialogueRunner != null && DialogueNodeExists(requestedNode))
            {
                StartDialogue(requestedNode);
            }
            else
            {
                StartDialogue(fallbackNode);
            }
        }

        private void StartDialogue(string nodeName)
        {
            if (dialogueRunner == null)
            {
                Debug.LogError($"[GameLoop] Cannot start Yarn node '{nodeName}': no DialogueRunner is available.");
                return;
            }

            if (!DialogueNodeExists(nodeName))
            {
                Debug.LogError($"[GameLoop] Yarn node '{nodeName}' does not exist in the assigned Yarn Project.");
                return;
            }

            if (dialogueRunner.IsDialogueRunning)
            {
                Debug.LogWarning($"[GameLoop] Cannot start '{nodeName}' while another dialogue is running.");
                return;
            }

            Log($"Starting Yarn node '{nodeName}'.");
            dialogueRunner.StartDialogue(nodeName);
        }

        private void ResetNarrativeHost()
        {
            if (dialogueRunner != null)
            {
                Destroy(dialogueRunner.transform.root.gameObject);
            }

            if (persistentEventSystem != null)
            {
                Destroy(persistentEventSystem.transform.root.gameObject);
            }

            dialogueRunner = null;
            persistentEventSystem = null;
            hostIsReady = false;
        }

        private void StopCurrentDialogue()
        {
            if (dialogueRunner != null && dialogueRunner.IsDialogueRunning)
            {
                dialogueRunner.Stop();
            }
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
                    "Add a route to the GameLoopService in the Bootstrap scene.");
            }

            return route;
        }

        private void SeedPrototypeProgression()
        {
            var munir = (CharacterID)"Munir";
            var topics = GameSession.Instance.State.GetOrCreateCharacterTopics(munir);

            topics.AddTopic(
                (TopicName)"PrototypeAskAboutResponsibility",
                ConversationTopicDirection.PlayerToCharacter);

            GameSession.Instance.State.Ponder.AddTopic((TopicName)"PrototypeQuietMoment");
            GameSession.Instance.State.GetOrCreateRelationship(munir);
        }

        private bool IsMorningSlot =>
            CurrentDecisionIndex < Mathf.CeilToInt(DecisionsAllowedPerDay * 0.5f);

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
                Log($"Phase: {previous} -> {nextPhase} " +
                    $"(Act {CurrentAct}, Day {CurrentDay}, Decision {CurrentDecisionIndex}/{DecisionsAllowedPerDay}).");
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
