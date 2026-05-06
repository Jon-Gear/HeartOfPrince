#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using HeartOfPrince.Domain;
using HeartOfPrince.Presentation;
using UnityEditor;
using UnityEngine;

namespace HeartOfPrince.Editor
{
    public sealed class GameSessionRuntimeMonitorWindow : EditorWindow
    {
        private GameSession targetSession;

        private Vector2 scroll;

        private bool gameStateFoldout = true;
        private bool playerToCharacterTopicsFoldout = true;
        private bool characterToPlayerTopicsFoldout = true;

        private int selectedCharacterIndex;

        private double lastRepaintTime;
        private const double RepaintInterval = 0.15f;

        [MenuItem("Heart of Prince/Game Session Runtime Monitor")]
        public static void Open()
        {
            GetWindow<GameSessionRuntimeMonitorWindow>("Game Session Monitor");
        }

        private void OnEnable()
        {
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            if (!Application.isPlaying)
                return;

            if (EditorApplication.timeSinceStartup - lastRepaintTime < RepaintInterval)
                return;

            lastRepaintTime = EditorApplication.timeSinceStartup;
            Repaint();
        }

        private void OnGUI()
        {
            DrawHeader();
            DrawTargetSessionField();

            EditorGUILayout.Space(8);

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox(
                    "Enter Play Mode to monitor the live GameSession.",
                    MessageType.Info);

                return;
            }

            if (targetSession == null)
            {
                DrawNoSessionState();
                return;
            }

            DrawSessionOverview();

            EditorGUILayout.Space(8);

            DrawGameState();
        }

        private void DrawHeader()
        {
            EditorGUILayout.LabelField(
                "Heart of Prince Runtime Monitor",
                EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Read-only live monitor for the selected GameSession. This window displays runtime GameState data and refreshes automatically during Play Mode.",
                MessageType.None);
        }

        private void DrawTargetSessionField()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                targetSession = (GameSession)EditorGUILayout.ObjectField(
                    "Target GameSession",
                    targetSession,
                    typeof(GameSession),
                    true);

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUI.enabled = Application.isPlaying && GameSession.Instance != null;

                    if (GUILayout.Button("Use GameSession.Instance"))
                    {
                        targetSession = GameSession.Instance;
                        Selection.activeObject = targetSession;
                    }

                    GUI.enabled = Application.isPlaying;

                    if (GUILayout.Button("Find In Scene"))
                    {
                        targetSession = FindObjectOfType<GameSession>();

                        if (targetSession != null)
                            Selection.activeObject = targetSession;
                    }

                    GUI.enabled = true;
                }
            }
        }

        private void DrawNoSessionState()
        {
            EditorGUILayout.HelpBox(
                "Drag a GameSession from the Hierarchy, or click 'Use GameSession.Instance' / 'Find In Scene'.",
                MessageType.Warning);
        }

        private void DrawSessionOverview()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Session", EditorStyles.boldLabel);

                DrawReadOnlyObjectField(
                    "GameObject",
                    targetSession.gameObject);

                DrawReadOnlyText(
                    "Active",
                    targetSession.gameObject.activeInHierarchy.ToString());

                DrawReadOnlyText(
                    "Scene",
                    targetSession.gameObject.scene.name);

                DrawReadOnlyText(
                    "Instance Match",
                    (GameSession.Instance == targetSession).ToString());

                DrawReadOnlyText(
                    "Has State",
                    (targetSession.State != null).ToString());
            }
        }

        private void DrawGameState()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                gameStateFoldout = EditorGUILayout.Foldout(
                    gameStateFoldout,
                    "Game State",
                    true);

                if (!gameStateFoldout)
                    return;

                var state = targetSession.State;

                if (state == null)
                {
                    EditorGUILayout.HelpBox(
                        "GameSession.State is null.",
                        MessageType.Warning);

                    return;
                }

                EditorGUILayout.Space(6);

                DrawCharacterArea(state);
            }
        }

        private void DrawCharacterArea(GameState state)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Characters", EditorStyles.boldLabel);

                if (state.CharactersTopics == null || state.CharactersTopics.Count == 0)
                {
                    EditorGUILayout.HelpBox(
                        "No characters have been added yet.",
                        MessageType.Info);

                    return;
                }

                var characterEntries = GetCharacterEntries(state);

                DrawCharacterSelector(characterEntries);

                EditorGUILayout.Space(8);

                scroll = EditorGUILayout.BeginScrollView(scroll);
                DrawSelectedCharacter(characterEntries);
                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawCharacterSelector(
            IReadOnlyList<KeyValuePair<CharacterID, CharacterTopicState>> characterEntries)
        {
            var characterNames = GetCharacterNames(characterEntries);

            selectedCharacterIndex = Mathf.Clamp(
                selectedCharacterIndex,
                0,
                characterNames.Length - 1);

            selectedCharacterIndex = EditorGUILayout.Popup(
                "Selected Character",
                selectedCharacterIndex,
                characterNames);
        }

        private void DrawSelectedCharacter(
            IReadOnlyList<KeyValuePair<CharacterID, CharacterTopicState>> characterEntries)
        {
            if (selectedCharacterIndex < 0 || selectedCharacterIndex >= characterEntries.Count)
                return;

            var selectedEntry = characterEntries[selectedCharacterIndex];
            var characterId = selectedEntry.Key;
            var topicState = selectedEntry.Value;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField(
                    characterId.ToString(),
                    EditorStyles.boldLabel);

                if (topicState == null)
                {
                    EditorGUILayout.HelpBox(
                        "CharacterTopicState is null.",
                        MessageType.Warning);

                    return;
                }

                DrawReadOnlyText(
                    "Character ID",
                    topicState.CharacterId.ToString());

                EditorGUILayout.Space(8);

                EditorGUILayout.LabelField(
                    "Character Topic State",
                    EditorStyles.boldLabel);

                DrawTopicList(
                    "Player → Character Topics",
                    topicState.PlayerToCharacterTopics,
                    ref playerToCharacterTopicsFoldout);

                EditorGUILayout.Space(4);

                DrawTopicList(
                    "Character → Player Topics",
                    topicState.CharacterToPlayerTopics,
                    ref characterToPlayerTopicsFoldout);

                EditorGUILayout.Space(8);

                EditorGUILayout.HelpBox(
                    "Runtime monitor is read-only. This shows the current live GameSession state.",
                    MessageType.None);
            }
        }

        private void DrawTopicList(
            string label,
            IReadOnlyList<TopicName> topics,
            ref bool foldout)
        {
            var count = topics == null ? 0 : topics.Count;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                foldout = EditorGUILayout.Foldout(
                    foldout,
                    $"{label} ({count})",
                    true);

                if (!foldout)
                    return;

                EditorGUI.indentLevel++;

                if (topics == null || topics.Count == 0)
                {
                    EditorGUILayout.HelpBox(
                        "No topics added.",
                        MessageType.Info);

                    EditorGUI.indentLevel--;
                    return;
                }

                for (int i = 0; i < topics.Count; i++)
                {
                    DrawReadOnlyText(
                        $"Topic {i + 1}",
                        topics[i].ToString());
                }

                EditorGUI.indentLevel--;
            }
        }

        private static List<KeyValuePair<CharacterID, CharacterTopicState>> GetCharacterEntries(
            GameState state)
        {
            return state.CharactersTopics
                .OrderBy(pair => pair.Key.ToString())
                .ToList();
        }

        private static string[] GetCharacterNames(
            IReadOnlyList<KeyValuePair<CharacterID, CharacterTopicState>> characterEntries)
        {
            var names = new string[characterEntries.Count];

            for (int i = 0; i < characterEntries.Count; i++)
            {
                names[i] = characterEntries[i].Key.ToString();
            }

            return names;
        }

        private static void DrawReadOnlyText(string label, string value)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.TextField(label, value ?? "null");
            }
        }

        private static void DrawReadOnlyObjectField(
            string label,
            UnityEngine.Object value)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField(
                    label,
                    value,
                    value != null ? value.GetType() : typeof(UnityEngine.Object),
                    true);
            }
        }
    }
}

#endif