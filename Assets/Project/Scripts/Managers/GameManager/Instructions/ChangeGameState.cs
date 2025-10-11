using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using System;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
[Title("Change Game State to Main Menu")]
[Description("Changes the current GameManager's game state to a new state.")]
public class ChangeGameStateToMainMenu : Instruction
{
    protected override Task Run(Args args)
    {
        GameManager.Instance.ChangeState(new MainMenuState());
        return DefaultResult;
    }
}

[Serializable]
[Title("Change Game State to Gameplay")]
[Description("Changes the current GameManager's game state to a new state.")]
public class ChangeGameStateToGameplay : Instruction
{
    protected override Task Run(Args args)
    {
        GameManager.Instance.ChangeState(new GameplayState());
        return DefaultResult;
    }
}