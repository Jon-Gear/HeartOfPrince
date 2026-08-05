#nullable enable
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace HeartOfPrince.Presentation
{
    /// <summary>
    /// Non-Yarn bottom-screen dialogue and option-selection view. Yarn and other
    /// gameplay systems should talk to this view through general text/options.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class BottomDialogueView : MonoBehaviour
    {
        private const string TreePath =
            "HeartOfPrince/UI/BottomDialogue/BottomDialogue";

        private const string StylePath =
            "HeartOfPrince/UI/BottomDialogue/BottomDialogue";

        [Header("Layout")]
        [SerializeField, Range(0.16f, 0.35f)]
        private float maxScreenHeight = 0.24f;

        [SerializeField]
        private float minHeight = 230f;

        [SerializeField]
        private float maxHeight = 250f;

        [SerializeField]
        private float maxWidth = 1540f;

        [SerializeField]
        private float bottomMargin = 18f;

        [SerializeField]
        private float horizontalMargin = 22f;

        [SerializeField]
        private float optionWidth = 420f;

        private UIDocument document = null!;
        private PanelSettings ownedPanelSettings = null!;

        private VisualElement root = null!;
        private VisualElement overlay = null!;
        private VisualElement safeArea = null!;
        private VisualElement shell = null!;
        private VisualElement dialogueCard = null!;
        private Label speakerLabel = null!;
        private Label lineLabel = null!;
        private Label hintLabel = null!;
        private Button continueButton = null!;
        private VisualElement optionPanel = null!;
        private Label optionTitle = null!;
        private VisualElement optionViewport = null!;
        private Label optionEmpty = null!;
        private Label optionCounter = null!;

        private FocusedOptionCarousel carousel = null!;
        private bool optionsVisible;
        private bool showDialogueSection = true;

        public event Action? ContinueRequested;
        public event Action<OptionSelectionItem>? OptionConfirmed;
        public event Action<OptionSelectionItem>? FocusedOptionChanged;

        public bool HasDialogueText =>
            !string.IsNullOrWhiteSpace(lineLabel?.text);

        public bool IsOptionsOpen => carousel?.IsOpen == true;

        private void Awake()
        {
            document = HeartOfPrinceUIToolkit.CreateDocument(
                gameObject,
                sortingOrder: 110,
                out ownedPanelSettings);

            BuildVisualTree();
            HideAll();
        }

        private void Update()
        {
            ApplySafeAreaAndSize();
        }

        private void OnDestroy()
        {
            if (carousel != null)
            {
                carousel.OptionConfirmed -= OnCarouselOptionConfirmed;
                carousel.FocusedOptionChanged -= OnCarouselFocusedOptionChanged;
            }

            HeartOfPrinceUIToolkit.DestroyPanelSettings(
                ownedPanelSettings);
        }

        public void ShowDialogue(
            string? speaker,
            string text,
            string? hint = "Tap to continue")
        {
            overlay.style.display = DisplayStyle.Flex;
            shell.style.display = DisplayStyle.Flex;
            showDialogueSection = true;
            dialogueCard.style.display = DisplayStyle.Flex;

            speakerLabel.text = speaker ?? string.Empty;
            speakerLabel.style.display = string.IsNullOrWhiteSpace(speaker)
                ? DisplayStyle.None
                : DisplayStyle.Flex;

            lineLabel.text = text ?? string.Empty;
            hintLabel.text = hint ?? string.Empty;
            hintLabel.style.display = string.IsNullOrWhiteSpace(hint)
                ? DisplayStyle.None
                : DisplayStyle.Flex;

            continueButton.style.display = DisplayStyle.Flex;
            UpdateModeClasses();

            if (!optionsVisible)
            {
                root.Focus();
            }
        }

        public void SetDialogueTextImmediately(
            string? speaker,
            string text,
            string? hint = "Tap to continue")
        {
            ShowDialogue(speaker, text, hint);
        }

        public void ClearDialogue()
        {
            speakerLabel.text = string.Empty;
            lineLabel.text = string.Empty;
            hintLabel.text = string.Empty;
            speakerLabel.style.display = DisplayStyle.None;
            hintLabel.style.display = DisplayStyle.None;
            continueButton.style.display = DisplayStyle.None;
        }

        public void OpenOptions(
            OptionSelectionRequest request,
            bool keepDialogueVisible)
        {
            overlay.style.display = DisplayStyle.Flex;
            shell.style.display = DisplayStyle.Flex;

            showDialogueSection = keepDialogueVisible;
            dialogueCard.style.display = keepDialogueVisible
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            optionsVisible = true;
            optionPanel.style.display = DisplayStyle.Flex;
            carousel.Show(request);
            UpdateModeClasses();
        }

        public void CloseOptions()
        {
            optionsVisible = false;
            carousel.Close();
            optionPanel.style.display = DisplayStyle.None;
            showDialogueSection = true;
            dialogueCard.style.display = HasDialogueText
                ? DisplayStyle.Flex
                : DisplayStyle.None;
            UpdateModeClasses();
        }

        public void HideAll()
        {
            optionsVisible = false;
            showDialogueSection = true;

            if (carousel != null)
            {
                carousel.Close();
            }

            if (overlay != null)
            {
                overlay.style.display = DisplayStyle.None;
            }

            if (optionPanel != null)
            {
                optionPanel.style.display = DisplayStyle.None;
            }
        }

        private void BuildVisualTree()
        {
            root = document.rootVisualElement;
            root.Clear();
            root.focusable = true;
            root.style.flexGrow = 1f;

            StyleSheet styleSheet = Resources.Load<StyleSheet>(StylePath);
            if (styleSheet != null)
            {
                root.styleSheets.Add(styleSheet);
            }
            else
            {
                Debug.LogWarning(
                    $"[Bottom Dialogue UI] Could not load stylesheet '{StylePath}'.");
            }

            VisualTreeAsset tree = Resources.Load<VisualTreeAsset>(TreePath);
            if (tree != null)
            {
                tree.CloneTree(root);
            }
            else
            {
                BuildFallbackTree(root);
            }

            overlay = Required<VisualElement>("bottom-dialogue-overlay");
            safeArea = Required<VisualElement>("bottom-dialogue-safe-area");
            shell = Required<VisualElement>("bottom-dialogue-shell");
            dialogueCard = Required<VisualElement>("bottom-dialogue-card");
            speakerLabel = Required<Label>("bottom-speaker");
            lineLabel = Required<Label>("bottom-line");
            hintLabel = Required<Label>("bottom-hint");
            continueButton = Required<Button>("bottom-continue");
            optionPanel = Required<VisualElement>("bottom-option-panel");
            optionTitle = Required<Label>("bottom-options-title");
            optionViewport = Required<VisualElement>("bottom-options-viewport");
            optionEmpty = Required<Label>("bottom-options-empty");
            optionCounter = Required<Label>("bottom-options-counter");

            carousel = new FocusedOptionCarousel(
                optionPanel,
                optionTitle,
                optionViewport,
                optionEmpty,
                optionCounter);

            carousel.OptionConfirmed += OnCarouselOptionConfirmed;
            carousel.FocusedOptionChanged += OnCarouselFocusedOptionChanged;
            continueButton.clicked += () => ContinueRequested?.Invoke();
            dialogueCard.RegisterCallback<PointerUpEvent>(
                _ => ContinueRequested?.Invoke());
            root.RegisterCallback<KeyDownEvent>(
                OnRootKeyDown,
                TrickleDown.TrickleDown);
        }

        private void OnRootKeyDown(KeyDownEvent evt)
        {
            if (optionsVisible && carousel.TryHandleKeyDown(evt.keyCode))
            {
                evt.StopPropagation();
                return;
            }

            if (optionsVisible)
            {
                return;
            }

            if (evt.keyCode == KeyCode.Return ||
                evt.keyCode == KeyCode.KeypadEnter ||
                evt.keyCode == KeyCode.Space)
            {
                ContinueRequested?.Invoke();
                evt.StopPropagation();
            }
        }

        private T Required<T>(string elementName)
            where T : VisualElement
        {
            T element = root.Q<T>(elementName);
            if (element == null)
            {
                throw new InvalidOperationException(
                    $"BottomDialogue.uxml is missing '{elementName}'.");
            }

            return element;
        }

        private static void BuildFallbackTree(VisualElement root)
        {
            var overlay = new VisualElement { name = "bottom-dialogue-overlay" };
            overlay.AddToClassList("hop-bottom-overlay");

            var safe = new VisualElement { name = "bottom-dialogue-safe-area" };
            safe.AddToClassList("hop-bottom-safe-area");

            var shell = new VisualElement { name = "bottom-dialogue-shell" };
            shell.AddToClassList("hop-bottom-shell");

            var card = new VisualElement { name = "bottom-dialogue-card" };
            card.AddToClassList("hop-bottom-card");
            card.Add(new Label { name = "bottom-speaker" });
            card.Add(new Label { name = "bottom-line" });
            card.Add(new Label { name = "bottom-hint" });
            card.Add(new Button { name = "bottom-continue", text = "Continue" });

            var options = new VisualElement { name = "bottom-option-panel" };
            options.AddToClassList("hop-bottom-option-panel");
            options.Add(new Label { name = "bottom-options-title" });
            options.Add(new VisualElement { name = "bottom-options-viewport" });
            options.Add(new Label { name = "bottom-options-empty" });
            options.Add(new Label { name = "bottom-options-counter" });

            shell.Add(card);
            shell.Add(options);
            safe.Add(shell);
            overlay.Add(safe);
            root.Add(overlay);
        }

        private void OnCarouselOptionConfirmed(OptionSelectionItem option)
        {
            if (option == null)
            {
                return;
            }

            if (!optionsVisible)
            {
                return;
            }

            OptionConfirmed?.Invoke(option);
        }

        private void OnCarouselFocusedOptionChanged(
            OptionSelectionItem option)
        {
            if (option == null || !optionsVisible)
            {
                return;
            }

            FocusedOptionChanged?.Invoke(option);
        }

        private void UpdateModeClasses()
        {
            if (shell == null)
            {
                return;
            }

            shell.EnableInClassList(
                "hop-bottom-options-open",
                optionsVisible);

            shell.EnableInClassList(
                "hop-bottom-option-only",
                optionsVisible && !showDialogueSection);
        }

        private void ApplySafeAreaAndSize()
        {
            if (root == null || shell == null || safeArea == null)
            {
                return;
            }

            float rootWidth = root.resolvedStyle.width;
            float rootHeight = root.resolvedStyle.height;
            if (rootWidth <= 0f || rootHeight <= 0f)
            {
                return;
            }

            Rect safe = Screen.safeArea;
            float widthScale = rootWidth / Mathf.Max(1f, Screen.width);
            float heightScale = rootHeight / Mathf.Max(1f, Screen.height);

            safeArea.style.paddingLeft =
                horizontalMargin + safe.xMin * widthScale;

            safeArea.style.paddingRight =
                horizontalMargin + (Screen.width - safe.xMax) * widthScale;

            safeArea.style.paddingBottom =
                bottomMargin + safe.yMin * heightScale;

            float compactHeight = Mathf.Clamp(
                rootHeight * maxScreenHeight,
                minHeight,
                maxHeight);

            shell.style.height = compactHeight;
            shell.style.maxWidth = maxWidth;

            float responsiveOptionWidth = Mathf.Clamp(
                rootWidth * 0.32f,
                280f,
                optionWidth);

            optionPanel.style.width = optionsVisible
                ? responsiveOptionWidth
                : 0f;
        }
    }
}
