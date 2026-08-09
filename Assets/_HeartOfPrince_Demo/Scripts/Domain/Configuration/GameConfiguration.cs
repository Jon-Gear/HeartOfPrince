using System;
using UnityEngine;

namespace HeartOfPrince.Domain
{
    [CreateAssetMenu(
        fileName = "Game Configuration",
        menuName = "Heart of Prince/Configuration/Game Configuration")]
    public sealed class GameConfiguration : ScriptableObject
    {
        [SerializeField] private string bootstrapScene = "Bootstrap";
        [SerializeField] private Chapter startingChapter;
        [SerializeField] private ActivityCatalog activityCatalog;
        [SerializeField] private GameStateDebugPreset initialStatePreset;

        public string BootstrapScene => bootstrapScene;
        public Chapter StartingChapter => startingChapter;
        public ActivityCatalog ActivityCatalog => activityCatalog;
        public GameStateDebugPreset InitialStatePreset => initialStatePreset;

        public ActivityDefinition FindActivityForScene(string sceneName)
        {
            ActivityDefinition activity =
                activityCatalog?.FindActivityForScene(sceneName);

            if (activity != null || startingChapter == null)
            {
                return activity;
            }

            foreach (Act act in startingChapter.Acts)
            {
                if (act?.ActivityCatalog == null)
                {
                    continue;
                }

                activity =
                    act.ActivityCatalog.FindActivityForScene(sceneName);

                if (activity != null)
                {
                    return activity;
                }
            }

            return null;
        }
    }
}
