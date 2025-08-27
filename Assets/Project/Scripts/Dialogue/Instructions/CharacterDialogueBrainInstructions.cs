using GameCreator.Runtime.Common;
using GameCreator.Runtime.Dialogue;
using GameCreator.Runtime.VisualScripting;
using System;
using System.Threading.Tasks;


[Title("Talk To Character")]
[Description("This will return a weighted YarnSpinner dialogue based from CharacterDialogueBrain")]
[Image(typeof(IconNodeText), ColorTheme.Type.Green)]
[Category("YarnSpinner/Dialogue/Talk To Character")]
[Serializable]
public class InstructionTalkToCharacter : Instruction
{
    public string characterName = "Character";

    public override string Title => $"Talk To {characterName}";
    protected override Task Run(Args args)
    {
        CharacterDialogueBrain character = CharacterManager.Instance.GetCharacter(characterName); 
        
        if(character == null) return DefaultResult;

        character.StartDialogue();
        return DefaultResult;
    }
}

[Title("Start Background Dialogue Loop")]
[Description("This will return a YarnSpinner background dialogue based from CharacterDialogueBrain")]
[Image(typeof(IconNodeText), ColorTheme.Type.Green)]
[Category("YarnSpinner/Dialogue/Have Character Speak")]
[Serializable]
public class InstructionRunBackgroundDialogue : Instruction
{
    public string characterName = "Character";
    public override string Title => $"Have {characterName} Speak";
    protected override Task Run(Args args)
    {
        CharacterDialogueBrain character = CharacterManager.Instance.GetCharacter(characterName);

        if (character == null) return DefaultResult;

        character.StartBackgroundDialogueLoop();
        return DefaultResult;
    }
}

[Title("Stop Background Dialogue Loop")]
[Description("This will stop the background dilaogue loop from CharacterDialogueBrain")]
[Image(typeof(IconNodeText), ColorTheme.Type.Green)]
[Category("YarnSpinner/Dialogue/Have Character Stop Speaking")]
[Serializable]
public class InstructionStopBackgroundDialogue : Instruction
{
    public string characterName = "Character";
    public override string Title => $"Have {characterName} Stop Speaking";
    protected override Task Run(Args args)
    {
        CharacterDialogueBrain character = CharacterManager.Instance.GetCharacter(characterName);

        if (character == null) return DefaultResult;

        character.StopBackgroundDialogueLoop();
        return DefaultResult;
    }
}
