using System;
using HeartOfPrince.Domain;
using UnityEngine;

namespace HeartOfPrince.Application
{
    [CreateAssetMenu(
        fileName = "Talk Activity Module",
        menuName = "Heart of Prince/Activities/Modules/Talk")]
    public sealed class TalkActivityRuntimeModule :
        ActivityRuntimeModule
    {
        public override void Register(
            ActivityDefinition activity,
            ActivityCatalog catalog,
            ActivityService activityService,
            ActivityQueryService queryService)
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            if (catalog == null)
            {
                throw new ArgumentNullException(nameof(catalog));
            }

            if (activityService == null)
            {
                throw new ArgumentNullException(
                    nameof(activityService));
            }

            if (queryService == null)
            {
                throw new ArgumentNullException(
                    nameof(queryService));
            }

            activityService.Register(
                activity.Id,
                new TalkActivityHandler(catalog));

            queryService.Register(
                new TalkActivityOptionProvider(
                    activity,
                    catalog));
        }

        public override bool TryCreateRequestForScene(
            ActivityDefinition activity,
            ActivityCatalog catalog,
            string sceneName,
            out IActivityRequest request)
        {
            if (activity != null &&
                activity.TryGetTargetForScene(
                    sceneName,
                    out string characterId) &&
                !string.IsNullOrWhiteSpace(characterId))
            {
                request =
                    new ActivityRequest<TalkActivityInput>(
                        activity,
                        new TalkActivityInput(characterId));
                return true;
            }

            request = null;
            return false;
        }
    }
}
