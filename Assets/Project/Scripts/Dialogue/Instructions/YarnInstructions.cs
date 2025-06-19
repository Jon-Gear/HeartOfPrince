using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Dialogue;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;
using Yarn.Unity;



[Title("Play Dialogue")]
[Description("This will play a given Yarn Spinner dialogue.")]
[Image(typeof(IconNodeText), ColorTheme.Type.Blue)]
[Category("YarnSpinner/Dialogue/Play Dialogue")]
[Serializable]
public class PlayDialogueYarn : Instruction
{
    //[SerializeField] private PropertyGetGameObject m_Dialogue = GetGameObjectDialogue.Create();
    [SerializeField] private CharacterDialogue characterDialogue;
    
    public override string Title => string.Format(
            "Play Dialogue"
        );
    protected override Task Run(Args args)
    {
        characterDialogue.Talk();
        return DefaultResult;
    }
}

[Title("Stop Dialogue")]
[Description("This will stop the current Yarn Spinner dialogue.")]
[Image(typeof(IconNodeText), ColorTheme.Type.Red, typeof(OverlayCross))]
[Category("YarnSpinner/Dialogue/Stop Dialogue")]
[Serializable]
public class StopDialogueYarn : Instruction
{
    protected override Task Run(Args args)
    {
        DialogueManager.Instance.StopDialogue();
        return DefaultResult;
    }
}

[Title("Play Comment Monologue")]
[Description("Plays a Yarn Spinner monologue for internal character commentary.")]
[Image(typeof(IconNodeText), ColorTheme.Type.Teal)]
[Category("YarnSpinner/Monologue/Play Comment Monologue")]
[Serializable]
public class PlayCommentMonologue : Instruction
{
    [SerializeField] private CommentMonologue commentMonologue;
    [SerializeField] private bool waitToFinish = true;

    public override string Title => string.Format("Play monologue");

    protected override Task Run(Args args)
    {
        commentMonologue.Comment();
        return DefaultResult;
    }
}


[Title("Stop Inner Monologue")]
[Description("Stops the currently playing Yarn Spinner inner monologue.")]
[Image(typeof(IconNodeText), ColorTheme.Type.Gray, typeof(OverlayCross))]
[Category("YarnSpinner/Monologue/Stop Inner Monologue")]
[Serializable]
public class StopInnerMonologue : Instruction
{
    protected override Task Run(Args args)
    {
        DialogueManager.Instance.StopInnerMonologue();
        return DefaultResult;
    }
}
