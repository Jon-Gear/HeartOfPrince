using UnityEngine;

public class StartCharacterToPlayerDialogueStep : ActivityStep
{
    public override void Start(CharacterBrain brain)
    {
        brain.Dialogue().TriggerCharacterDialogueWithPlayer(OnFinish);
    }

    private void OnFinish()
    {
        IsComplete = true;
    }

    public override void Tick(CharacterBrain brain)
    {
    }
}
