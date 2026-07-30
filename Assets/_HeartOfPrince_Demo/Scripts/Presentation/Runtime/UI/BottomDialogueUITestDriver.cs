#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

namespace HeartOfPrince.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BottomDialogueView))]
    public sealed class BottomDialogueUITestDriver : MonoBehaviour
    {
        [SerializeField]
        private DialogueRunner? dialogueRunner;

        private BottomDialogueView view = null!;
        private bool standaloneSelectionVisible;

        private void OnDestroy()
        {
            if (view != null)
            {
                view.ContinueRequested -= OnContinueRequested;
            }
        }

        private IEnumerator Start()
        {
            view = GetComponent<BottomDialogueView>();
            view.ContinueRequested += OnContinueRequested;
            dialogueRunner ??= GetComponent<DialogueRunner>();

            if (dialogueRunner == null)
            {
                yield break;
            }

            yield return new WaitUntil(() => dialogueRunner.IsDialogueRunning);
            yield return new WaitUntil(() => !dialogueRunner.IsDialogueRunning);
            yield return new WaitForSeconds(0.45f);

            ShowStandaloneDecisionOptions();
        }

        private void ShowStandaloneDecisionOptions()
        {
            var options = new List<OptionSelectionItem>
            {
                new(
                    "talk-munir",
                    "Talk with Munir",
                    "Activity",
                    requirementLabel: "Builds conversation topics"),
                new(
                    "ponder-courtyard",
                    "Ponder in the courtyard",
                    "Activity"),
                new(
                    "visit-youth-center",
                    "Visit the youth center",
                    "Activity"),
                new(
                    "help-kitchen",
                    "Help in the kitchen",
                    "Activity"),
                new(
                    "call-family",
                    "Call family",
                    "Activity"),
                new(
                    "late-night-walk",
                    "Take a late walk",
                    "Activity",
                    isEnabled: false,
                    requirementLabel: "Only available in the evening"),
                new(
                    "sleep",
                    "End the day",
                    "Activity")
            };

            var request = new OptionSelectionRequest(
                options,
                title: "Choose Activity",
                emptyMessage: "No activities are available.",
                closeOnSelection: false,
                onSelected: OnStandaloneOptionSelected);

            view.ClearDialogue();
            view.OpenOptions(request, keepDialogueVisible: false);
        }

        private void OnStandaloneOptionSelected(
            OptionSelectionItem option)
        {
            standaloneSelectionVisible = true;
            view.ShowDialogue(
                "Decision",
                $"Selected: {option.DisplayText}",
                "Tap to hide");
        }

        private void OnContinueRequested()
        {
            if (!standaloneSelectionVisible)
            {
                return;
            }

            standaloneSelectionVisible = false;
            view.HideAll();
        }
    }
}
