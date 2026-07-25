using System.Collections.Generic;
using HeartOfPrince.Application;
using UnityEngine;
using UnityEngine.UIElements;

namespace HeartOfPrince.Presentation
{
    /// <summary>
    /// Data-driven activity decision screen implemented with UI Toolkit.
    /// It intentionally remains independent of specific activity types.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class DecisionScenePresenter : MonoBehaviour
    {
        private IReadOnlyList<ActivityOption> options;

        private UIDocument document;
        private PanelSettings ownedPanelSettings;
        private VisualElement overlay;
        private VisualElement activitiesContainer;
        private Label dayTimeLabel;
        private Label emptyMessageLabel;
        private Label feedbackLabel;

        private bool wasVisible;
        private int displayedDay = -1;
        private string displayedTime;

        private void Awake()
        {
            document =
                HeartOfPrinceUIToolkit.CreateDocument(
                    gameObject,
                    sortingOrder: 90,
                    out ownedPanelSettings);

            BuildVisualTree();
        }

        private void OnEnable()
        {
            Refresh();
            UpdateVisibility(force: true);
        }

        private void Update()
        {
            UpdateVisibility(force: false);
        }

        private void OnDisable()
        {
            if (overlay != null)
            {
                overlay.style.display =
                    DisplayStyle.None;
            }

            wasVisible = false;
        }

        private void OnDestroy()
        {
            HeartOfPrinceUIToolkit.DestroyPanelSettings(
                ownedPanelSettings);
        }

        public void Refresh()
        {
            options =
                GameSession.Instance?
                    .ActivityQuery?
                    .GetOptions();

            RebuildOptions();
        }

        private void BuildVisualTree()
        {
            VisualElement root =
                document.rootVisualElement;

            root.Clear();
            root.style.flexGrow = 1f;
            HeartOfPrinceUIToolkit.ApplySharedStyle(root);

            bool loaded =
                HeartOfPrinceUIToolkit.CloneTree(
                    root,
                    HeartOfPrinceUIToolkit.DecisionTreePath);

            if (!loaded)
            {
                BuildFallbackTree(root);
            }

            overlay =
                root.Q<VisualElement>("decision-overlay");

            activitiesContainer =
                root.Q<VisualElement>("activities");

            dayTimeLabel =
                root.Q<Label>("day-time");

            emptyMessageLabel =
                root.Q<Label>("empty-message");

            feedbackLabel =
                root.Q<Label>("feedback");

            if (overlay == null ||
                activitiesContainer == null ||
                dayTimeLabel == null ||
                emptyMessageLabel == null ||
                feedbackLabel == null)
            {
                Debug.LogError(
                    "[Decision UI] DecisionScreen.uxml is " +
                    "missing required named elements.");

                enabled = false;
                return;
            }

            overlay.style.display =
                DisplayStyle.None;
        }

        private static void BuildFallbackTree(
            VisualElement root)
        {
            var fallbackOverlay = new VisualElement
            {
                name = "decision-overlay"
            };
            fallbackOverlay.AddToClassList("hop-overlay");
            fallbackOverlay.AddToClassList(
                "hop-decision-overlay");

            var card = new VisualElement();
            card.AddToClassList("hop-card");
            card.AddToClassList("hop-decision-card");

            var kicker = new Label("DAILY RHYTHM");
            kicker.AddToClassList("hop-kicker");

            var dayTime = new Label
            {
                name = "day-time"
            };
            dayTime.AddToClassList("hop-eyebrow");

            var title = new Label(
                "What should Prince do?");
            title.AddToClassList("hop-title");

            var subtitle = new Label(
                "Choose how to spend the next part of the day.");
            subtitle.AddToClassList("hop-subtitle");

            var divider = new VisualElement();
            divider.AddToClassList("hop-divider");

            var activities = new VisualElement
            {
                name = "activities"
            };
            activities.AddToClassList("hop-options");
            activities.AddToClassList(
                "hop-activity-list");

            var emptyMessage = new Label
            {
                name = "empty-message"
            };
            emptyMessage.AddToClassList(
                "hop-empty-message");

            var feedback = new Label
            {
                name = "feedback"
            };
            feedback.AddToClassList("hop-feedback");

            card.Add(kicker);
            card.Add(dayTime);
            card.Add(title);
            card.Add(subtitle);
            card.Add(divider);
            card.Add(activities);
            card.Add(emptyMessage);
            card.Add(feedback);
            fallbackOverlay.Add(card);
            root.Add(fallbackOverlay);
        }

        private void UpdateVisibility(bool force)
        {
            GameLoopService loop =
                GameLoopService.Instance;

            bool shouldBeVisible =
                loop != null &&
                loop.Phase ==
                    HeartOfPrince.Domain
                        .GameLoopPhase
                        .AwaitingDecision;

            if (force ||
                shouldBeVisible != wasVisible)
            {
                overlay.style.display =
                    shouldBeVisible
                        ? DisplayStyle.Flex
                        : DisplayStyle.None;

                wasVisible = shouldBeVisible;

                if (shouldBeVisible)
                {
                    Refresh();
                }
            }

            if (!shouldBeVisible)
            {
                return;
            }

            string currentTime =
                loop.CurrentTimeDisplay;

            if (force ||
                displayedDay != loop.CurrentDay ||
                displayedTime != currentTime)
            {
                displayedDay = loop.CurrentDay;
                displayedTime = currentTime;

                dayTimeLabel.text =
                    $"Day {displayedDay} — " +
                    displayedTime;
            }
        }

        private void RebuildOptions()
        {
            if (activitiesContainer == null)
            {
                return;
            }

            activitiesContainer.Clear();

            bool hasOptions =
                options != null &&
                options.Count > 0;

            activitiesContainer.style.display =
                hasOptions
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;

            emptyMessageLabel.style.display =
                hasOptions
                    ? DisplayStyle.None
                    : DisplayStyle.Flex;

            emptyMessageLabel.text =
                hasOptions
                    ? string.Empty
                    : "No activities are currently available.";

            feedbackLabel.style.display =
                DisplayStyle.None;

            if (!hasOptions)
            {
                return;
            }

            Button firstAvailableButton = null;

            foreach (ActivityOption option in options)
            {
                ActivityOption capturedOption = option;

                var row = new VisualElement();
                row.AddToClassList(
                    "hop-activity-row");

                var button =
                    new Button(
                        () => SelectActivity(
                            capturedOption))
                    {
                        text = option.DisplayName
                    };

                button.AddToClassList("hop-button");
                button.AddToClassList(
                    "hop-option-button");

                button.userData = capturedOption;
                button.SetEnabled(option.IsAvailable);

                if (option.IsAvailable &&
                    firstAvailableButton == null)
                {
                    firstAvailableButton = button;
                }

                row.Add(button);

                if (!option.IsAvailable &&
                    !string.IsNullOrWhiteSpace(
                        option.UnavailableReason))
                {
                    var reason = new Label(
                        option.UnavailableReason);

                    reason.AddToClassList(
                        "hop-unavailable-reason");

                    row.Add(reason);
                }

                activitiesContainer.Add(row);
            }

            firstAvailableButton?.Focus();
        }

        private void SelectActivity(
            ActivityOption option)
        {
            if (option == null ||
                !option.IsAvailable)
            {
                return;
            }

            SetButtonsEnabled(false);

            feedbackLabel.text =
                $"Starting {option.DisplayName}…";

            feedbackLabel.style.display =
                DisplayStyle.Flex;

            GameLoopService loop =
                GameLoopService.Instance;

            if (loop == null)
            {
                feedbackLabel.text =
                    "The game loop is not available.";

                SetButtonsEnabled(true);
                return;
            }

            loop.RequestActivity(option);
        }

        private void SetButtonsEnabled(
            bool enabledState)
        {
            foreach (Button button in
                     activitiesContainer
                         .Query<Button>()
                         .ToList())
            {
                ActivityOption option =
                    button.userData as ActivityOption;

                button.SetEnabled(
                    enabledState &&
                    option != null &&
                    option.IsAvailable);
            }
        }
    }
}
