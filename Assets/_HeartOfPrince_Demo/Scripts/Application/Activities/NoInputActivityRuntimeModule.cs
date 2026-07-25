using System;
using HeartOfPrince.Domain;
using UnityEngine;

namespace HeartOfPrince.Application
{
    [CreateAssetMenu(
        fileName = "No Input Activity Module",
        menuName = "Heart of Prince/Activities/Modules/No Input")]
    public sealed class NoInputActivityRuntimeModule :
        ActivityRuntimeModule
    {
        public override void Register(
            ActivityDefinition activity,
            ActivityCatalog catalog,
            ActivityService activityService,
            ActivityQueryService queryService)
        {
            ValidateArguments(
                activity,
                activityService,
                queryService);

            activityService.Register(
                activity.Id,
                new NoInputActivityHandler());

            queryService.Register(
                new NoInputActivityOptionProvider(activity));
        }

        public override bool TryCreateRequestForScene(
            ActivityDefinition activity,
            ActivityCatalog catalog,
            string sceneName,
            out IActivityRequest request)
        {
            if (activity != null &&
                activity.ContainsScene(sceneName))
            {
                request =
                    new ActivityRequest<NoActivityInput>(
                        activity,
                        NoActivityInput.Instance);
                return true;
            }

            request = null;
            return false;
        }

        private static void ValidateArguments(
            ActivityDefinition activity,
            ActivityService activityService,
            ActivityQueryService queryService)
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
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
        }
    }
}
