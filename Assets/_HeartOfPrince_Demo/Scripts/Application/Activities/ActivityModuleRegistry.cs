using System;
using System.Collections.Generic;
using HeartOfPrince.Domain;

namespace HeartOfPrince.Application
{
    /// <summary>
    /// Resolves Unity-authored activity runtime modules by stable module ID.
    /// The registry contains no knowledge of concrete activity types.
    /// </summary>
    public sealed class ActivityModuleRegistry
    {
        private readonly Dictionary<string, ActivityRuntimeModule> modules =
            new Dictionary<string, ActivityRuntimeModule>(
                StringComparer.OrdinalIgnoreCase);

        public ActivityModuleRegistry(
            IEnumerable<ActivityRuntimeModule> runtimeModules)
        {
            if (runtimeModules == null)
            {
                throw new ArgumentNullException(
                    nameof(runtimeModules));
            }

            foreach (ActivityRuntimeModule module in runtimeModules)
            {
                if (module == null)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(module.ModuleId))
                {
                    throw new InvalidOperationException(
                        $"Activity runtime module '{module.name}' " +
                        "has an empty module ID.");
                }

                if (modules.ContainsKey(module.ModuleId))
                {
                    throw new InvalidOperationException(
                        $"More than one activity runtime module uses " +
                        $"the ID '{module.ModuleId}'.");
                }

                modules.Add(module.ModuleId.Trim(), module);
            }
        }

        public ActivityRuntimeModule Find(string moduleId)
        {
            if (string.IsNullOrWhiteSpace(moduleId))
            {
                return null;
            }

            modules.TryGetValue(
                moduleId.Trim(),
                out ActivityRuntimeModule module);

            return module;
        }

        public void Configure(
            ActivityCatalog catalog,
            ActivityService activityService,
            ActivityQueryService queryService)
        {
            if (catalog == null)
            {
                throw new ArgumentNullException(nameof(catalog));
            }

            ActivityDefinition[] activities = catalog.Activities;

            if (activities == null)
            {
                return;
            }

            foreach (ActivityDefinition activity in activities)
            {
                if (activity == null)
                {
                    continue;
                }

                ActivityRuntimeModule module =
                    Find(activity.RuntimeModuleId);

                if (module == null)
                {
                    throw new InvalidOperationException(
                        $"Activity '{activity.Id}' references missing " +
                        $"runtime module '{activity.RuntimeModuleId}'.");
                }

                module.Register(
                    activity,
                    catalog,
                    activityService,
                    queryService);
            }
        }

        public bool TryCreateRequestForScene(
            ActivityDefinition activity,
            ActivityCatalog catalog,
            string sceneName,
            out IActivityRequest request)
        {
            request = null;

            if (activity == null)
            {
                return false;
            }

            ActivityRuntimeModule module =
                Find(activity.RuntimeModuleId);

            return module != null &&
                   module.TryCreateRequestForScene(
                       activity,
                       catalog,
                       sceneName,
                       out request);
        }
    }
}
