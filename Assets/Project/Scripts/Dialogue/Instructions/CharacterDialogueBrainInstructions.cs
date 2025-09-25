using GameCreator.Runtime.Common;
using GameCreator.Runtime.Dialogue;
using GameCreator.Runtime.VisualScripting;
using System;
using System.Threading.Tasks;
using UnityEngine;


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
        
        Debug.Log($"[InstructionTalkToCharacter] character: {character}");

        if (character == null) return DefaultResult;

        Debug.Log($"[InstructionTalkToCharacter] Starting dialogue with {characterName}");

        character.PlayerStartDialogueWithCharacter();
        return DefaultResult;
    }
}

[Title("Character Talk To You")]
[Description("This will return a topic the NPC wants to ask the player CharacterDialogueBrain")]
[Image(typeof(IconNodeText), ColorTheme.Type.Green)]
[Category("YarnSpinner/Dialogue/Character Talk To You")]
[Serializable]
public class InstructionCharacterTalkToYou : Instruction
{
    public string characterName = "Character";
    public override string Title => $"{characterName} Talk To You";
    protected override Task Run(Args args)
    {
        CharacterDialogueBrain character = CharacterManager.Instance.GetCharacter(characterName);

        if (character == null) return DefaultResult;

        character.CharacterStartDialogueWithPlayer();
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

[Title("Add Background Dialogue Topic")]
[Description("This will add a background dialogue topic to the CharacterDialogueBrain")]
[Image(typeof(IconPlus), ColorTheme.Type.Green)]
[Category("YarnSpinner/Misc/Add Background Dialogue Topic")]
[Serializable]
public class InstructionAddBackgroundDialogueTopic : Instruction
{
    [SerializeField] private PropertyGetGameObject target;
    [SerializeField] private BackgroundDialogueTopic topic = null;
    public override string Title => $"Add a background dialogue topic to nearby character";

    protected override Task Run(Args args)
    {
        GameObject gameObject = this.target.Get(args);

        if(gameObject == null) return DefaultResult;

        Actor actor = gameObject.GetComponent<Actor>();
        if(actor == null) return DefaultResult;

        CharacterDialogueBrain character = CharacterManager.Instance.GetCharacter(actor.actorName);

        if (character == null) return DefaultResult;


        // TODO: Fix THIS later
        //character.AddBackgroundDialogueTopic(topic);

        return DefaultResult;
    }
}

[Title("Remove Background Dialogue Topic")]
[Description("This will remove a background dialogue topic to the CharacterDialogueBrain")]
[Image(typeof(IconMinus), ColorTheme.Type.Red)]
[Category("YarnSpinner/Misc/Remove Background Dialogue Topic")]
[Serializable]
public class InstructionRemoveBackgroundDialogueTopic : Instruction
{
    [SerializeField] private PropertyGetGameObject target;
    [SerializeField] private BackgroundDialogueTopic topic = null;
    public override string Title => $"Add a background dialogue topic to nearby character";

    protected override Task Run(Args args)
    {
        GameObject gameObject = this.target.Get(args);

        if (gameObject == null) return DefaultResult;

        Actor actor = gameObject.GetComponent<Actor>();
        if (actor == null) return DefaultResult;

        CharacterDialogueBrain character = CharacterManager.Instance.GetCharacter(actor.actorName);

        if (character == null) return DefaultResult;

        // TODO: Fix THIS later
        //character.RemoveBackgroundDialogueTopic(topic);

        return DefaultResult;
    }
}

