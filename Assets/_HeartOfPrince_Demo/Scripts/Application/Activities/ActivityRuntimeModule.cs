using HeartOfPrince.Domain;
using UnityEngine;

namespace HeartOfPrince.Application
{
    /// <summary>
    /// Unity-authored composition point for one activity type.
    ///
    /// A module owns the typed input/handler/provider wiring for its activity.
    /// GameSession only iterates configured activities; it never branches on
    /// activity IDs. New activities can therefore add a new module subclass
    /// without modifying the central game loop or session.
    /// </summary>
    public abstract class ActivityRuntimeModule : ScriptableObject
    {
        [SerializeField] private string moduleId;

        public string ModuleId => moduleId;

        public abstract void Register(
            ActivityDefinition activity,
            ActivityCatalog catalog,
            ActivityService activityService,
            ActivityQueryService queryService);

        public abstract bool TryCreateRequestForScene(
            ActivityDefinition activity,
            ActivityCatalog catalog,
            string sceneName,
            out IActivityRequest request);
    }
}
