using GameCreator.Runtime.Common;
using GameCreator.Runtime.Dialogue;
using GameCreator.Runtime.VisualScripting;
using System;

[Title("Is Dialogue Running")]
[Description("This return if the dialogue is running")]
[Image(typeof(IconNodeText), ColorTheme.Type.Yellow)]
[Category("YarnSpinner/Dialogue/Is Dialogue Running")]
[Serializable]
public class ConditionIsDialogueRunning : Condition
{
    protected override bool Run(Args args)
    {
        return false;
        //return DialogueManager.Instance.IsDialogueRunning();
    }
}
