using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

public class GoToMarkerStep : ActivityStep
{
    Marker targetMarker;
    public GoToMarkerStep(Marker targetMarker)
    {
        this.targetMarker = targetMarker;
    }


    public override void Start(CharacterBrain brain)
    {
        brain.Actor().Character().Motion.MoveToMarker(targetMarker, 0.1f, OnMarkerReached);
    }

    public override void Tick(CharacterBrain brain)
    {
    }

    public override void Finish(CharacterBrain brain)
    {
        brain.Actor().Character().Motion.StopToDirection();
    }

    public void OnMarkerReached(Character character, bool hasReached)
    {
        if(hasReached)
        {
            IsComplete = true;
            Debug.Log("Destination reached");
        }
        else
        {
            IsComplete = false;
            Debug.Log("Destination not reached");
        }
    }

}
