using UnityEngine;

public class ActivityExitScene : Activity
{
    public override float EvaluateScore(CharacterBrain brain)
    {
        return 1.0f;
    }

    protected override void CreateSteps(CharacterBrain brain)
    {
        Level level = GameManager.Instance.GetLevel();

        steps.Add(new GoToMarkerStep(level.GetRandomExit()));
        steps.Add(new DespawnCharacterStep()); 
    }

    protected override void Init(CharacterBrain brain)
    {
    }

    protected override void Shutdown(CharacterBrain brain)
    {
    }
}
