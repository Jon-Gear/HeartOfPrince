#nullable enable
using System.Collections.Generic;
using HeartOfPrince.Application;
using HeartOfPrince.Domain;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

namespace HeartOfPrince.Presentation
{
    /// <summary>
    /// Activity decision bridge that feeds the shared bottom option carousel.
    /// It owns no separate decision UI.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class DecisionScenePresenter : MonoBehaviour
    {
        private IReadOnlyList<ActivityOption>? options;
        private BottomDialogueView view = null!;
        private bool wasVisible;
        private int displayedDay = -1;
        private string? displayedTime;
        private bool selectionInProgress;
        private Coroutine? showOptionsRoutine;

        private void Awake()
        {
            view = Object.FindFirstObjectByType<BottomDialogueView>();
            if (view == null)
            {
                view = gameObject.AddComponent<BottomDialogueView>();
            }
        }

        private void OnEnable()
        {
            if (view != null)
            {
                view.FocusedOptionChanged += OnFocusedOptionChanged;
            }

            Refresh();
            UpdateVisibility(force: true);
        }

        private void Update()
        {
            UpdateVisibility(force: false);
        }

        private void OnDisable()
        {
            wasVisible = false;
            selectionInProgress = false;
            StopPendingShowOptions();

            if (view != null)
            {
                view.FocusedOptionChanged -= OnFocusedOptionChanged;
                view.HideAll();
            }
        }

        public void Refresh()
        {
            options = GameSession.Instance?
                .ActivityQuery?
                .GetOptions();
        }

        private void UpdateVisibility(bool force)
        {
            GameLoopService loop = GameLoopService.Instance;

            bool shouldBeVisible =
                loop != null &&
                loop.Phase == GameLoopPhase.AwaitingDecision;

            if (force || shouldBeVisible != wasVisible)
            {
                wasVisible = shouldBeVisible;

                if (shouldBeVisible && loop != null)
                {
                    Refresh();
                    ScheduleShowOptionsAfterDialogue();
                }
                else if (view != null)
                {
                    StopPendingShowOptions();
                    view.HideAll();
                    selectionInProgress = false;
                }

                return;
            }

            if (!shouldBeVisible || loop == null)
            {
                return;
            }

            string currentTime = loop.CurrentTimeDisplay;
            if (force ||
                displayedDay != loop.CurrentDay ||
                displayedTime != currentTime)
            {
                displayedDay = loop.CurrentDay;
                displayedTime = currentTime;
                ScheduleShowOptionsAfterDialogue();
            }
        }

        private void ScheduleShowOptionsAfterDialogue()
        {
            StopPendingShowOptions();
            showOptionsRoutine = StartCoroutine(
                ShowOptionsAfterDialogueRoutine());
        }

        private void StopPendingShowOptions()
        {
            if (showOptionsRoutine == null)
            {
                return;
            }

            StopCoroutine(showOptionsRoutine);
            showOptionsRoutine = null;
        }

        private IEnumerator ShowOptionsAfterDialogueRoutine()
        {
            // Let the scene-local DialogueRunner run Start() before checking
            // IsDialogueRunning; otherwise options can appear before dialogue.
            yield return null;

            DialogueRunner? runner = FindSceneDialogueRunner();
            while (runner != null && runner.IsDialogueRunning)
            {
                yield return null;
            }

            showOptionsRoutine = null;

            GameLoopService loop = GameLoopService.Instance;
            if (loop == null ||
                loop.Phase != GameLoopPhase.AwaitingDecision ||
                selectionInProgress)
            {
                yield break;
            }

            Refresh();
            ShowOptions(loop);
        }

        private static DialogueRunner? FindSceneDialogueRunner()
        {
            Scene activeScene = SceneManager.GetActiveScene();

            foreach (DialogueRunner runner in
                     Object.FindObjectsByType<DialogueRunner>(
                         FindObjectsInactive.Include,
                         FindObjectsSortMode.None))
            {
                if (runner != null &&
                    runner.gameObject.scene == activeScene &&
                    runner.gameObject.activeInHierarchy &&
                    runner.enabled)
                {
                    return runner;
                }
            }

            return null;
        }

        private void ShowOptions(GameLoopService loop)
        {
            if (view == null || loop == null)
            {
                return;
            }

            IReadOnlyList<ActivityOption> currentOptions =
                options ?? System.Array.Empty<ActivityOption>();

            var items = new List<OptionSelectionItem>();
            for (int index = 0; index < currentOptions.Count; index++)
            {
                ActivityOption option = currentOptions[index];
                ActivityOption capturedOption = option;

                items.Add(new OptionSelectionItem(
                    id: BuildOptionId(option, index),
                    displayText: option.DisplayName,
                    category: "Activity",
                    isEnabled: option.IsAvailable,
                    requirementLabel: option.IsAvailable
                        ? null
                        : option.UnavailableReason,
                    payload: option,
                    onSelected: _ => SelectActivity(capturedOption)));
            }

            string title =
                $"Day {loop.CurrentDay} - {loop.CurrentTimeDisplay}";

            var request = new OptionSelectionRequest(
                items,
                title,
                emptyMessage: "No activities are currently available.",
                closeOnSelection: false);

            ShowDecisionDialogue(
                items.Count > 0 ? items[0] : null);
            view.OpenOptions(request, keepDialogueVisible: true);
        }

        private void OnFocusedOptionChanged(OptionSelectionItem item)
        {
            if (selectionInProgress)
            {
                return;
            }

            ShowDecisionDialogue(item);
        }

        private void ShowDecisionDialogue(OptionSelectionItem? item)
        {
            if (view == null)
            {
                return;
            }

            if (item?.Payload is not ActivityOption option)
            {
                view.ShowDialogue(
                    "Decision",
                    "Choose how Prince spends this time.",
                    hint: null);
                return;
            }

            view.ShowDialogue(
                option.DisplayName,
                BuildDecisionText(option),
                hint: null);
        }

        private string BuildDecisionText(ActivityOption option)
        {
            var lines = new List<string>();

            string description = ResolveDecisionDescription(option);
            if (!string.IsNullOrWhiteSpace(description))
            {
                lines.Add(description.Trim());
            }

            string duration = FormatDuration(
                option.Request?.Activity?.DurationMinutes ?? 0);
            if (!string.IsNullOrWhiteSpace(duration))
            {
                lines.Add($"Time: {duration}.");
            }

            if (!option.IsAvailable &&
                !string.IsNullOrWhiteSpace(option.UnavailableReason))
            {
                lines.Add($"Unavailable: {option.UnavailableReason}");
            }

            return lines.Count > 0
                ? string.Join("\n", lines)
                : "Choose how Prince spends this time.";
        }

        private string ResolveDecisionDescription(ActivityOption option)
        {
            if (option?.Request?.Input is TalkActivityInput talkInput)
            {
                CharacterDefinition? character =
                    GameSession.Instance?
                        .ActiveActivityCatalog?
                        .FindCharacter(talkInput.CharacterId);

                if (character == null)
                {
                    character = GameSession.Instance?
                        .Configuration?
                        .ActivityCatalog?
                        .FindCharacter(talkInput.CharacterId);
                }

                if (!string.IsNullOrWhiteSpace(
                        character?.TalkDecisionDescription))
                {
                    return character.TalkDecisionDescription;
                }
            }

            string? activityDescription =
                option?.Request?.Activity?.DecisionDescription;

            if (!string.IsNullOrWhiteSpace(activityDescription))
            {
                return activityDescription;
            }

            return "Choose how Prince spends this time.";
        }

        private static string FormatDuration(int minutes)
        {
            if (minutes <= 0)
            {
                return "No time passes";
            }

            int hours = minutes / 60;
            int remainingMinutes = minutes % 60;
            var parts = new List<string>();

            if (hours > 0)
            {
                parts.Add(hours == 1 ? "1 hour" : $"{hours} hours");
            }

            if (remainingMinutes > 0)
            {
                parts.Add(
                    remainingMinutes == 1
                        ? "1 minute"
                        : $"{remainingMinutes} minutes");
            }

            return string.Join(" ", parts);
        }

        private static string BuildOptionId(
            ActivityOption option,
            int index)
        {
            string activityId =
                option.Request?.Activity?.Id ??
                option.DisplayName ??
                "activity";

            string selectionKey =
                option.Request?.Input?.SelectionKey ??
                index.ToString();

            return $"{activityId}:{selectionKey}";
        }

        private void SelectActivity(ActivityOption option)
        {
            if (selectionInProgress ||
                option == null ||
                !option.IsAvailable)
            {
                return;
            }

            selectionInProgress = true;
            view.CloseOptions();
            view.ShowDialogue(
                "Activity",
                $"Starting {option.DisplayName}...",
                hint: null);

            GameLoopService loop = GameLoopService.Instance;
            if (loop == null)
            {
                selectionInProgress = false;
                view.ShowDialogue(
                    "Activity",
                    "The game loop is not available.");
                return;
            }

            loop.RequestActivity(option);
        }
    }
}
