using System.Collections.Generic;
using HeartOfPrince.Application;
using UnityEngine;

namespace HeartOfPrince.Presentation
{
    /// <summary>
    /// Minimal data-driven decision UI. It deliberately has no knowledge of
    /// Talk, Ponder, characters, or future activity types.
    /// Replace this IMGUI presentation with the game's final UI without
    /// changing ActivityQueryService or GameLoopService.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class DecisionScenePresenter : MonoBehaviour
    {
        [SerializeField] private float panelWidth = 440f;

        private IReadOnlyList<ActivityOption> options;
        private string feedback;

        private void OnEnable()
        {
            Refresh();
        }

        public void Refresh()
        {
            options =
                GameSession.Instance?.ActivityQuery?.GetOptions();
            feedback = null;
        }

        private void OnGUI()
        {
            GameLoopService loop = GameLoopService.Instance;

            if (loop == null ||
                loop.Phase != HeartOfPrince.Domain.GameLoopPhase.AwaitingDecision)
            {
                return;
            }

            float width = Mathf.Min(panelWidth, Screen.width - 32f);

            GUILayout.BeginArea(
                new Rect(
                    (Screen.width - width) * 0.5f,
                    Mathf.Max(20f, Screen.height * 0.18f),
                    width,
                    Screen.height * 0.7f),
                GUI.skin.window);

            GUILayout.Label(
                $"Day {loop.CurrentDay} — {loop.CurrentTimeDisplay}",
                GUI.skin.box);
            GUILayout.Space(8f);
            GUILayout.Label("What should Prince do?");

            if (options == null)
            {
                Refresh();
            }

            if (options == null || options.Count == 0)
            {
                GUILayout.Label("No activities are currently available.");
            }
            else
            {
                foreach (ActivityOption option in options)
                {
                    bool previousEnabled = GUI.enabled;
                    GUI.enabled = option.IsAvailable;

                    if (GUILayout.Button(option.DisplayName))
                    {
                        feedback = null;
                        loop.RequestActivity(option);
                    }

                    GUI.enabled = previousEnabled;

                    if (!option.IsAvailable &&
                        !string.IsNullOrWhiteSpace(option.UnavailableReason))
                    {
                        GUILayout.Label(
                            $"  {option.UnavailableReason}",
                            GUI.skin.label);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(feedback))
            {
                GUILayout.Space(8f);
                GUILayout.Label(feedback);
            }

            GUILayout.EndArea();
        }
    }
}
