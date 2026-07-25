#if UNITY_EDITOR

using HeartOfPrince.Presentation;
using UnityEditor;
using UnityEngine;

namespace HeartOfPrince.Editor
{
    [CustomEditor(typeof(GameLoopService))]
    public sealed class GameLoopServiceEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField(
                "Game Loop Configuration",
                EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("startAutomatically"));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("logTransitions"));

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField(
                "Live State",
                EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(true))
            {
                Draw("inspectorPhase");
                Draw("inspectorCurrentAct");
                Draw("inspectorCurrentDay");
                Draw("inspectorMinuteOfDay");
                Draw("inspectorActionsCompleted");
                Draw("inspectorCurrentActivity");
                Draw("inspectorDayEnding");
                Draw("inspectorGameComplete");
                Draw("inspectorStandaloneSceneMode");
                Draw("inspectorActiveScene");
            }

            serializedObject.ApplyModifiedProperties();

            var service = (GameLoopService)target;

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField(
                "Debug Controls",
                EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(
                       !UnityEngine.Application.isPlaying))
            {
                if (GUILayout.Button("Start New Game"))
                {
                    service.StartNewGame();
                }

                if (GUILayout.Button("Reset All Progression"))
                {
                    service.ResetAllProgression();
                }

                if (GUILayout.Button("Skip To Next Day"))
                {
                    service.DebugSkipToNextDay();
                }
            }

            if (!UnityEngine.Application.isPlaying)
            {
                EditorGUILayout.HelpBox(
                    "Activity definitions, scene variants, characters, " +
                    "and day rules are configured through the " +
                    "GameConfiguration asset in Resources/HeartOfPrince.",
                    MessageType.Info);
            }
        }

        private void Draw(string propertyName)
        {
            SerializedProperty property =
                serializedObject.FindProperty(propertyName);

            if (property != null)
            {
                EditorGUILayout.PropertyField(property);
            }
        }
    }
}

#endif
