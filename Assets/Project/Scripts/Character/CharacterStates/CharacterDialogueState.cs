using Tools;
using UnityEngine;

public class CharacterDialogueState : Tools.State
{
    CharacterBrain characterBrain;
    public CharacterDialogueState(StateMachine stateMachine) : base(stateMachine)
    {
        stateMachine.gameObject.GetComponent<CharacterBrain>();
    }

    public override void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public override void OnExit()
    {
        throw new System.NotImplementedException();
    }

    public override void OnUpdate()
    {


        throw new System.NotImplementedException();
    }
}
