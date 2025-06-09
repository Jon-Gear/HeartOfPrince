using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Dialogue;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;
using Yarn.Unity;

[Title("Play Dialogue Yarn")]
[Description("This will play a given Yarn Spinner dialogue.")]
[Image(typeof(IconNodeText), ColorTheme.Type.Blue)]
[Category("YarnSpinner/Dialogue/Play Dialogue Yarn")]
[Serializable]
public class PlayDialogueYarn : Instruction
{
    //[SerializeField] private PropertyGetGameObject m_Dialogue = GetGameObjectDialogue.Create();
    [SerializeField] private bool m_WaitToFinish = true;
    public string startNodeName = "Start";

    public override string Title => string.Format(
            "Play {0} {1}",
            this.startNodeName, 
            this.m_WaitToFinish ? "and wait" : string.Empty
        );
    protected override Task Run(Args args)
    {
        DialogueManager.Instance.StartDialogue(startNodeName);
        return DefaultResult;
    }
}

[Title("Stop Dialogue Yarn")]
[Description("This will stop the current Yarn Spinner dialogue.")]
[Image(typeof(IconNodeText), ColorTheme.Type.Red, typeof(OverlayCross))]
[Category("YarnSpinner/Dialogue/Stop Dialogue Yarn")]
[Serializable]
public class StopDialogueYarn : Instruction
{
    protected override Task Run(Args args)
    {
        DialogueManager.Instance.StopDialogue();
        return DefaultResult;
    }
}


[Title("Set Yarn Project")]
[Description("This will set the current Yarn Project.")]
[Image(typeof(IconDialogue), ColorTheme.Type.Purple)]
[Category("YarnSpinner/Dialogue/Set Yarn Project")]
[Serializable]
public class SetYarnProject : Instruction
{
    public YarnProject yarnProject;
    protected override Task Run(Args args)
    {
        DialogueManager.Instance.SetYarnProject(yarnProject);
        return DefaultResult;
    }
}