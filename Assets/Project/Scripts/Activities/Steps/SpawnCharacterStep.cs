using GameCreator.Runtime.Common;
using UnityEngine;

public class SpawnCharacterStep : ActivityStep
{
    Marker spawnMarker;

    public SpawnCharacterStep(Marker spawnMarker)
    {
        this.spawnMarker = spawnMarker;
    }

    public override void Start(CharacterBrain brain)
    {
        brain.SpawnActor(spawnMarker);
        IsComplete = true;
    }

    public override void Tick(CharacterBrain brain)
    {
    }

    public override void Finish(CharacterBrain brain)
    {
    }

}
