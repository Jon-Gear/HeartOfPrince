#if UNITY_EDITOR

using HeartOfPrince.Domain;
using UnityEditor;
using UnityEngine;

namespace HeartOfPrince.Editor
{
    [CustomEditor(typeof(GameStateDebugPreset))]
    public sealed class GameStateDebugPresetEditor :
        UnityEditor.Editor
    {
        private SerializedProperty startingChapterProperty;
        private SerializedProperty startingActProperty;
        private SerializedProperty startingDayProperty;
        private SerializedProperty startingActionsCompletedProperty;
        private SerializedProperty startingMinuteProperty;

        private SerializedProperty ponderTopicsProperty;
        private SerializedProperty discussedPonderTopicsProperty;
        private SerializedProperty charactersProperty;

        private UnityEditor.Editor chapterEditor;

        private bool showChapterDefinition;
        private bool showPonder = true;
        private bool showCharacters = true;

        private void OnEnable()
        {
            startingChapterProperty =
                serializedObject.FindProperty("startingChapter");

            startingActProperty =
                serializedObject.FindProperty("startingAct");

            startingDayProperty =
                serializedObject.FindProperty("startingDay");

            startingActionsCompletedProperty =
                serializedObject.FindProperty(
                    "startingActionsCompleted");

            startingMinuteProperty =
                serializedObject.FindProperty(
                    "startingMinute");

            ponderTopicsProperty =
                serializedObject.FindProperty("ponderTopics");

            discussedPonderTopicsProperty =
                serializedObject.FindProperty(
                    "discussedPonderTopics");

            charactersProperty =
                serializedObject.FindProperty("characters");
        }

        private void OnDisable()
        {
            if (chapterEditor != null)
            {
                DestroyImmediate(chapterEditor);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawNarrativeSection();
            EditorGUILayout.Space(6);

            DrawPonderSection();
            EditorGUILayout.Space(6);

            DrawCharactersSection();

            bool changed =
                serializedObject.ApplyModifiedProperties();

            if (changed)
            {
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.Space(8);

            if (GUILayout.Button("Save Preset Asset"))
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }

        private void DrawNarrativeSection()
        {
            using var section =
                new EditorGUILayout.VerticalScope(
                    EditorStyles.helpBox);

            EditorGUILayout.LabelField(
                "Narrative Starting State",
                EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(
                startingChapterProperty);

            EditorGUILayout.PropertyField(
                startingActProperty);

            EditorGUILayout.PropertyField(
                startingDayProperty);

            EditorGUILayout.PropertyField(
                startingActionsCompletedProperty);

            EditorGUILayout.PropertyField(
                startingMinuteProperty);

            DrawChapterInline();
        }

        private void DrawChapterInline()
        {
            var chapter =
                startingChapterProperty.objectReferenceValue
                as Chapter;

            if (chapter == null)
            {
                EditorGUILayout.HelpBox(
                    "Assign a starting chapter.",
                    MessageType.Info);

                return;
            }

            showChapterDefinition = EditorGUILayout.Foldout(
                showChapterDefinition,
                "Edit Starting Chapter",
                true);

            if (!showChapterDefinition)
            {
                return;
            }

            UnityEditor.Editor.CreateCachedEditor(
                chapter,
                null,
                ref chapterEditor);

            using (new EditorGUI.IndentLevelScope())
            using (new EditorGUILayout.VerticalScope(
                       EditorStyles.helpBox))
            {
                chapterEditor.OnInspectorGUI();
            }
        }

        private void DrawPonderSection()
        {
            using var section =
                new EditorGUILayout.VerticalScope(
                    EditorStyles.helpBox);

            showPonder = EditorGUILayout.Foldout(
                showPonder,
                "Ponder State",
                true);

            if (!showPonder)
            {
                return;
            }

            EditorGUILayout.PropertyField(
                ponderTopicsProperty,
                new GUIContent("Available Topics"),
                true);

            EditorGUILayout.PropertyField(
                discussedPonderTopicsProperty,
                new GUIContent("Discussed Topics"),
                true);
        }

        private void DrawCharactersSection()
        {
            using var section =
                new EditorGUILayout.VerticalScope(
                    EditorStyles.helpBox);

            showCharacters = EditorGUILayout.Foldout(
                showCharacters,
                "Character State",
                true);

            if (!showCharacters)
            {
                return;
            }

            EditorGUILayout.PropertyField(
                charactersProperty,
                includeChildren: true);
        }
    }
}

#endif