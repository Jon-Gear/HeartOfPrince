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

    public override void Finish(CharacterBrain brain)
    {
    }

    private void OnFinish()
    {
        Debug.Log("Finished monologue");
        IsComplete = true;
    }
}
