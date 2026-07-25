using UnityEngine;
using UnityEngine.UIElements;

namespace HeartOfPrince.Presentation
{
    /// <summary>
    /// Creates the runtime UI Toolkit panel used by dialogue and decisions.
    /// The visual trees and stylesheet live under Resources/HeartOfPrince/UI,
    /// so the presentation can be edited without changing gameplay code.
    /// </summary>
    internal static class HeartOfPrinceUIToolkit
    {
        internal const string PanelSettingsPath =
            "HeartOfPrince/UI/HeartOfPrincePanelSettings";

        internal const string SharedStylePath =
            "HeartOfPrince/UI/HeartOfPrinceRuntime";

        internal const string DialogueTreePath =
            "HeartOfPrince/UI/DialogueSystem";

        internal const string DecisionTreePath =
            "HeartOfPrince/UI/DecisionScreen";

        internal static UIDocument CreateDocument(
            GameObject owner,
            int sortingOrder,
            out PanelSettings ownedPanelSettings)
        {
            UIDocument document =
                owner.GetComponentInChildren<UIDocument>(
                    includeInactive: true);

            GameObject documentObject = null;

            if (document == null)
            {
                documentObject =
                    new GameObject("UI Toolkit Document");

                documentObject.transform.SetParent(
                    owner.transform,
                    worldPositionStays: false);

                documentObject.SetActive(false);
                document =
                    documentObject.AddComponent<UIDocument>();
            }

            PanelSettings panelTemplate =
                Resources.Load<PanelSettings>(
                    PanelSettingsPath);

            if (panelTemplate == null)
            {
                throw new System.InvalidOperationException(
                    $"Could not load PanelSettings asset " +
                    $"'{PanelSettingsPath}'. Make sure " +
                    "HeartOfPrincePanelSettings.asset is present " +
                    "under Resources/HeartOfPrince/UI.");
            }

            // Clone a serialized, fully configured asset instead of
            // constructing PanelSettings at runtime. Unity validates the
            // Theme Style Sheet during CreateInstance, before code can
            // assign one, which causes the panel to initialize incorrectly.
            ownedPanelSettings =
                Object.Instantiate(panelTemplate);

            ownedPanelSettings.name =
                $"{owner.name} UI Toolkit Panel";

            ownedPanelSettings.scaleMode =
                PanelScaleMode.ScaleWithScreenSize;

            ownedPanelSettings.referenceResolution =
                new Vector2Int(1920, 1080);

            ownedPanelSettings.screenMatchMode =
                PanelScreenMatchMode.MatchWidthOrHeight;

            ownedPanelSettings.match = 0.5f;

            if (ownedPanelSettings.themeStyleSheet == null)
            {
                throw new System.InvalidOperationException(
                    "HeartOfPrincePanelSettings has no Theme Style " +
                    "Sheet assigned. Reimport the supplied PanelSettings " +
                    "and UnityDefaultRuntimeTheme assets.");
            }

            document.panelSettings = ownedPanelSettings;
            document.sortingOrder = sortingOrder;

            if (documentObject != null)
            {
                documentObject.SetActive(true);
            }

            return document;
        }

        internal static bool CloneTree(
            VisualElement root,
            string resourcePath)
        {
            VisualTreeAsset tree =
                Resources.Load<VisualTreeAsset>(resourcePath);

            if (tree == null)
            {
                Debug.LogWarning(
                    $"[UI Toolkit] Could not load '{resourcePath}'. " +
                    "Using the code-generated fallback layout.");

                return false;
            }

            tree.CloneTree(root);
            return true;
        }

        internal static void ApplySharedStyle(
            VisualElement root)
        {
            StyleSheet styleSheet =
                Resources.Load<StyleSheet>(SharedStylePath);

            if (styleSheet == null)
            {
                Debug.LogWarning(
                    $"[UI Toolkit] Could not load stylesheet " +
                    $"'{SharedStylePath}'.");
                return;
            }

            root.styleSheets.Add(styleSheet);
        }

        internal static void DestroyPanelSettings(
            PanelSettings panelSettings)
        {
            if (panelSettings == null)
            {
                return;
            }

            if (UnityEngine.Application.isPlaying)
            {
                Object.Destroy(panelSettings);
            }
            else
            {
                Object.DestroyImmediate(panelSettings);
            }
        }
    }
}
