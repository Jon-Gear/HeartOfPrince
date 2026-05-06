#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using HeartOfPrince.Domain;
using HeartOfPrince.Presentation;
using UnityEditor;
using UnityEngine;
using Yarn.Unity;

namespace HeartOfPrince.Editor
{


public sealed class GameStateEditorWindow : EditorWindow
{
    private GameStateDebugPreset preset;
    private GameSession targetSession;

    private SerializedObject serializedPreset;
    private SerializedProperty yarnProjectProperty;
    private SerializedProperty charactersProperty;

    private int selectedCharacterIndex;
    private Vector2 scroll;

    private string newCharacterId = "";
    
    private bool playerToCharacterTopicsFoldout = true;
    private bool characterToPlayerTopicsFoldout = true;

    [MenuItem("Heart of Prince/Game State Editor")]
    public static void Open()
    {
        GetWindow<GameStateEditorWindow>("Game State Editor");
    }

    private void OnGUI()
    {
        DrawTopBar();

        if (preset == null)
        {
            DrawNoPresetState();
            return;
        }

        BindSerializedObject();

        serializedPreset.Update();

        DrawApplyButtons();

        EditorGUILayout.Space(8);

        DrawCharacterArea();

        serializedPreset.ApplyModifiedProperties();
    }

    private void DrawTopBar()
    {
        EditorGUILayout.LabelField("Heart of Prince Game State Editor", EditorStyles.boldLabel);

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            preset = (GameStateDebugPreset)EditorGUILayout.ObjectField(
                "Debug Preset",
                preset,
                typeof(GameStateDebugPreset),
                false);

            targetSession = (GameSession)EditorGUILayout.ObjectField(
                "Target Session",
                targetSession,
                typeof(GameSession),
                true);
        }
    }

    private void DrawNoPresetState()
    {
        EditorGUILayout.HelpBox(
            "Assign a GameStateDebugPreset, or create a new one.",
            MessageType.Info);

        if (GUILayout.Button("Create New Game State Debug Preset"))
        {
            CreateNewPreset();
        }
    }

    private void BindSerializedObject()
    {
        if (serializedPreset != null && serializedPreset.targetObject == preset)
            return;

        serializedPreset = new SerializedObject(preset);
        yarnProjectProperty = serializedPreset.FindProperty("yarnProject");
        charactersProperty = serializedPreset.FindProperty("characters");

        selectedCharacterIndex = Mathf.Clamp(
            selectedCharacterIndex,
            0,
            Mathf.Max(0, charactersProperty.arraySize - 1));
    }

    

    private void DrawApplyButtons()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            GUI.enabled = targetSession != null && preset != null;

            if (GUILayout.Button("Apply Preset To Live GameSession"))
            {
                Undo.RecordObject(targetSession, "Apply Game State Preset");
                targetSession.Editor_ApplyPreset(preset);
                EditorUtility.SetDirty(targetSession);
            }

            GUI.enabled = true;

            if (GUILayout.Button("Save Preset Asset"))
            {
                serializedPreset.ApplyModifiedProperties();
                EditorUtility.SetDirty(preset);
                AssetDatabase.SaveAssets();
            }
        }
    }

    private void DrawCharacterArea()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("Characters", EditorStyles.boldLabel);

            DrawAddCharacterControls();

            if (charactersProperty.arraySize == 0)
            {
                EditorGUILayout.HelpBox(
                    "No characters have been added yet.",
                    MessageType.Info);

                return;
            }

            DrawCharacterSelector();

            EditorGUILayout.Space(8);

            scroll = EditorGUILayout.BeginScrollView(scroll);
            DrawSelectedCharacter();
            EditorGUILayout.EndScrollView();
        }
    }

    private void DrawAddCharacterControls()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            newCharacterId = EditorGUILayout.TextField("New Character", newCharacterId);

            GUI.enabled = !string.IsNullOrWhiteSpace(newCharacterId);

            if (GUILayout.Button("Add Character", GUILayout.Width(130)))
            {
                AddCharacter(newCharacterId.Trim());
                newCharacterId = "";
            }

            GUI.enabled = true;
        }
    }

    private void DrawCharacterSelector()
    {
        var characterNames = GetCharacterNames();

        selectedCharacterIndex = Mathf.Clamp(
            selectedCharacterIndex,
            0,
            characterNames.Length - 1);

        selectedCharacterIndex = EditorGUILayout.Popup(
            "Selected Character",
            selectedCharacterIndex,
            characterNames);
    }

    private void DrawSelectedCharacter()
{
    if (selectedCharacterIndex < 0 || selectedCharacterIndex >= charactersProperty.arraySize)
        return;

    var characterProperty = charactersProperty.GetArrayElementAtIndex(selectedCharacterIndex);

    var characterIdProperty = characterProperty.FindPropertyRelative("characterId");
    var topicStateProperty = characterProperty.FindPropertyRelative("topicState");

    var playerToCharacterTopicsProperty =
        topicStateProperty.FindPropertyRelative("playerToCharacterTopics");

    var characterToPlayerTopicsProperty =
        topicStateProperty.FindPropertyRelative("characterToPlayerTopics");

    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField(
                string.IsNullOrWhiteSpace(characterIdProperty.stringValue)
                    ? "Unnamed Character"
                    : characterIdProperty.stringValue,
                EditorStyles.boldLabel);

            if (GUILayout.Button("Remove Character", GUILayout.Width(150)))
            {
                charactersProperty.DeleteArrayElementAtIndex(selectedCharacterIndex);
                selectedCharacterIndex = Mathf.Clamp(
                    selectedCharacterIndex,
                    0,
                    Mathf.Max(0, charactersProperty.arraySize - 1));

                return;
            }
        }

        EditorGUILayout.PropertyField(
            characterIdProperty,
            new GUIContent("Character ID"));

        EditorGUILayout.Space(8);

        EditorGUILayout.LabelField("Character Topic State", EditorStyles.boldLabel);

        DrawTopicList(
            "Player → Character Topics",
            playerToCharacterTopicsProperty,
            ref playerToCharacterTopicsFoldout);

        EditorGUILayout.Space(4);

        DrawTopicList(
            "Character → Player Topics",
            characterToPlayerTopicsProperty,
            ref characterToPlayerTopicsFoldout);

        EditorGUILayout.Space(8);

        EditorGUILayout.HelpBox(
            "Future character data can be added here later.",
            MessageType.None);
    }
}

    private void DrawTopicList(
        string label,
        SerializedProperty topicListProperty,
        ref bool foldout)
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                foldout = EditorGUILayout.Foldout(
                    foldout,
                    $"{label} ({topicListProperty.arraySize})",
                    true);

                if (GUILayout.Button("+", GUILayout.Width(28)))
                {
                    topicListProperty.arraySize++;

                    var newTopicProperty =
                        topicListProperty.GetArrayElementAtIndex(topicListProperty.arraySize - 1);

                    newTopicProperty.stringValue = "";
                    foldout = true;
                }

                GUI.enabled = topicListProperty.arraySize > 0;

                if (GUILayout.Button("Clear", GUILayout.Width(55)))
                {
                    topicListProperty.ClearArray();
                }

                GUI.enabled = true;
            }

            if (!foldout)
                return;

            EditorGUI.indentLevel++;

            if (topicListProperty.arraySize == 0)
            {
                EditorGUILayout.HelpBox(
                    "No topics added.",
                    MessageType.Info);
            }

            for (int i = 0; i < topicListProperty.arraySize; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    var topicProperty = topicListProperty.GetArrayElementAtIndex(i);

                    topicProperty.stringValue = EditorGUILayout.TextField(
                        $"Topic {i + 1}",
                        topicProperty.stringValue);

                    if (GUILayout.Button("-", GUILayout.Width(28)))
                    {
                        topicListProperty.DeleteArrayElementAtIndex(i);
                        break;
                    }
                }
            }

            EditorGUI.indentLevel--;
        }
    }

    private void DrawTopicPopup(SerializedProperty topicProperty, string[] nodeNames)
    {
        if (nodeNames.Length == 0)
        {
            topicProperty.stringValue = EditorGUILayout.TextField(topicProperty.stringValue);
            return;
        }

        var currentIndex = System.Array.IndexOf(nodeNames, topicProperty.stringValue);

        if (currentIndex < 0)
        {
            var mixedOptions = new List<string>
            {
                topicProperty.stringValue
            };

            mixedOptions.AddRange(nodeNames);

            var selectedIndex = EditorGUILayout.Popup(0, mixedOptions.ToArray());

            topicProperty.stringValue = mixedOptions[selectedIndex];

            return;
        }

        var newIndex = EditorGUILayout.Popup(currentIndex, nodeNames);

        topicProperty.stringValue = nodeNames[newIndex];
    }

    private void AddCharacter(string characterId)
    {
        if (string.IsNullOrWhiteSpace(characterId))
            return;

        for (int i = 0; i < charactersProperty.arraySize; i++)
        {
            var existingCharacter = charactersProperty.GetArrayElementAtIndex(i);
            var existingId = existingCharacter
                .FindPropertyRelative("characterId")
                .stringValue;

            if (existingId == characterId)
            {
                selectedCharacterIndex = i;
                return;
            }
        }

        charactersProperty.arraySize++;

        var newCharacter = charactersProperty.GetArrayElementAtIndex(charactersProperty.arraySize - 1);

        newCharacter.FindPropertyRelative("characterId").stringValue = characterId;

        var topicState = newCharacter.FindPropertyRelative("topicState");

        topicState
            .FindPropertyRelative("playerToCharacterTopics")
            .ClearArray();

        topicState
            .FindPropertyRelative("characterToPlayerTopics")
            .ClearArray();

        selectedCharacterIndex = charactersProperty.arraySize - 1;
    }

    private string[] GetCharacterNames()
    {
        var names = new string[charactersProperty.arraySize];

        for (int i = 0; i < charactersProperty.arraySize; i++)
        {
            var character = charactersProperty.GetArrayElementAtIndex(i);
            var characterId = character.FindPropertyRelative("characterId").stringValue;

            names[i] = string.IsNullOrWhiteSpace(characterId)
                ? $"Unnamed Character {i + 1}"
                : characterId;
        }

        return names;
    }

    private static string[] GetNodeNames(YarnProject yarnProject)
    {
        if (yarnProject == null)
            return System.Array.Empty<string>();

        return yarnProject.NodeNames
            .Where(nodeName => !string.IsNullOrWhiteSpace(nodeName))
            .OrderBy(nodeName => nodeName)
            .ToArray();
    }

    private void CreateNewPreset()
    {
        var path = EditorUtility.SaveFilePanelInProject(
            "Create Game State Debug Preset",
            "GameStateDebugPreset",
            "asset",
            "Choose where to save the preset.");

        if (string.IsNullOrWhiteSpace(path))
            return;

        var newPreset = CreateInstance<GameStateDebugPreset>();

        AssetDatabase.CreateAsset(newPreset, path);
        AssetDatabase.SaveAssets();

        preset = newPreset;
        Selection.activeObject = newPreset;
    }
}

}

#endif