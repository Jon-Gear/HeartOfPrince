using GameCreator.Runtime.Common;
using UnityEngine;

public class GameplayState : GameState
{
    public override void Enter()
    {
        var gameManager = GameManager.Instance;

        gameManager.ActivateSystem<TimeManager>();
        gameManager.ActivateSystem<LightingManager>();

        gameManager.ActivateSystem<QuestManager>();
        gameManager.ActivateSystem<DialogueManager>();
        gameManager.ActivateSystem<CharacterManager>();
        gameManager.ActivateSystem<CharacterManager>();
    }

    public override void Exit()
    {
        var gameManager = GameManager.Instance;

        gameManager.DeactivateSystem<TimeManager>();
        gameManager.DeactivateSystem<QuestManager>();
        gameManager.DeactivateSystem<DialogueManager>();
        gameManager.DeactivateSystem<CharacterManager>();
        gameManager.DeactivateSystem<CharacterManager>();
    }

    public override void Update()
    {
    }
}
