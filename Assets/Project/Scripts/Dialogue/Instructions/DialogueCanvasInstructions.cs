using GameCreator.Runtime.Behavior;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Dialogue;
using GameCreator.Runtime.VisualScripting;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Yarn.Unity;

[Title("Continue to Next Line")]
[Description("This will move on to the next line")]
[Image(typeof(IconArrowRight), ColorTheme.Type.Blue)]
[Category("YarnSpinner/Dialogue/Continue to Next Line")]
[Serializable]
public class ContinueToNextLine : Instruction
{
    [SerializeField] LineView lineView;
    protected override Task Run(Args args)
    {
        lineView.OnContinueClicked();
        return DefaultResult;
    }
}

[Title("Next Option")]
[Description("This will select the next option")]
[Image(typeof(IconNodeArrowRight), ColorTheme.Type.Blue)]
[Category("YarnSpinner/Dialogue/Next Option")]
[Serializable]
public class NextOption : Instruction
{
    [SerializeField] SingleOptionsBubble optionView;
    protected override Task Run(Args args)
    {
        optionView.NextOption();
        return DefaultResult;
    }
}
[Title("Previous Option")]
[Description("This will select the previous option")]
[Image(typeof(IconNodeArrowLeft), ColorTheme.Type.Blue)]
[Category("YarnSpinner/Dialogue/Previous Option")]
[Serializable]
public class PrevOption : Instruction
{
    [SerializeField] SingleOptionsBubble optionView;
    protected override Task Run(Args args)
    {
        optionView.PrevOption();
        return DefaultResult;
    }
}
