using UnityEngine;

public class StartMonologueStep : ActivityStep
{
    public override void Start(CharacterBrain brain)
    {
        var dialogueManager = GameManager.Instance.GetSystem<DialogueManager>();

        dialogueManager.Secondary().onDialogueComplete.AddListener(OnDialogueComplete);
        dialogueManager.Secondary().StartDialogue(brain.Dialogue().ChooseMonologueTopic());

        Debug.Log("Triggering monologue");
    }

    public override void Tick(CharacterBrain brain)
    {

    
    }

    public override void Finish(CharacterBrain brain)
    {
        var dialogueManager = GameManager.Instance.GetSystem<DialogueManager>();

        dialogueManager.Secondary().onDialogueComplete.RemoveListener(OnDialogueComplete);
    }

    private void OnDialogueComplete()
    {
        Debug.Log("Finished monologue");
        IsComplete = true;
    }
}
