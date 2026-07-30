#nullable enable
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Yarn.Unity;

namespace HeartOfPrince.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(DialogueRunner))]
    [RequireComponent(typeof(BottomDialogueView))]
    public sealed class BottomDialogueYarnPresenter : DialoguePresenterBase
    {
        [SerializeField, Min(0f)]
        private float charactersPerSecond = 60f;

        private DialogueRunner runner = null!;
        private BottomDialogueView view = null!;
        private bool waitingForNextLine;
        private bool presentingOptions;
        private string? lastSpeaker;
        private string lastText = string.Empty;
        private Coroutine? idleHideRoutine;

        private void Awake()
        {
            runner = GetComponent<DialogueRunner>();
            view = GetComponent<BottomDialogueView>();
            view.ContinueRequested += OnContinueRequested;
        }

        private void OnDestroy()
        {
            if (view != null)
            {
                view.ContinueRequested -= OnContinueRequested;
            }
        }

        public override YarnTask OnDialogueStartedAsync()
        {
            CancelIdleHide();
            view.HideAll();
            waitingForNextLine = false;
            presentingOptions = false;
            lastSpeaker = null;
            lastText = string.Empty;
            return YarnTask.CompletedTask;
        }

        public override YarnTask OnDialogueCompleteAsync()
        {
            CancelIdleHide();
            view.HideAll();
            waitingForNextLine = false;
            presentingOptions = false;
            lastSpeaker = null;
            lastText = string.Empty;
            return YarnTask.CompletedTask;
        }

        public override async YarnTask RunLineAsync(
            LocalizedLine line,
            LineCancellationToken token)
        {
            CancelIdleHide();
            presentingOptions = false;
            view.CloseOptions();

            lastSpeaker = line.CharacterName;
            lastText = line.TextWithoutCharacterName.Text;
            waitingForNextLine = false;

            await RevealLineAsync(lastSpeaker, lastText, token.HurryUpToken);

            waitingForNextLine = true;

            await YarnTask
                .WaitUntilCanceled(token.NextLineToken)
                .SuppressCancellationThrow();

            waitingForNextLine = false;
            ScheduleIdleHide();
        }

        public override async YarnTask<DialogueOption?> RunOptionsAsync(
            DialogueOption[] dialogueOptions,
            CancellationToken cancellationToken)
        {
            CancelIdleHide();
            presentingOptions = true;

            var completion = new TaskCompletionSource<DialogueOption?>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            var items = new List<OptionSelectionItem>();
            foreach (DialogueOption option in dialogueOptions)
            {
                DialogueOption capturedOption = option;
                string text = option.Line.TextWithoutCharacterName.Text;

                items.Add(new OptionSelectionItem(
                    id: option.DialogueOptionID.ToString(),
                    displayText: text,
                    category: "Dialogue",
                    isEnabled: option.IsAvailable,
                    requirementLabel: option.IsAvailable ? null : "Unavailable",
                    payload: option,
                    onSelected: _ => completion.TrySetResult(capturedOption)));
            }

            using (cancellationToken.Register(
                       () => completion.TrySetResult(null)))
            {
                var request = new OptionSelectionRequest(
                    items,
                    title: "Respond",
                    emptyMessage: "No responses are available.");

                bool keepDialogueVisible = !string.IsNullOrWhiteSpace(lastText);
                if (keepDialogueVisible)
                {
                    view.SetDialogueTextImmediately(
                        lastSpeaker,
                        lastText,
                        hint: null);
                }

                view.OpenOptions(request, keepDialogueVisible);
                DialogueOption? selected = await completion.Task;

                presentingOptions = false;
                view.CloseOptions();
                ScheduleIdleHide();
                return selected;
            }
        }

        private void ScheduleIdleHide()
        {
            CancelIdleHide();
            idleHideRoutine = StartCoroutine(HideWhenIdleRoutine());
        }

        private void CancelIdleHide()
        {
            if (idleHideRoutine == null)
            {
                return;
            }

            StopCoroutine(idleHideRoutine);
            idleHideRoutine = null;
        }

        private IEnumerator HideWhenIdleRoutine()
        {
            yield return null;

            idleHideRoutine = null;

            if (waitingForNextLine || presentingOptions)
            {
                yield break;
            }

            view.HideAll();
            lastSpeaker = null;
            lastText = string.Empty;
        }

        private async Task RevealLineAsync(
            string? speaker,
            string text,
            CancellationToken hurryUpToken)
        {
            if (charactersPerSecond <= 0f || string.IsNullOrEmpty(text))
            {
                view.ShowDialogue(speaker, text);
                return;
            }

            int delayMilliseconds = Mathf.Max(
                1,
                Mathf.RoundToInt(1000f / charactersPerSecond));

            for (int index = 1; index <= text.Length; index++)
            {
                if (hurryUpToken.IsCancellationRequested)
                {
                    view.ShowDialogue(speaker, text);
                    return;
                }

                view.ShowDialogue(speaker, text.Substring(0, index));
                await Task.Delay(delayMilliseconds);
            }
        }

        private void OnContinueRequested()
        {
            if (presentingOptions ||
                runner == null ||
                !runner.IsDialogueRunning)
            {
                return;
            }

            if (waitingForNextLine)
            {
                runner.RequestNextLine();
            }
            else
            {
                runner.RequestHurryUpLine();
            }
        }
    }
}
