using UnityEngine;

public class StartCharacterToPlayerDialogueStep : ActivityStep
{
    public override void Start(CharacterBrain brain)
    {
        var dialogueManager = GameManager.Instance.GetSystem<DialogueManager>();

        dialogueManager.Primary().onDialogueComplete.AddListener(OnDialogueComplete);
        dialogueManager.Primary().StartDialogue(brain.Dialogue().ChooseCharacterToPlayerTopic());

        Debug.Log("Starting dialogue");
    }
    public override void Tick(CharacterBrain brain)
    {
    }

    public override void Finish(CharacterBrain brain)
    {
        var dialogueManager = GameManager.Instance.GetSystem<DialogueManager>();
        dialogueManager.Primary().onDialogueComplete.RemoveListener(OnDialogueComplete);
    }

    private void OnDialogueComplete()
    {
        IsComplete = true;
        Debug.Log("Finished dialogue");
    }

}
