using UnityEngine;

public class StartMonologueStep : ActivityStep
{
    public override void Start(CharacterBrain brain)
    {
        brain.Dialogue().TriggerMonologue(OnFinish);
        Debug.Log("Triggering monologue");
    }

    public override void Tick(CharacterBrain brain)
    {

    }

    private void OnFinish()
    {
        IsComplete = false;
    }
}
