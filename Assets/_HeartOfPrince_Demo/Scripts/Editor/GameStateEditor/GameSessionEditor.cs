#if UNITY_EDITOR

using HeartOfPrince.Domain;
using HeartOfPrince.Presentation;
using UnityEditor;
using UnityEngine;

namespace HeartOfPrince.Editor
{
    [CustomEditor(typeof(GameSession))]
    public sealed class GameSessionEditor :
        UnityEditor.Editor
    {
        private SerializedProperty configurationProperty;
        private SerializedProperty initialStatePresetProperty;

        private UnityEditor.Editor presetEditor;
        private bool showPreset = true;

        private void OnEnable()
        {
            configurationProperty =
                serializedObject.FindProperty(
                    "configuration");

            initialStatePresetProperty =
                serializedObject.FindProperty(
                    "initialStatePreset");
        }

        private void OnDisable()
        {
            if (presetEditor != null)
            {
                DestroyImmediate(presetEditor);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(
                configurationProperty);

            EditorGUILayout.PropertyField(
                initialStatePresetProperty);

            serializedObject.ApplyModifiedProperties();

            var preset =
                initialStatePresetProperty.objectReferenceValue
                as GameStateDebugPreset;

            if (preset == null)
            {
                EditorGUILayout.HelpBox(
                    "Assign a GameStateDebugPreset to configure " +
                    "the initial game state.",
                    MessageType.Info);

                return;
            }

            EditorGUILayout.Space(6);

            showPreset = EditorGUILayout.Foldout(
                showPreset,
                "Edit Initial State Preset",
                true);

            if (showPreset)
            {
                UnityEditor.Editor.CreateCachedEditor(
                    preset,
                    null,
                    ref presetEditor);

                using (new EditorGUILayout.VerticalScope(
                           EditorStyles.helpBox))
                {
                    if (presetEditor != null)
                    {
                        presetEditor.OnInspectorGUI();
                    }
                    else
                    {
                        EditorGUILayout.HelpBox(
                            $"Unity could not create an editor for " +
                            $"'{preset.name}'.",
                            MessageType.Error);
                    }
                }
            }

            EditorGUILayout.Space(6);

            using (new EditorGUI.DisabledScope(
                       !UnityEngine.Application.isPlaying))
            {
                if (GUILayout.Button(
                        "Apply Preset To Live Session"))
                {
                    var session = (GameSession)target;
                    session.Editor_ApplyPreset(preset);
                }
            }

            if (!UnityEngine.Application.isPlaying)
            {
                EditorGUILayout.HelpBox(
                    "Enter Play Mode to apply the preset to " +
                    "the live GameSession.",
                    MessageType.None);
            }
        }
    }
}

#endif