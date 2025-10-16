using UnityEngine;

public class ActivityEnterScene : Activity
{
    public override string Name() => "Enter Scene";
    
    public override float EvaluateScore(CharacterBrain brain)
    {
        return 10.0f;
    }

    
    protected override void CreateSteps(CharacterBrain brain)
    {
        Level level = GameManager.Instance.GetLevel();
        steps.Add(new SpawnCharacterStep(level.GetRandomEntrance()));
        //steps.Add(new GoToMarkerStep());
    }

    protected override void Init(CharacterBrain brain)
    {
    }

    protected override void Shutdown(CharacterBrain brain)
    {
        brain.Activity().RemoveActivity<ActivityEnterScene>();
    }
}
