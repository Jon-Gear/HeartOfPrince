#nullable enable
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Yarn.Unity;

namespace HeartOfPrince.Presentation
{
    /// <summary>
    /// Yarn Spinner 3 dialogue presenter implemented entirely with UI Toolkit.
    /// It replaces the legacy Canvas, Line Presenter, Options Presenter, and
    /// Line Advancer hierarchy from Yarn Spinner's GameObject-based prefab.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(DialogueRunner))]
    public sealed class UIToolkitDialoguePresenter :
        DialoguePresenterBase
    {
        [Header("Typewriter")]
        [SerializeField, Min(0f)]
        private float charactersPerSecond = 60f;

        [Header("Input")]
        [SerializeField]
        private bool focusContinueButton = true;

        private DialogueRunner runner = null!;
        private UIDocument document = null!;
        private PanelSettings ownedPanelSettings = null!;

        private VisualElement overlay = null!;
        private VisualElement lineCard = null!;
        private VisualElement optionsContainer = null!;
        private Label speakerLabel = null!;
        private Label lineLabel = null!;
        private Button continueButton = null!;

        private bool waitingForNextLine;
        private bool presentingOptions;

        private void Awake()
        {
            runner = GetComponent<DialogueRunner>();

            document =
                HeartOfPrinceUIToolkit.CreateDocument(
                    gameObject,
                    sortingOrder: 100,
                    out ownedPanelSettings);

            BuildVisualTree();
            HideDialogue();
        }

        private void OnDestroy()
        {
            HeartOfPrinceUIToolkit.DestroyPanelSettings(
                ownedPanelSettings);
        }

        private void BuildVisualTree()
        {
            VisualElement root = document.rootVisualElement;
            root.Clear();
            root.style.flexGrow = 1f;

            HeartOfPrinceUIToolkit.ApplySharedStyle(root);

            bool loaded =
                HeartOfPrinceUIToolkit.CloneTree(
                    root,
                    HeartOfPrinceUIToolkit.DialogueTreePath);

            if (!loaded)
            {
                BuildFallbackTree(root);
            }

            overlay =
                root.Q<VisualElement>("dialogue-overlay");

            lineCard =
                root.Q<VisualElement>("dialogue-card");

            optionsContainer =
                root.Q<VisualElement>("options");

            speakerLabel =
                root.Q<Label>("speaker");

            lineLabel =
                root.Q<Label>("line");

            continueButton =
                root.Q<Button>("continue");

            if (overlay == null ||
                lineCard == null ||
                optionsContainer == null ||
                speakerLabel == null ||
                lineLabel == null ||
                continueButton == null)
            {
                throw new InvalidOperationException(
                    "DialogueSystem.uxml is missing one or more " +
                    "required named elements.");
            }

            continueButton.clicked += OnContinueClicked;

            overlay.focusable = true;
            overlay.RegisterCallback<KeyDownEvent>(
                OnDialogueKeyDown);
        }

        private static void BuildFallbackTree(
            VisualElement root)
        {
            var fallbackOverlay = new VisualElement
            {
                name = "dialogue-overlay"
            };
            fallbackOverlay.AddToClassList("hop-overlay");
            fallbackOverlay.AddToClassList(
                "hop-dialogue-overlay");

            var spacer = new VisualElement();
            spacer.AddToClassList("hop-dialogue-spacer");
            fallbackOverlay.Add(spacer);

            var fallbackCard = new VisualElement
            {
                name = "dialogue-card"
            };
            fallbackCard.AddToClassList("hop-card");
            fallbackCard.AddToClassList(
                "hop-dialogue-card");

            var fallbackSpeaker = new Label
            {
                name = "speaker"
            };
            fallbackSpeaker.AddToClassList("hop-eyebrow");

            var fallbackLine = new Label
            {
                name = "line"
            };
            fallbackLine.AddToClassList(
                "hop-dialogue-text");

            var fallbackOptions = new VisualElement
            {
                name = "options"
            };
            fallbackOptions.AddToClassList("hop-options");

            var fallbackContinue = new Button
            {
                name = "continue",
                text = "▼"
            };
            fallbackContinue.AddToClassList("hop-button");
            fallbackContinue.AddToClassList(
                "hop-continue");

            fallbackCard.Add(fallbackSpeaker);
            fallbackCard.Add(fallbackLine);
            fallbackCard.Add(fallbackOptions);
            fallbackCard.Add(fallbackContinue);
            fallbackOverlay.Add(fallbackCard);
            root.Add(fallbackOverlay);
        }

        private void OnDialogueKeyDown(
            KeyDownEvent evt)
        {
            // Let focused option buttons handle Enter/Space themselves.
            if (presentingOptions)
            {
                return;
            }

            if (evt.keyCode != KeyCode.Space &&
                evt.keyCode != KeyCode.Return &&
                evt.keyCode != KeyCode.KeypadEnter)
            {
                return;
            }

            OnContinueClicked();
            evt.StopPropagation();
        }

        private void OnContinueClicked()
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

        public override YarnTask OnDialogueStartedAsync()
        {
            presentingOptions = false;
            overlay.style.display = DisplayStyle.Flex;
            lineCard.style.display = DisplayStyle.None;
            optionsContainer.style.display = DisplayStyle.None;
            return YarnTask.CompletedTask;
        }

        public override YarnTask OnDialogueCompleteAsync()
        {
            HideDialogue();
            return YarnTask.CompletedTask;
        }

        public override async YarnTask RunLineAsync(
            LocalizedLine line,
            LineCancellationToken token)
        {
            presentingOptions = false;
            overlay.style.display = DisplayStyle.Flex;
            lineCard.style.display = DisplayStyle.Flex;
            optionsContainer.style.display = DisplayStyle.None;
            speakerLabel.style.display = DisplayStyle.Flex;
            lineLabel.style.display = DisplayStyle.Flex;
            continueButton.style.display = DisplayStyle.Flex;

            string characterName =
                line.CharacterName ?? string.Empty;

            speakerLabel.text = characterName;
            speakerLabel.style.display =
                string.IsNullOrWhiteSpace(characterName)
                    ? DisplayStyle.None
                    : DisplayStyle.Flex;

            string text =
                line.TextWithoutCharacterName.Text;

            waitingForNextLine = false;

            // Focus before the typewriter starts so keyboard input can
            // hurry up the very first line as well as later lines.
            if (focusContinueButton)
            {
                continueButton.Focus();
            }
            else
            {
                overlay.Focus();
            }

            await RevealLineAsync(
                text,
                token.HurryUpToken);

            waitingForNextLine = true;

            await YarnTask
                .WaitUntilCanceled(token.NextLineToken)
                .SuppressCancellationThrow();

            waitingForNextLine = false;
        }

        private async Task RevealLineAsync(
            string text,
            CancellationToken hurryUpToken)
        {
            lineLabel.text = string.Empty;

            if (charactersPerSecond <= 0f ||
                string.IsNullOrEmpty(text))
            {
                lineLabel.text = text;
                return;
            }

            int delayMilliseconds =
                Mathf.Max(
                    1,
                    Mathf.RoundToInt(
                        1000f / charactersPerSecond));

            for (int index = 1;
                 index <= text.Length;
                 index++)
            {
                if (hurryUpToken.IsCancellationRequested)
                {
                    lineLabel.text = text;
                    return;
                }

                lineLabel.text =
                    text.Substring(0, index);

                await Task.Delay(delayMilliseconds);
            }

            lineLabel.text = text;
        }

        public override async YarnTask<DialogueOption?>
            RunOptionsAsync(
                DialogueOption[] options,
                CancellationToken cancellationToken)
        {
            presentingOptions = true;
            overlay.style.display = DisplayStyle.Flex;
            lineCard.style.display = DisplayStyle.Flex;
            speakerLabel.style.display = DisplayStyle.None;
            lineLabel.style.display = DisplayStyle.None;
            continueButton.style.display = DisplayStyle.None;

            optionsContainer.Clear();
            optionsContainer.style.display = DisplayStyle.Flex;

            var completion =
                new TaskCompletionSource<DialogueOption?>(
                    TaskCreationOptions
                        .RunContinuationsAsynchronously);

            Button? firstAvailableButton = null;

            foreach (DialogueOption option in options)
            {
                // The legacy Yarn presenter in the supplied project had
                // Show Unavailable Options disabled.
                if (!option.IsAvailable)
                {
                    continue;
                }

                DialogueOption capturedOption = option;

                var button = new Button
                {
                    text =
                        option.Line
                            .TextWithoutCharacterName
                            .Text
                };

                button.AddToClassList("hop-button");
                button.AddToClassList("hop-option-button");
                if (firstAvailableButton == null)
                {
                    firstAvailableButton = button;
                }

                button.clicked += () =>
                {
                    if (!capturedOption.IsAvailable)
                    {
                        return;
                    }

                    SetOptionButtonsEnabled(false);
                    completion.TrySetResult(
                        capturedOption);
                };

                optionsContainer.Add(button);
            }

            if (firstAvailableButton == null)
            {
                presentingOptions = false;
                optionsContainer.style.display =
                    DisplayStyle.None;
                lineCard.style.display =
                    DisplayStyle.None;
                return null;
            }

            firstAvailableButton.Focus();

            using (cancellationToken.Register(
                       () => completion.TrySetResult(null)))
            {
                DialogueOption? selected =
                    await completion.Task;

                presentingOptions = false;

                optionsContainer.style.display =
                    DisplayStyle.None;

                optionsContainer.Clear();
                lineCard.style.display =
                    DisplayStyle.None;

                return selected;
            }
        }

        private void SetOptionButtonsEnabled(
            bool enabled)
        {
            foreach (Button button in
                     optionsContainer
                         .Children()
                         .OfType<Button>())
            {
                button.SetEnabled(enabled);
            }
        }

        private void HideDialogue()
        {
            waitingForNextLine = false;
            presentingOptions = false;

            if (overlay != null)
            {
                overlay.style.display =
                    DisplayStyle.None;
            }

            if (optionsContainer != null)
            {
                optionsContainer.Clear();
            }
        }
    }
}
