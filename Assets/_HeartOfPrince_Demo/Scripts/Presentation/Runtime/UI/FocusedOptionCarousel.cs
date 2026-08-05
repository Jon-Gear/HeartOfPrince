#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace HeartOfPrince.Presentation
{
    /// <summary>
    /// UI Toolkit controller for a focused vertical option carousel. It renders a
    /// small window around the selected item, so long lists never grow the UI.
    /// </summary>
    public sealed class FocusedOptionCarousel
    {
        private const int CenterSlot = 1;
        private const int VisibleOptionCount = 3;
        private const float SwipeThreshold = 28f;

        private readonly VisualElement panel;
        private readonly Label titleLabel;
        private readonly VisualElement viewport;
        private readonly Label emptyLabel;
        private readonly Label counterLabel;

        private OptionSelectionRequest? request;
        private IReadOnlyList<OptionSelectionItem> options =
            Array.Empty<OptionSelectionItem>();

        private int selectedIndex;
        private Vector2 pointerStart;
        private bool pointerPressed;

        public FocusedOptionCarousel(
            VisualElement panel,
            Label titleLabel,
            VisualElement viewport,
            Label emptyLabel,
            Label counterLabel)
        {
            this.panel = panel ?? throw new ArgumentNullException(nameof(panel));
            this.titleLabel = titleLabel ?? throw new ArgumentNullException(nameof(titleLabel));
            this.viewport = viewport ?? throw new ArgumentNullException(nameof(viewport));
            this.emptyLabel = emptyLabel ?? throw new ArgumentNullException(nameof(emptyLabel));
            this.counterLabel = counterLabel ?? throw new ArgumentNullException(nameof(counterLabel));

            this.panel.focusable = true;
            this.panel.RegisterCallback<KeyDownEvent>(OnKeyDown);
            this.panel.RegisterCallback<WheelEvent>(OnWheel);
            this.panel.RegisterCallback<PointerDownEvent>(OnPointerDown);
            this.panel.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            this.panel.RegisterCallback<PointerUpEvent>(OnPointerUp);
            this.panel.RegisterCallback<PointerCancelEvent>(OnPointerCancel);
        }

        public event Action<OptionSelectionItem>? OptionConfirmed;
        public event Action<OptionSelectionItem>? FocusedOptionChanged;

        public bool IsOpen => request != null;

        public void Show(OptionSelectionRequest selectionRequest)
        {
            request = selectionRequest ??
                throw new ArgumentNullException(nameof(selectionRequest));

            options = request.Options;
            selectedIndex = ResolveInitialIndex(request.InitialOptionId);

            panel.style.display = DisplayStyle.Flex;
            titleLabel.text = request.Title;

            Render();
            NotifyFocusedOptionChanged();
            panel.Focus();
        }

        public void Close()
        {
            request = null;
            options = Array.Empty<OptionSelectionItem>();
            viewport.Clear();
            panel.style.display = DisplayStyle.None;
        }

        private int ResolveInitialIndex(string? initialOptionId)
        {
            if (!string.IsNullOrWhiteSpace(initialOptionId))
            {
                int requestedIndex = options
                    .Select((option, index) => new { option, index })
                    .FirstOrDefault(entry => entry.option.Id == initialOptionId)
                    ?.index ?? -1;

                if (requestedIndex >= 0)
                {
                    return requestedIndex;
                }
            }

            int firstEnabledIndex = options
                .Select((option, index) => new { option, index })
                .FirstOrDefault(entry => entry.option.IsEnabled)
                ?.index ?? -1;

            return firstEnabledIndex >= 0 ? firstEnabledIndex : 0;
        }

        private void Render()
        {
            viewport.Clear();

            bool hasOptions = options.Count > 0;
            viewport.style.display = hasOptions
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            emptyLabel.style.display = hasOptions
                ? DisplayStyle.None
                : DisplayStyle.Flex;

            counterLabel.style.display = hasOptions
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            emptyLabel.text = request?.EmptyMessage ?? string.Empty;

            if (!hasOptions)
            {
                counterLabel.text = string.Empty;
                return;
            }

            selectedIndex = Mathf.Clamp(selectedIndex, 0, options.Count - 1);

            for (int slot = 0; slot < VisibleOptionCount; slot++)
            {
                int offset = slot - CenterSlot;
                int optionIndex = selectedIndex + offset;

                if (options.Count >= VisibleOptionCount)
                {
                    optionIndex =
                        (optionIndex % options.Count + options.Count) %
                        options.Count;
                }

                viewport.Add(
                    optionIndex >= 0 && optionIndex < options.Count
                        ? BuildOptionElement(optionIndex)
                        : BuildEmptySlot(Mathf.Abs(slot - CenterSlot)));
            }

            counterLabel.text =
                $"{selectedIndex + 1} / {options.Count}";
        }

        private VisualElement BuildOptionElement(int index)
        {
            OptionSelectionItem option = options[index];
            int distance = Mathf.Abs(index - selectedIndex);

            var button = new Button(() => OnOptionTapped(index))
            {
                focusable = true
            };

            button.AddToClassList("hop-bottom-option");
            button.EnableInClassList(
                "hop-bottom-option-focused",
                distance == 0);
            button.EnableInClassList(
                "hop-bottom-option-adjacent",
                distance == 1);
            button.EnableInClassList(
                "hop-bottom-option-edge",
                distance > 1);
            button.EnableInClassList(
                "hop-bottom-option-disabled",
                !option.IsEnabled);

            var content = new VisualElement();
            content.AddToClassList("hop-bottom-option-content");

            if (option.Icon != null)
            {
                var icon = new VisualElement();
                icon.AddToClassList("hop-bottom-option-icon");
                icon.style.backgroundImage =
                    new StyleBackground(option.Icon);
                content.Add(icon);
            }

            var textStack = new VisualElement();
            textStack.AddToClassList("hop-bottom-option-text-stack");

            if (!string.IsNullOrWhiteSpace(option.Category))
            {
                var category = new Label(option.Category);
                category.AddToClassList("hop-bottom-option-category");
                textStack.Add(category);
            }

            var text = new Label(option.DisplayText);
            text.AddToClassList("hop-bottom-option-text");
            textStack.Add(text);

            if (!string.IsNullOrWhiteSpace(option.RequirementLabel))
            {
                var requirement = new Label(option.RequirementLabel);
                requirement.AddToClassList("hop-bottom-option-requirement");
                textStack.Add(requirement);
            }

            content.Add(textStack);
            button.Add(content);

            button.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return ||
                    evt.keyCode == KeyCode.KeypadEnter ||
                    evt.keyCode == KeyCode.Space)
                {
                    OnOptionTapped(index);
                    evt.StopPropagation();
                }
            });

            return button;
        }

        private static VisualElement BuildEmptySlot(int distanceFromCenter)
        {
            var slot = new VisualElement();
            slot.AddToClassList("hop-bottom-option");
            slot.AddToClassList("hop-bottom-option-placeholder");
            slot.EnableInClassList(
                "hop-bottom-option-adjacent",
                distanceFromCenter == 1);
            slot.EnableInClassList(
                "hop-bottom-option-edge",
                distanceFromCenter > 1);
            return slot;
        }

        private void OnOptionTapped(int index)
        {
            if (index != selectedIndex)
            {
                selectedIndex = index;
                Render();
                NotifyFocusedOptionChanged();
                return;
            }

            ConfirmFocusedOption();
        }

        public void ConfirmFocusedOption()
        {
            if (options.Count == 0 ||
                selectedIndex < 0 ||
                selectedIndex >= options.Count)
            {
                return;
            }

            OptionSelectionItem option = options[selectedIndex];
            if (!option.IsEnabled)
            {
                return;
            }

            option.OnSelected?.Invoke(option);
            request?.OnSelected?.Invoke(option);
            OptionConfirmed?.Invoke(option);

            if (request?.CloseOnSelection == true)
            {
                Close();
            }
        }

        public void MoveSelection(int delta)
        {
            if (options.Count == 0)
            {
                return;
            }

            selectedIndex = options.Count >= VisibleOptionCount
                ? (selectedIndex + delta + options.Count) % options.Count
                : Mathf.Clamp(
                    selectedIndex + delta,
                    0,
                    options.Count - 1);

            Render();
            NotifyFocusedOptionChanged();
        }

        private void NotifyFocusedOptionChanged()
        {
            if (options.Count == 0 ||
                selectedIndex < 0 ||
                selectedIndex >= options.Count)
            {
                return;
            }

            FocusedOptionChanged?.Invoke(options[selectedIndex]);
        }

        public bool TryHandleKeyDown(KeyCode keyCode)
        {
            if (!IsOpen)
            {
                return false;
            }

            if (keyCode == KeyCode.UpArrow ||
                keyCode == KeyCode.W)
            {
                MoveSelection(-1);
                return true;
            }

            if (keyCode == KeyCode.DownArrow ||
                keyCode == KeyCode.S)
            {
                MoveSelection(1);
                return true;
            }

            if (keyCode == KeyCode.Return ||
                keyCode == KeyCode.KeypadEnter ||
                keyCode == KeyCode.Space)
            {
                ConfirmFocusedOption();
                return true;
            }

            return false;
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (!IsOpen)
            {
                return;
            }

            if (TryHandleKeyDown(evt.keyCode))
            {
                evt.StopPropagation();
            }
        }

        private void OnWheel(WheelEvent evt)
        {
            if (!IsOpen || Mathf.Approximately(evt.delta.y, 0f))
            {
                return;
            }

            MoveSelection(evt.delta.y > 0f ? 1 : -1);
            evt.StopPropagation();
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (!IsOpen)
            {
                return;
            }

            pointerPressed = true;
            pointerStart = evt.position;
            panel.CapturePointer(evt.pointerId);
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (!pointerPressed || !IsOpen)
            {
                return;
            }

            float deltaY = evt.position.y - pointerStart.y;
            if (Mathf.Abs(deltaY) < SwipeThreshold)
            {
                return;
            }

            MoveSelection(deltaY > 0f ? -1 : 1);
            pointerStart = evt.position;
            evt.StopPropagation();
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            pointerPressed = false;
            if (panel.HasPointerCapture(evt.pointerId))
            {
                panel.ReleasePointer(evt.pointerId);
            }
        }

        private void OnPointerCancel(PointerCancelEvent evt)
        {
            pointerPressed = false;
            if (panel.HasPointerCapture(evt.pointerId))
            {
                panel.ReleasePointer(evt.pointerId);
            }
        }
    }
}
