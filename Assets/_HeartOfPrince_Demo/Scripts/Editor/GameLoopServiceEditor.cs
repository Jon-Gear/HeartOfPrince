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

            EditorGUILayout.LabelField("Game Loop Configuration", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("decisionsPerDay"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("daysPerAct"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("actsInDemo"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("startAutomatically"));
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("talkRoutes"),
                includeChildren: true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("logTransitions"));

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Live State", EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("inspectorPhase"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("inspectorCurrentAct"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("inspectorCurrentDay"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("inspectorDecisionIndex"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("inspectorActionRunning"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("inspectorDayEnding"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("inspectorGameComplete"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("inspectorStandaloneSceneMode"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("inspectorActiveScene"));
            }

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Debug Controls", EditorStyles.boldLabel);

            var service = (GameLoopService)target;
            using (new EditorGUI.DisabledScope(!UnityEngine.Application.isPlaying))
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
                    "Debug controls become available in Play Mode. " +
                    "The GameLoopService is created automatically on the persistent GameSession. " +
                    "Playing a non-Bootstrap scene enters standalone-scene mode instead of redirecting.",
                    MessageType.Info);
            }
        }
    }
}

#endif
