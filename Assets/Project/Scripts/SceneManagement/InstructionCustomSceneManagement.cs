using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using System;
using System.Threading.Tasks;
using UnityEngine;

using UnityEngine.SceneManagement;
using LoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode;


[Version(0, 1, 1)]

[Title("Load Scene with Screen Effects")]
[Description("Loads a new Scene with screen effects")]

[Category("Scenes/Load Scene With Screen Effects")]

[Parameter(
    "Scene",
    "The scene to be loaded"
)]

[Parameter(
    "Mode",
    "Single mode replaces all other scenes. Additive mode loads the scene on top of the others"
)]

[Parameter(
    "Async",
    "Loads the scene in the background or freeze the game until its done"
)]

[Parameter(
    "Scene Entries",
    "Define the starting location of the player and other characters after loading the scene"
)]

[Keywords("Change")]
[Image(typeof(IconUnity), ColorTheme.Type.Green)]

[Serializable]
public class FadeSceneLoad : Instruction
{
    // EXPOSED MEMBERS: -----------------------------------------------------------------------

    [SerializeField] private PropertyGetScene m_Scene = new PropertyGetScene();

    [SerializeField] private LoadSceneMode m_Mode = LoadSceneMode.Single;
    [SerializeField] private bool m_Async = false;

    [SerializeField] private SceneEntries m_SceneEntries = new SceneEntries();

    [Space]

    [SerializeField] private float m_Duration = 0.5f;

    // MEMBERS: -------------------------------------------------------------------------------

    private AsyncOperation m_Loader;

    // PROPERTIES: ----------------------------------------------------------------------------

    public override string Title => string.Format(
        "Load{0} scene {1}{2}",
        this.m_Mode == LoadSceneMode.Additive ? " additive" : string.Empty,
        this.m_Scene,
        this.m_Async ? " (async)" : string.Empty
    );

    // RUN METHOD: ----------------------------------------------------------------------------

    protected override async Task Run(Args args)
    {
        int scene = this.m_Scene.Get(args);
        this.m_SceneEntries.Schedule(scene, args);

        if (this.m_Async)
        {
            await GameManager.Instance.GetSystem<ScreenEffectsManager>().BasicFadeOut(m_Duration);

            this.m_Loader = SceneManager.LoadSceneAsync(scene, this.m_Mode);
            await this.Until(() => this.m_Loader.isDone || ApplicationManager.IsExiting);

            await GameManager.Instance.GetSystem<ScreenEffectsManager>().BasicFadeIn(m_Duration);
        }
        else
        {
            await GameManager.Instance.GetSystem<ScreenEffectsManager>().BasicFadeOut(m_Duration);
            SceneManager.LoadScene(scene, this.m_Mode);
            await GameManager.Instance.GetSystem<ScreenEffectsManager>().BasicFadeIn(m_Duration);
        }
    }
}