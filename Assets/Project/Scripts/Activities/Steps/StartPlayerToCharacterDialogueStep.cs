using UnityEngine;

public class StartPlayerToCharacterDialogueStep : ActivityStep
{
    
    public override void Start(CharacterBrain brain)
    {
        var dialogueManager = GameManager.Instance.GetSystem<DialogueManager>();
        var playerCharacter = GameManager.Instance.GetSystem<CharacterManager>().GetPlayerCharacter();

        if (playerCharacter.Traits().GetAttribute("attribute-energy") == 0.0f)
        {
            dialogueManager.StartDialogue("prince_tired");
            return;
        }

        brain.Dialogue().TriggerPlayerDialogueWithCharacter(OnFinish);
    }

    public override void Tick(CharacterBrain brain)
    {
    }

    public override void Finish(CharacterBrain brain)
    {
    }


    private void OnFinish()
    {
        IsComplete = true;
    }
}
