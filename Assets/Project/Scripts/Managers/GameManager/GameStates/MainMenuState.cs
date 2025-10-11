using GameCreator.Runtime.Common;
using UnityEngine;

public class MainMenuState : GameState
{
    public override void Enter()
    {
        var gameManager = GameManager.Instance;

        gameManager.DeactivateSystem<TimeManager>();
        gameManager.DeactivateSystem<QuestManager>();
        gameManager.DeactivateSystem<DialogueManager>();
        gameManager.DeactivateSystem<CinemachineManager>();
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
    }   
}
