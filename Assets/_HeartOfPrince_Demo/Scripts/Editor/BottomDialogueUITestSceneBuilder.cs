#nullable enable
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

namespace HeartOfPrince.Presentation.Editor
{
    public static class BottomDialogueUITestSceneBuilder
    {
        private const string ScenePath =
            "Assets/_HeartOfPrince_Demo/Scenes/UI/BottomDialogueUITest.unity";

        private const string YarnProjectPath =
            "Assets/_HeartOfPrince_Demo/Yarn/YarnSpinner2/HeartOfPrince.yarnproject";

        private const string DialoguePrefabPath =
            "Assets/_HeartOfPrince_Demo/Prefabs/BottomDialogueSystem.prefab";

        [MenuItem("Heart of Prince/UI/Build Bottom Dialogue Test Scene")]
        public static void BuildScene()
        {
            Scene scene = EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene,
                NewSceneMode.Single);

            CreateCamera();
            CreateEnvironment();
            CreateDialogueSystem();

            string directory = System.IO.Path.GetDirectoryName(ScenePath);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.Refresh();
            Debug.Log($"Built bottom dialogue UI test scene: {ScenePath}");
        }

        private static void CreateCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 2.1f, -7.5f);
            cameraObject.transform.rotation = Quaternion.Euler(12f, 0f, 0f);

            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.08f, 0.09f, 0.12f);
            camera.fieldOfView = 42f;

            cameraObject.AddComponent<AudioListener>();
        }

        private static void CreateEnvironment()
        {
            RenderSettings.ambientLight = new Color(0.34f, 0.32f, 0.30f);

            var lightObject = new GameObject("Key Light");
            lightObject.transform.rotation = Quaternion.Euler(45f, -30f, 0f);
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.15f;

            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Simple Environment Floor";
            floor.transform.localScale = new Vector3(4f, 1f, 3f);

            GameObject prince = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            prince.name = "Prince Test Stand-In";
            prince.transform.position = new Vector3(-1.1f, 1f, 0f);

            GameObject munir = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            munir.name = "Munir Test Stand-In";
            munir.transform.position = new Vector3(1.1f, 1f, 0f);

            GameObject backdrop = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backdrop.name = "Backdrop";
            backdrop.transform.position = new Vector3(0f, 1.6f, 1.8f);
            backdrop.transform.localScale = new Vector3(5f, 2.4f, 0.2f);
        }

        private static void CreateDialogueSystem()
        {
            GameObject prefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    DialoguePrefabPath);

            GameObject dialogueObject = prefab != null
                ? (GameObject)PrefabUtility.InstantiatePrefab(prefab)
                : new GameObject("BottomDialogueSystem");

            dialogueObject.name = "Bottom Dialogue Test System";

            DialogueRunner runner =
                dialogueObject.GetComponent<DialogueRunner>() ??
                dialogueObject.AddComponent<DialogueRunner>();

            InMemoryVariableStorage variableStorage =
                dialogueObject.GetComponent<InMemoryVariableStorage>() ??
                dialogueObject.AddComponent<InMemoryVariableStorage>();

            BottomDialogueView view =
                dialogueObject.GetComponent<BottomDialogueView>() ??
                dialogueObject.AddComponent<BottomDialogueView>();

            BottomDialogueYarnPresenter presenter =
                dialogueObject.GetComponent<BottomDialogueYarnPresenter>() ??
                dialogueObject.AddComponent<BottomDialogueYarnPresenter>();

            BottomDialogueUITestDriver driver =
                dialogueObject.GetComponent<BottomDialogueUITestDriver>() ??
                dialogueObject.AddComponent<BottomDialogueUITestDriver>();

            YarnProject yarnProject =
                AssetDatabase.LoadAssetAtPath<YarnProject>(YarnProjectPath);

            runner.VariableStorage = variableStorage;
            runner.DialoguePresenters =
                new List<DialoguePresenterBase> { presenter };
            runner.autoStart = true;
            runner.startNode = "BottomDialogueUITest";

            var serializedRunner = new SerializedObject(runner);
            serializedRunner.FindProperty("yarnProject").objectReferenceValue =
                yarnProject;
            serializedRunner.ApplyModifiedPropertiesWithoutUndo();

            var serializedDriver = new SerializedObject(driver);
            serializedDriver.FindProperty("dialogueRunner").objectReferenceValue =
                runner;
            serializedDriver.ApplyModifiedPropertiesWithoutUndo();

            _ = view;
        }
    }
}
