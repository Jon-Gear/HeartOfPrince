using UnityEngine;

public class StartCharacterToPlayerDialogueStep : ActivityStep
{
    public override void Start(CharacterBrain brain)
    {
        brain.Dialogue().TriggerCharacterDialogueWithPlayer(OnFinish);

        Debug.Log("Starting dialogue");
    }

    private void OnFinish()
    {
        IsComplete = true;
        Debug.Log("Finished dialogue");
    }

    public override void Tick(CharacterBrain brain)
    {
    }

    public override void Finish(CharacterBrain brain)
    {
    }
}
