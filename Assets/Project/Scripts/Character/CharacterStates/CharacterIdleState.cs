using Tools;
using UnityEngine;

public class CharacterIdleState : Tools.State
{
    CharacterBrain characterBrain;
    public CharacterIdleState(StateMachine stateMachine) : base(stateMachine)
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
