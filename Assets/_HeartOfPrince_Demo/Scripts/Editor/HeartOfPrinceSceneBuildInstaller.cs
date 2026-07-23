#if UNITY_EDITOR

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HeartOfPrince.Editor
{
    [InitializeOnLoad]
    public static class HeartOfPrinceSceneBuildInstaller
    {
        private static readonly string[] DemoSceneNames =
        {
            "Bootstrap",
            "Chapter_1_Start",
            "Act_1_Start",
            "Conversation_Munir_Evening",
            "Day_Start",
            "Decision_Morning",
            "Ponder_Morning",
            "Decision_Evening",
            "Ponder_Evening",
            "Day_End",
            "Conversation_Munir_Morning",
            "Act_1_End",
            "Chapter_1_End"
        };

        static HeartOfPrinceSceneBuildInstaller()
        {
            EditorApplication.delayCall += EnsureDemoScenesAreInBuildSettings;
        }

        [MenuItem("Heart of Prince/Rebuild Demo Scene List")]
        public static void EnsureDemoScenesAreInBuildSettings()
        {
            var existing = EditorBuildSettings.scenes.ToList();

            var added = 0;
            foreach (var sceneName in DemoSceneNames)
            {
                var path = FindScenePath(sceneName);
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                var existingIndex = existing.FindIndex(
                    scene => string.Equals(scene.path, path, StringComparison.OrdinalIgnoreCase));

                if (existingIndex >= 0)
                {
                    if (!existing[existingIndex].enabled)
                    {
                        existing[existingIndex] = new EditorBuildSettingsScene(path, true);
                        added++;
                    }

                    continue;
                }

                existing.Add(new EditorBuildSettingsScene(path, true));
                added++;
            }

            if (added > 0)
            {
                EditorBuildSettings.scenes = existing.ToArray();
                Debug.Log($"[Heart of Prince] Added {added} demo scene(s) to Build Settings.");
            }
        }


        [MenuItem("Heart of Prince/Debug/Play Current Open Scene")]
        public static void UseCurrentSceneForPlayMode()
        {
            EditorSceneManager.playModeStartScene = null;
            Debug.Log(
                "[Heart of Prince] Play Mode will now start from the currently open scene. " +
                "Non-Bootstrap demo scenes enter standalone-scene mode.");
        }

        [MenuItem("Heart of Prince/Debug/Play Full Game From Bootstrap")]
        public static void UseBootstrapForPlayMode()
        {
            var path = FindScenePath("Bootstrap");
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("[Heart of Prince] Could not find Bootstrap.unity.");
                return;
            }

            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            EditorSceneManager.playModeStartScene = sceneAsset;
            Debug.Log(
                "[Heart of Prince] Play Mode will start from Bootstrap until " +
                "'Play Current Open Scene' is selected.");
        }

        [MenuItem("Heart of Prince/Open Starting Scene")]
        public static void OpenStartingScene()
        {
            var path = FindScenePath("Bootstrap");
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("[Heart of Prince] Could not find Bootstrap.unity.");
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            EditorSceneManager.OpenScene(path);
        }

        private static string FindScenePath(string sceneName)
        {
            return AssetDatabase.FindAssets($"{sceneName} t:Scene")
                .Select(AssetDatabase.GUIDToAssetPath)
                .FirstOrDefault(path =>
                    string.Equals(
                        Path.GetFileNameWithoutExtension(path),
                        sceneName,
                        StringComparison.OrdinalIgnoreCase) &&
                    path.Replace('\\', '/').Contains("/_HeartOfPrince_Demo/Scenes/"));
        }
    }
}

#endif
