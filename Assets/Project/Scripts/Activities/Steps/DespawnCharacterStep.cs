using UnityEngine;

public class DespawnCharacterStep : ActivityStep
{
    public override void Start(CharacterBrain brain)
    {
        brain.DespawnActor();
        IsComplete = true;
    }
    
    public override void Tick(CharacterBrain brain)
    {
    }

    public override void Finish(CharacterBrain brain)
    {
    }

}
