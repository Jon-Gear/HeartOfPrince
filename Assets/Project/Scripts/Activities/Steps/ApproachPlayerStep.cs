using GameCreator.Runtime.Characters;
using UnityEngine;

public class ApproachPlayerStep : ActivityStep
{
    private Character playerCharacter;
    private const float STOP_DISTANCE = 1.5f;

    public override void Start(CharacterBrain brain)
    {
        playerCharacter = GameManager.Instance.GetSystem<ActorRegistry>().GetActorByName("Prince").Character();
        var myCharacter = brain.Actor().Character();


        myCharacter.Motion.StartFollowingTarget(playerCharacter.transform, 1f, 2f);
        brain.Dialogue().SetIntention(DialogueIntention.ApproachingPlayer);
    }

    public override void Tick(CharacterBrain brain)
    {
        Character myCharacter = brain.Actor().Character();

        float distance = Vector3.Distance(myCharacter.transform.position, playerCharacter.transform.position);

        if (distance < STOP_DISTANCE)
        {
            IsComplete = true;
        }
    }

    public override void Finish(CharacterBrain brain)
    {
        Character myCharacter = brain.Actor().Character();

        myCharacter.Motion.StopFollowingTarget();
        brain.Dialogue().ClearIntention();
    }

    
}
